using System.Collections.Generic;
using DG.Tweening; // Required for DOTween
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI libraryText;
    private HashSet<string> discoveredMolecules = new HashSet<string>();

    [SerializeField]
    private TMP_Dropdown secondPanelType;

    [SerializeField]
    private GameObject secondPanel_cheatSheet,
        secondPanel_debug;

    // Store the original color of the text so we can flash it and return it to normal
    private Color originalTextColor;

    private void OnEnable()
    {
        originalTextColor = libraryText.color;
        libraryText.text = "Discovered Molecules";
        ChemistryEngine.OnMoleculeFormed += HandleMoleculeFormed;
        secondPanelType.onValueChanged.AddListener(OnSecondPanelTypeChange);
    }

    void Start()
    {
        OnSecondPanelTypeChange(secondPanelType.value); // Use actual dropdown value on start
    }

    private void OnDisable()
    {
        secondPanelType.onValueChanged.RemoveListener(OnSecondPanelTypeChange);
        ChemistryEngine.OnMoleculeFormed -= HandleMoleculeFormed;
    }

    private void HandleMoleculeFormed(MoleculeSO newMolecule)
    {
        if (discoveredMolecules.Add(newMolecule.moleculeName))
        {
            UpdateLibraryUI();
            AnimateTextPolish(); // Trigger the DOTween animation
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

    // --- DOTWEEN ANIMATIONS ---

    private void AnimateTextPolish()
    {
        // 1. Kill any existing tweens on this text so they don't overlap if triggered rapidly
        libraryText.transform.DOKill();
        libraryText.DOKill();

        // 2. Reset scale and color just in case a previous tween was interrupted
        libraryText.transform.localScale = Vector3.one;
        libraryText.color = originalTextColor;

        // 3. Make the text physically "bounce" or "punch" outwards
        libraryText.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0f), 0.4f, 5, 1f);

        // 4. Flash the text green to indicate a successful discovery
        libraryText.DOColor(Color.green, 0.2f).SetLoops(2, LoopType.Yoyo);
    }

    public void ResetDiscovery()
    {
        AudioManager.Instance.PlayReset(transform.position);
        discoveredMolecules.Clear();
        libraryText.text = "Discovered Molecules";

        // Add a little shake animation when resetting
        libraryText.transform.DOShakePosition(0.3f, 10f, 20);
    }

    // --- PANEL TRANSITIONS ---

    public void OnSecondPanelTypeChange(int newType)
    {
        // Smoothly hide both first
        HidePanel(secondPanel_cheatSheet);
        HidePanel(secondPanel_debug);

        // Then smoothly show the requested one
        switch (newType)
        {
            case 0:
                // Option 1: None (Both stay hidden)
                break;
            case 1:
                ShowPanel(secondPanel_cheatSheet);
                break;
            case 2:
                ShowPanel(secondPanel_debug);
                break;
        }
    }

    private void ShowPanel(GameObject panel)
    {
        if (panel == null)
            return;

        panel.SetActive(true);
        panel.transform.DOKill(); // Stop any hiding tweens

        // Start tiny and spring open using OutBack ease
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    private void HidePanel(GameObject panel)
    {
        if (panel == null || !panel.activeSelf)
            return;

        panel.transform.DOKill(); // Stop any showing tweens

        // Shrink down quickly, then deactivate the GameObject so it doesn't waste performance
        panel
            .transform.DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                panel.SetActive(false);
            });
    }
}
