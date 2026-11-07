using UnityEngine;

public class MoveActionAdapter : MonoBehaviour, IActionAdapter {
    public string ActionName => "move";

    [SerializeField] private Transform playerTransform;
    [SerializeField] private float stopDistance = 0.2f;
    [SerializeField] private float stuckLimit = 3f;
    [SerializeField] private float minProgress = 0.05f;

    private ActionTracker tracker;
    private Vector3 targetPosition;
    private float lastProgressDistance;

    public void Begin(ActionResponse request, ActionTracker tracker) {
        this.tracker = tracker;

        if (request.position == null) {
            tracker.BeginAction(ActionName, request.target_id);
            tracker.Fail("Missing target position");
            CommonIntent.MoveDirection = Vector3.zero;
            return;
        }

        targetPosition = new Vector3(
            request.position.x,
            request.position.y,
            request.position.z
        );

        lastProgressDistance = HorizontalDistanceToTarget();

        tracker.BeginAction(ActionName);
    }

    public void Tick(float deltaTime) {
        float distance = HorizontalDistanceToTarget();

        // SUCCESS
        if (distance <= stopDistance) {
            tracker.Succeed();
            CommonIntent.MoveDirection = Vector3.zero;
            return;
        }

        // PROGRESS
        if (lastProgressDistance - distance >= minProgress) {
            lastProgressDistance = distance;
            tracker.ReportProgress();
        }

        // FAILURE
        if (tracker.TimeWithoutProgress >= stuckLimit) {
            tracker.Fail("STUCK");
            CommonIntent.MoveDirection = Vector3.zero;
            return;
        }

        // CONTINUE MOVING
        Vector3 direction = targetPosition - playerTransform.position;
        direction.y = 0f;
        CommonIntent.MoveDirection = direction.normalized;
    }

    public void Cancel() {
        CommonIntent.MoveDirection = Vector3.zero;
    }

    private float HorizontalDistanceToTarget() {
        Vector3 direction = targetPosition - playerTransform.position;
        direction.y = 0f;
        return direction.magnitude;
    }

    private void Awake() {
        // The adapter normally sits on the player itself.
        if (playerTransform == null) {
            playerTransform = transform;
        }
    }

    private void OnEnable() {
        ActionRegistry.Register(this);
    }

    private void OnDisable() {
        ActionRegistry.Unregister(this);
    }
}
