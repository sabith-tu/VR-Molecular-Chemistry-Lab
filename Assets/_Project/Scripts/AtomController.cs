using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
// Use the namespace from your original script
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AtomController : MonoBehaviour
{
    public AtomSO atomData;
    public bool isGrabbed = false;

    [Header("Visuals")]
    public GameObject snapOutlineObject;
    public GameObject selectOutlineObject;

    // Cache the XR component
    private XRGrabInteractable grabInteractable;

    public static List<AtomController> allAtomControllers = new();

    private void Awake()
    {
        snapOutlineObject.SetActive(false);
        selectOutlineObject.SetActive(false);
        // Dynamically find the component on this prefab
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError($"[Atom] No XRGrabInteractable found on {gameObject.name}!");
        }
    }

    private void OnEnable()
    {
        allAtomControllers.Add(this);
        // Subscribe to the grab/drop events dynamically when the atom spawns
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnDropped);
        }
    }

    private void OnDisable()
    {
        allAtomControllers.Remove(this);

        // Always unsubscribe when disabled/destroyed to prevent memory leaks
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnDropped);
        }
    }

    public void SetOutline(bool show)
    {
        if (show)
        {
            selectOutlineObject.SetActive(false);
        }
        else if (isGrabbed)
        {
            selectOutlineObject.SetActive(true);
        }

        if (snapOutlineObject != null)
            snapOutlineObject.SetActive(show);
    }

    // Notice: We added 'SelectEnterEventArgs' because the XR event requires it
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log($"<color=green>[Atom]</color> {gameObject.name} dynamically GRABBED!");
        selectOutlineObject.SetActive(true);
        isGrabbed = true;
        ChemistryEngine.Instance.RegisterGrabbedAtom(this);
    }

    // Notice: We added 'SelectExitEventArgs' because the XR event requires it
    private void OnDropped(SelectExitEventArgs args)
    {
        Debug.Log($"<color=red>[Atom]</color> {gameObject.name} dynamically DROPPED!");
        selectOutlineObject.SetActive(false);
        isGrabbed = false;
        ChemistryEngine.Instance.ProcessAtomDrop(this);
    }

    public void ResetState()
    {
        isGrabbed = false;
        SetOutline(false);
    }
}
