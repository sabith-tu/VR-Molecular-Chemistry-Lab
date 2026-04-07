using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
// Use the namespace from your original script
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AtomController : MonoBehaviour
{
    public AtomSO atomData;
    public bool isGrabbed = false;

    [Header("Visuals")]
    public GameObject outlineObject;

    // Cache the XR component
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        outlineObject.SetActive(false);
        // Dynamically find the component on this prefab
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError($"[Atom] No XRGrabInteractable found on {gameObject.name}!");
        }
    }

    private void OnEnable()
    {
        // Subscribe to the grab/drop events dynamically when the atom spawns
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnDropped);
        }
    }

    private void OnDisable()
    {
        // Always unsubscribe when disabled/destroyed to prevent memory leaks
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnDropped);
        }
    }

    public void SetOutline(bool show)
    {
        if (outlineObject != null)
            outlineObject.SetActive(show);
    }

    // Notice: We added 'SelectEnterEventArgs' because the XR event requires it
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log($"<color=green>[Atom]</color> {gameObject.name} dynamically GRABBED!");
        isGrabbed = true;
        ChemistryEngine.Instance.RegisterGrabbedAtom(this);
    }

    // Notice: We added 'SelectExitEventArgs' because the XR event requires it
    private void OnDropped(SelectExitEventArgs args)
    {
        Debug.Log($"<color=red>[Atom]</color> {gameObject.name} dynamically DROPPED!");
        isGrabbed = false;
        ChemistryEngine.Instance.ProcessAtomDrop(this);
    }

    public void ResetState()
    {
        isGrabbed = false;
        SetOutline(false);
    }
}
