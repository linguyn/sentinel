using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour {
    private PlayerInput playerInput;
    private InputAction interactAction;

    private RaycastHit hit;
    private IInteractable proximityInteractable;
    private IInteractable aimedInteractable;
    private IInteractable CurrentInteractable => proximityInteractable ?? aimedInteractable;

    private string binding;
    

    /* The maximum distance at which the player can interact with objects */
    [SerializeField]
    private InteractionUI interactionUI;
    [SerializeField]
    private float interactionDistance = 2f;
    [SerializeField]
    private float interactionHeight = 2.7f;
    [SerializeField]
    private LayerMask layersToHit;

    private void DetectInteractable() {

        aimedInteractable = null;

        Vector3 origin = transform.position + Vector3.up * interactionHeight;
        Vector3 direction = transform.forward;

        if (proximityInteractable == null && 
            Physics.Raycast(origin, 
            direction, 
            out hit, 
            interactionDistance, 
            layersToHit)) 
        {
            aimedInteractable = hit.collider.GetComponentInParent<IInteractable>() 
                                ?? hit.collider.GetComponentInChildren<IInteractable>();
        } 
    }


    /*Handle player input for interaction*/
    private void HandleInteractionRequest() {
        if (CurrentInteractable != null && CommonIntent.HumanAction.HasFlag(PlayerAction.Interact)) {
            CurrentInteractable.Interact();
        }

        CommonIntent.HumanAction &= ~PlayerAction.Interact;
    }


    /*Ensure that the player can interact with objects that are in proximity, 
     * even if they are not directly in front of the player.*/
    private void OnTriggerEnter(Collider other) {
        IInteractable interactable = other.GetComponentInParent<IInteractable>() 
                                    ?? other.GetComponentInChildren<IInteractable>();
        if (interactable != null) {
            proximityInteractable = interactable;
        }
    }

    private void OnTriggerExit(Collider other) {
        IInteractable interactable = other.GetComponentInParent<IInteractable>() 
                                    ?? other.GetComponentInChildren<IInteractable>();

        if (proximityInteractable == interactable) {
            proximityInteractable = null;
        }
    }

    private void UpdateInteractionUI() {
        if (CurrentInteractable == null) {
            interactionUI.Hide();
            return;
        }

        string prompt = CurrentInteractable.GetPrompt();
        if (prompt == "Locked") {
            interactionUI.Show(prompt);
        } else {
            interactionUI.Show($"[{binding}]" + " " + prompt);
        }
    }

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        interactAction = playerInput.actions["Interact"];
        binding = interactAction.GetBindingDisplayString();
    }
    void Update() {
        DetectInteractable();
        HandleInteractionRequest(); 
        UpdateInteractionUI();
    }

}