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
    private List<AtomController> currentlyGrabbedAtoms = new List<AtomController>();
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

    public void RegisterGrabbedAtom(AtomController atom)
    {
        if (!currentlyGrabbedAtoms.Contains(atom))
        {
            currentlyGrabbedAtoms.Add(atom);
        }
    }

    public void ProcessAtomDrop(AtomController droppedAtom)
    {
        if (currentlyGrabbedAtoms.Contains(droppedAtom))
        {
            // If we drop an atom that is part of a valid preview, spawn it!
            if (pendingMolecule != null && pendingAtomsToConsume.Contains(droppedAtom))
            {
                SpawnMolecule(
                    pendingMolecule,
                    pendingAtomsToConsume,
                    droppedAtom.transform.position
                );
            }

            currentlyGrabbedAtoms.Remove(droppedAtom);

            // Clean up state
            ClearPreviews();
        }
    }

    private void Update()
    {
        // Clean out any unexpectedly destroyed atoms from the list
        currentlyGrabbedAtoms.RemoveAll(atom => atom == null);

        if (currentlyGrabbedAtoms.Count > 0)
        {
            ScanForPotentialBonds();
        }
        else
        {
            if (debugText != null)
            {
                debugText.text = "Grab an atom to scan...";
            }
        }
    }

    private void ScanForPotentialBonds()
    {
        List<AtomController> largestLocalCluster = new List<AtomController>();

        // 1. Scan the local area around EACH grabbed hand individually
        foreach (var grabbedAtom in currentlyGrabbedAtoms)
        {
            List<AtomController> localCandidates = new List<AtomController>();
            localCandidates.Add(grabbedAtom); // Center the cluster on this specific hand

            // Gather physical overlaps (for atoms sitting on the table)
            Collider[] colliders = Physics.OverlapSphere(
                grabbedAtom.transform.position,
                bondRadius
            );
            foreach (var col in colliders)
            {
                AtomController atom = col.GetComponent<AtomController>();
                if (atom != null && !localCandidates.Contains(atom))
                {
                    localCandidates.Add(atom);
                }
            }

            // 2. TRUE DISTANCE CHECK: Manually check distance to the OTHER hand
            // (This bypasses XR disabling colliders, but still enforces physical distance)
            foreach (var otherGrabbed in currentlyGrabbedAtoms)
            {
                if (!localCandidates.Contains(otherGrabbed))
                {
                    float distance = Vector3.Distance(
                        grabbedAtom.transform.position,
                        otherGrabbed.transform.position
                    );
                    if (distance <= bondRadius)
                    {
                        localCandidates.Add(otherGrabbed);
                    }
                }
            }

            // Track the largest cluster just so the Debug Text looks nice
            if (localCandidates.Count > largestLocalCluster.Count)
            {
                largestLocalCluster = new List<AtomController>(localCandidates);
            }

            // 3. Test if THIS specific local cluster can form a molecule
            if (localCandidates.Count >= 2)
            {
                foreach (MoleculeSO molecule in sortedMolecules)
                {
                    // Notice we pass 'grabbedAtom' to guarantee the molecule is built around THIS hand
                    if (
                        CanFormMolecule(
                            localCandidates,
                            molecule,
                            grabbedAtom,
                            out List<AtomController> atomsToConsume
                        )
                    )
                    {
                        UpdateDebugUI(largestLocalCluster);

                        if (pendingMolecule != molecule && AudioManager.Instance != null)
                        {
                            AudioManager.Instance.PlaySnap(atomsToConsume[0].transform.position);
                        }

                        UpdatePreviews(molecule, atomsToConsume);
                        return; // Stop scanning once we find a valid match!
                    }
                }
            }
        }

        // If we get here, no matches were found for ANY hand
        UpdateDebugUI(largestLocalCluster);
        ClearPreviews();
    }

    private void UpdateDebugUI(List<AtomController> candidates)
    {
        if (debugText == null)
            return;

        if (candidates.Count > 1)
        {
            string debugString = $"<color=green>Near Hands ({candidates.Count}):</color>\n";
            foreach (var a in candidates)
            {
                debugString += $"- {a.gameObject.name}\n";
            }
            debugText.text = debugString;
        }
        else
        {
            debugText.text =
                $"<color=yellow>Scanning ({bondRadius}m)...</color>\nBring atoms closer.";
        }
    }

    private bool CanFormMolecule(
        List<AtomController> availableAtoms,
        MoleculeSO molecule,
        AtomController anchorAtom, // NEW: We enforce that this specific atom is used
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

        // Rule: The potential molecule MUST contain the specific atom we are anchoring the scan to
        if (!atomsToConsume.Contains(anchorAtom))
        {
            return false;
        }

        return true;
    }

    private void UpdatePreviews(MoleculeSO newPendingMolecule, List<AtomController> newPendingAtoms)
    {
        ClearPreviews();

        pendingMolecule = newPendingMolecule;
        pendingAtomsToConsume = new List<AtomController>(newPendingAtoms);

        foreach (var atom in pendingAtomsToConsume)
        {
            if (atom != null)
                atom.SetOutline(true);
        }
    }

    private void ClearPreviews()
    {
        foreach (var atom in pendingAtomsToConsume)
        {
            if (atom != null)
                atom.SetOutline(false);
        }

        pendingAtomsToConsume.Clear();
        pendingMolecule = null;
    }

    public void ResetAllMolecules()
    {
        AudioManager.Instance.PlayReset(transform.position);
        MoleculeController.allMoleculeControllers.ToList().ForEach(item => item?.BreakApart());
    }

    public void DestroyAllAtoms()
    {
        AudioManager.Instance.PlayReset(transform.position);
        AtomController
            .allAtomControllers.ToList()
            .ForEach(item =>
            {
                if (item)
                    Destroy(item.gameObject);
            });
    }

    private void SpawnMolecule(MoleculeSO molecule, List<AtomController> consumed, Vector3 spawnPos)
    {
        GameObject spawnedMolecule = Instantiate(
            molecule.moleculePrefab,
            spawnPos,
            Quaternion.identity
        );
        spawnedMolecule.transform.localScale = Vector3.one * 0.1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBondSound(spawnPos);
        }

        foreach (var atom in consumed)
        {
            if (currentlyGrabbedAtoms.Contains(atom))
            {
                currentlyGrabbedAtoms.Remove(atom);
            }
            Destroy(atom.gameObject);
        }

        OnMoleculeFormed?.Invoke(molecule);
        Debug.Log($"[ChemistryEngine] Formed: {molecule.moleculeName}");
    }
}
