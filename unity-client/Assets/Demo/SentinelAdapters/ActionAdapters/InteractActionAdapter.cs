using UnityEngine;

public class InteractActionAdapter : MonoBehaviour, IActionAdapter {
    public string ActionName => "interact";

    [SerializeField] private PlayerInteraction playerInteraction;

    // Interaction finishes in the same frame, so all the work happens in Begin.
    public void Begin(ActionResponse request, ActionTracker tracker) {
        tracker.BeginAction(ActionName, request.target_id);

        // Use the same interaction path as human input and consume any pending human request.
        CommonIntent.Actions &= ~PlayerAction.Interact;

        try {
            if (playerInteraction == null || !playerInteraction.isActiveAndEnabled) {
                tracker.Fail("PlayerInteraction is missing or disabled on the player");
            } else if (playerInteraction.TryInteract(out string reason)) {
                tracker.Succeed();
            } else {
                tracker.Fail(reason);
            }
        } catch (System.Exception exception) {
            tracker.Fail("Interaction threw an exception: " + exception.Message);
            Debug.LogException(exception, this);
        }
    }

    public void Tick(float deltaTime) { }
    public void Cancel() { }

    private void Awake() {
        // The adapter normally sits on the player, next to PlayerInteraction.
        if (playerInteraction == null) {
            playerInteraction = GetComponent<PlayerInteraction>();
        }
    }

    private void OnEnable() {
        ActionRegistry.Register(this);
    }

    private void OnDisable() {
        ActionRegistry.Unregister(this);
    }
}
