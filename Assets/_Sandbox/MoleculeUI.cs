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

        // 2. Rotate it to face the player's VR headset around the world Y axis only
        if (vrCamera != null)
        {
            Vector3 lookDirection = vrCamera.position - transform.position;
            lookDirection.y = 0f;
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            }
        }
    }
}
