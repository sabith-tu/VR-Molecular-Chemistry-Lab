using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI libraryText; // Drag your panel's text component here
    private HashSet<string> discoveredMolecules = new HashSet<string>();

    private void OnEnable()
    {
        libraryText.text = "Discovered Molecules";
        ChemistryEngine.OnMoleculeFormed += HandleMoleculeFormed;
    }

    private void OnDisable()
    {
        ChemistryEngine.OnMoleculeFormed -= HandleMoleculeFormed;
    }

    private void HandleMoleculeFormed(MoleculeSO newMolecule)
    {
        // HashSet prevents duplicates if the player makes Water twice
        if (discoveredMolecules.Add(newMolecule.moleculeName))
        {
            UpdateLibraryUI();
        }
    }

    private void UpdateLibraryUI()
    {
        libraryText.text = "<b>Discovered Molecules:</b>\n\n";
        foreach (string mol in discoveredMolecules)
        {
            libraryText.text += $"- {mol}\n";
        }
    }

    public void ResetDiscovery()
    {
        discoveredMolecules.Clear();
        libraryText.text = "Discovered Molecules";
    }
}
