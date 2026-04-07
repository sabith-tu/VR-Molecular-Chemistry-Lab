using TMPro;
using UnityEngine;

public class MoleculeUI : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How high above the molecule the text should float")]
    public float heightOffset = 0.3f;

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
            detailsText.text =
                $"{data.moleculeName}\nFormula: {data.formula}\nBond: {data.bondType}";
        }
    }

    private void Update()
    {
        // 1. Position it perfectly above the molecule
        if (parentMolecule != null)
        {
            // Vector3.up is absolute world-up. It ignores the molecule's rotation.
            transform.position = parentMolecule.transform.position + (Vector3.up * heightOffset);
        }

        // 2. Rotate it to face the player's VR headset
        if (vrCamera != null)
        {
            transform.LookAt(
                transform.position + vrCamera.rotation * Vector3.forward,
                vrCamera.rotation * Vector3.up
            );
        }
    }
}
