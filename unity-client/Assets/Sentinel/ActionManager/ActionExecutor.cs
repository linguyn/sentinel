using UnityEngine;

public class ActionExecutor : MonoBehaviour {

    [SerializeField] private ApiClients apiClients;

    // Maximum total time for any action before it is cancelled.
    [SerializeField] private float actionTimeout = 30f;

    private readonly ActionTracker actionTracker = new();

    private ActionResponse trackedResponse;
    private IActionAdapter currentAdapter;
    private Vector3 targetPosition;

    public ActionTracker Tracker => actionTracker;

    public bool ActionCompleted => actionTracker.IsFinished;

    // Read by the debug UI to show where the current action is heading.
    public Vector3 TargetPosition => targetPosition;

    public void ResetActionCompletion() {
        actionTracker.Reset();
    }

    // Stops the running action from outside, e.g. when the session stops.
    public void CancelCurrent(string reason = "CANCELLED") {
        if (currentAdapter != null && actionTracker.Status == ActionStatus.Running) {
            currentAdapter.Cancel();
            actionTracker.Fail(reason);
        }

        currentAdapter = null;
    }

    private void HandleActionResponse() {
        ActionResponse response = apiClients.GetActionResponse;

        if (response == null) { return; }

        // Detect a newly received command and hand it to the matching adapter.
        if (!ReferenceEquals(trackedResponse, response)) {
            trackedResponse = response;

            if (response.position != null) {
                targetPosition = new Vector3(response.position.x, response.position.y, response.position.z);
            }

            if (!ActionRegistry.TryGet(response.action, out currentAdapter)) {
                actionTracker.BeginAction(response.action, response.target_id);
                actionTracker.Fail($"No adapter registered for action '{response.action}'");
                apiClients.ClearActionResponse();
                return;
            }

            currentAdapter.Begin(response, actionTracker);
        }

        if (actionTracker.Status == ActionStatus.Running) {
            actionTracker.Tick(Time.deltaTime);
            currentAdapter.Tick(Time.deltaTime);

            // The adapter did not finish in time; stop it from outside.
            if (actionTracker.Status == ActionStatus.Running && actionTracker.ElapsedTime >= actionTimeout) {
                CancelCurrent("TIMEOUT");
            }
        }

        // The adapter reports success or failure itself
        if (actionTracker.IsFinished) {
            currentAdapter = null;
            apiClients.ClearActionResponse();
        }
    }

    private void Update() {
        HandleActionResponse();
    }
}
