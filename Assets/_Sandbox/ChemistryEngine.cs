using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChemistryEngine : MonoBehaviour
{
    public static ChemistryEngine Instance { get; private set; }
    public TextMeshProUGUI debugText;

    [Header("Core Data")]
    public MoleculeDatabase moleculeDatabase;
    private List<MoleculeSO> sortedMolecules;

    [Header("Settings")]
    public float bondRadius = 0.5f; // Keep this slightly larger for VR comfort

    public static event Action<MoleculeSO> OnMoleculeFormed;

    // State Tracking
    private AtomController currentlyGrabbedAtom;
    private MoleculeSO pendingMolecule;
    private List<AtomController> pendingAtomsToConsume = new List<AtomController>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (moleculeDatabase != null && moleculeDatabase.validMolecules != null)
        {
            // Cache the sorted list so we don't modify the ScriptableObject
            sortedMolecules = moleculeDatabase
                .validMolecules.OrderByDescending(m => m.requiredAtoms.Count)
                .ToList();
        }
    }

    // --- NEW: State Management ---
    public void RegisterGrabbedAtom(AtomController atom)
    {
        currentlyGrabbedAtom = atom;
    }

    public void ProcessAtomDrop(AtomController droppedAtom)
    {
        if (currentlyGrabbedAtom == droppedAtom)
        {
            // If we drop the atom and there is a valid preview, spawn it!
            if (pendingMolecule != null && pendingAtomsToConsume.Count > 0)
            {
                SpawnMolecule(
                    pendingMolecule,
                    pendingAtomsToConsume,
                    droppedAtom.transform.position
                );
            }

            // Clean up state
            ClearPreviews();
            currentlyGrabbedAtom = null;
        }
    }

    // --- NEW: Continuous Scanning ---
    private void Update()
    {
        if (currentlyGrabbedAtom != null)
        {
            ScanForPotentialBonds();
        }
        else
        {
            // --- NEW: Clear UI when not holding anything ---
            if (debugText != null)
            {
                debugText.text = "Grab an atom to scan...";
            }
        }
    }

    private void ScanForPotentialBonds()
    {
        Collider[] colliders = Physics.OverlapSphere(
            currentlyGrabbedAtom.transform.position,
            bondRadius
        );

        // Only collect OTHER atoms — not the grabbed one
        List<AtomController> otherNearbyAtoms = new List<AtomController>();
        foreach (var col in colliders)
        {
            AtomController atom = col.GetComponent<AtomController>();
            if (atom != null && atom != currentlyGrabbedAtom && !atom.isGrabbed)
                otherNearbyAtoms.Add(atom);
        }

        // debugText.text = otherNearbyAtoms.Count().ToString();

        // Now build the full candidate list: grabbed atom + others
        List<AtomController> candidateAtoms = new List<AtomController>(otherNearbyAtoms);
        candidateAtoms.Add(currentlyGrabbedAtom);

        if (debugText != null)
        {
            if (otherNearbyAtoms.Count > 0)
            {
                string debugString =
                    $"<color=green>Overlapping ({candidateAtoms.Count}):</color>\n";
                foreach (var a in candidateAtoms)
                {
                    debugString += $"- {a.gameObject.name}\n";
                }
                debugText.text = debugString;
            }
            else
            {
                debugText.text =
                    $"<color=yellow>Scanning ({bondRadius}m)...</color>\nNo other atoms found.";
            }
        }

        // No other atoms nearby — clear and bail out immediately
        if (otherNearbyAtoms.Count == 0)
        {
            ClearPreviews();
            return;
        }

        foreach (MoleculeSO molecule in sortedMolecules)
        {
            if (CanFormMolecule(candidateAtoms, molecule, out List<AtomController> atomsToConsume))
            {
                UpdatePreviews(molecule, atomsToConsume);
                return;
            }
        }

        ClearPreviews();
    }

    private bool CanFormMolecule(
        List<AtomController> availableAtoms,
        MoleculeSO molecule,
        out List<AtomController> atomsToConsume
    )
    {
        atomsToConsume = new List<AtomController>();
        List<AtomController> pool = new List<AtomController>(availableAtoms);

        foreach (AtomSO requiredAtomData in molecule.requiredAtoms)
        {
            AtomController match = pool.FirstOrDefault(a =>
                a.atomData.elementType == requiredAtomData.elementType
            );

            if (match != null)
            {
                atomsToConsume.Add(match);
                pool.Remove(match);
            }
            else
            {
                return false; // Missing a required element
            }
        }

        // Rule: The potential molecule MUST require the atom the player is currently holding
        if (!atomsToConsume.Contains(currentlyGrabbedAtom))
        {
            return false;
        }

        return true;
    }

    // --- NEW: Visual Feedback Logic ---
    private void UpdatePreviews(MoleculeSO newPendingMolecule, List<AtomController> newPendingAtoms)
    {
        // 1. Always turn off old outlines first to prevent getting "stuck"
        ClearPreviews();

        // 2. Set new state
        pendingMolecule = newPendingMolecule;

        // Create a strict COPY of the list to prevent memory reference bugs
        pendingAtomsToConsume = new List<AtomController>(newPendingAtoms);

        // 3. Turn on new outlines
        foreach (var atom in pendingAtomsToConsume)
        {
            if (atom != null)
                atom.SetOutline(true);
        }
    }

    private void ClearPreviews()
    {
        // Safely turn off all current outlines
        foreach (var atom in pendingAtomsToConsume)
        {
            if (atom != null)
                atom.SetOutline(false);
        }

        // Clear the memory
        pendingAtomsToConsume.Clear();
        pendingMolecule = null;
    }

    // --- UPDATED: Spawning ---
    private void SpawnMolecule(MoleculeSO molecule, List<AtomController> consumed, Vector3 spawnPos)
    {
        Instantiate(molecule.moleculePrefab, spawnPos, Quaternion.identity);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBondSound(spawnPos);
        }

        // Destroy the used atoms
        foreach (var atom in consumed)
        {
            // You can remove the aggressive XR drop fix here because the user
            // has ALREADY dropped the atom to trigger this spawn!
            Destroy(atom.gameObject);
        }

        OnMoleculeFormed?.Invoke(molecule);
        Debug.Log($"[ChemistryEngine] Formed: {molecule.moleculeName}");
    }
}
