using DG.Tweening; // Required for DOTween
using TMPro;
using UnityEngine;

public class MoleculeUI : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How high above the molecule the text should float")]
    float baseHeightOffset = 0.15f;

    private MoleculeController parentMolecule;
    private TextMeshPro detailsText;
    private Transform vrCamera;

    private void Start()
    {
        detailsText = GetComponent<TextMeshPro>();
        parentMolecule = GetComponentInParent<MoleculeController>();

        if (Camera.main != null)
        {
            vrCamera = Camera.main.transform;
        }

        if (parentMolecule != null && parentMolecule.moleculeData != null)
        {
            MoleculeSO data = parentMolecule.moleculeData;

            // CLEANER UI: Name and Formula grouped together. Extra words removed.
            detailsText.text =
                $"<b><color=#5CE1E6>{data.moleculeName}</color></b> <color=#DDDDDD>({data.formula})</color>\n"
                + $"<size=70%><color=#A0A0A0>{data.bondType} Bond</color>\n"
                + $"<color=#FF5757>Grab and Press Trigger to Break</color></size>";
        }

        // Reset local transforms just to be safe before animating
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        AnimateHologram();
    }

    private void AnimateHologram()
    {
        // 1. Prepare for entrance: Set scale to 0 and alpha to 0
        transform.localScale = Vector3.zero;

        Color startColor = detailsText.color;
        startColor.a = 0f;
        detailsText.color = startColor;

        // 2. Single crisp spawn animation (pop-up effect)
        transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);

        // 3. Fade in the text alpha smoothly
        detailsText.DOFade(1f, 0.3f);

        // Note: The continuous floating animation has been removed
        // so it feels solid and stable when grabbed!
    }

    private void Update()
    {
        // 1. Position it perfectly above the molecule
        if (parentMolecule != null)
        {
            transform.position =
                parentMolecule.transform.position + (Vector3.up * baseHeightOffset);
        }

        // 2. Rotate it to face the player's VR headset
        if (vrCamera != null)
        {
            // FIXED ROTATION: By subtracting camera position FROM transform position,
            // the Z-axis points away from the camera, causing the front of the text to face the player.
            Vector3 lookDirection = transform.position - vrCamera.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            }
        }
    }

    private void OnDestroy()
    {
        // CRITICAL: Always kill tweens when the object is destroyed so DOTween doesn't
        // keep trying to animate a deleted transform, which causes console errors.
        transform.DOKill();
        if (detailsText != null)
            detailsText.DOKill();
    }
}
