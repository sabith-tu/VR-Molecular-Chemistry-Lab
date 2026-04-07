using UnityEngine;


public class MoleculeController : MonoBehaviour
{
    public MoleculeSO moleculeData;

    public void BreakApart()
    {
        // 1. Check if it's currently held and force the hand to drop it
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            grabInteractable.interactionManager.CancelInteractableSelection(
                (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grabInteractable
            );
        }

        // 2. Spawn atoms
        foreach (AtomSO atomData in moleculeData.requiredAtoms)
        {
            Vector3 randomOffset = Random.insideUnitSphere * 0.15f;
            Instantiate(
                atomData.atomPrefab,
                transform.position + randomOffset,
                Quaternion.identity
            );
        }

        // 3. Destroy safely
        Destroy(gameObject);
        AudioManager.Instance.PlayBreakSound(transform.position);
    }
}
