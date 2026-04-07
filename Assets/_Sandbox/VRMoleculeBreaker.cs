using UnityEngine;
using UnityEngine.InputSystem;


public class VRMoleculeBreaker : MonoBehaviour
{
    [Header("XR Settings")]
    [Tooltip("The interactor attached to this hand (e.g., XR Direct Interactor)")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor handInteractor;

    [Tooltip("The input action for the button press (e.g., Primary Button)")]
    public InputActionReference breakButtonAction;

    private void OnEnable()
    {
        if (breakButtonAction != null)
        {
            breakButtonAction.action.Enable();
            breakButtonAction.action.performed += OnButtonPressed;
        }
    }

    private void OnDisable()
    {
        if (breakButtonAction != null)
        {
            breakButtonAction.action.performed -= OnButtonPressed;
        }
    }

    private void OnButtonPressed(InputAction.CallbackContext context)
    {
        // 1. Check if this hand is currently holding anything at all
        if (handInteractor.hasSelection)
        {
            // 2. Get the specific object currently being held by this hand
            UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable grabbedObject = handInteractor.firstInteractableSelected;

            if (grabbedObject != null)
            {
                // 3. Try to find the MoleculeController script on the held object
                MoleculeController molecule =
                    grabbedObject.transform.GetComponent<MoleculeController>();

                if (molecule != null)
                {
                    // 4. It's a molecule! Break it apart.
                    molecule.BreakApart();
                    Debug.Log($"{gameObject.name} broke a molecule!");
                }
            }
        }
    }
}
