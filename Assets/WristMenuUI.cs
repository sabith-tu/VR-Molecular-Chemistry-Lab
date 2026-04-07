using System.Collections.Generic;
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
            int buttonIndex = i; // Capture the current index value
            buttonComponent.onClick.AddListener(() =>
            {
                GameObject spawnedAtom = Instantiate(
                    atoms[buttonIndex].atomPrefab,
                    camTransform.position + camTransform.forward,
                    Quaternion.identity
                );

                AudioManager.Instance.PlayAtomCreated(spawnedAtom.transform.position);
            });
            spawnedButton.GetComponentInChildren<TextMeshProUGUI>().text = atoms[i].symbol;
        }
    }
}
