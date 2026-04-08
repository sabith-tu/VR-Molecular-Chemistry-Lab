using System.Collections.Generic;
using DG.Tweening; // Required for DOTween
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WristMenuUI : MonoBehaviour
{
    [SerializeField]
    private List<AtomSO> atoms;

    [SerializeField]
    private GameObject buttonPrefab;
    float distanceBetweenButtons = 60;
    private Transform camTransform;

    private void Start()
    {
        camTransform = Camera.main.transform;

        for (int i = 0; i < atoms.Count; i++)
        {
            float xOffset = -i * distanceBetweenButtons;
            GameObject spawnedButton = Instantiate(buttonPrefab, transform);

            RectTransform buttonRectTransform = spawnedButton.GetComponent<RectTransform>();
            buttonRectTransform.anchoredPosition = new Vector3(xOffset, 0, 0);
            buttonRectTransform.localScale = Vector3.one;

            Button buttonComponent = spawnedButton.GetComponent<Button>();

            // 1. Cache the Text component so we can animate it
            TextMeshProUGUI btnText = spawnedButton.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = atoms[i].symbol;

            int buttonIndex = i;
            buttonComponent.onClick.AddListener(() =>
            {
                // --- DOTWEEN TEXT ANIMATION ---
                // Kill any existing tween so it doesn't glitch if the user spam-clicks
                btnText.transform.DOKill();
                // Reset scale to normal before punching
                btnText.transform.localScale = Vector3.one;
                // Punch scale: inflates by 20% (0.2f), lasts 0.3 seconds
                btnText.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.3f, 5, 1f);

                // Original spawn logic
                GameObject spawnedAtom = Instantiate(
                    atoms[buttonIndex].atomPrefab,
                    camTransform.position + camTransform.forward,
                    Quaternion.identity
                );

                AudioManager.Instance.PlayAtomCreated(spawnedAtom.transform.position);
            });
        }
    }
}
