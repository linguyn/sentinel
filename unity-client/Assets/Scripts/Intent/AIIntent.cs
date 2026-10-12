using System.Runtime.CompilerServices;
using UnityEngine;

public class AIIntent : MonoBehaviour {

    private Vector3 targetPosition;
    [SerializeField] private float stopDistance = 0.2f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private ApiClients apiClients; 

    [SerializeField] private float stuckLimit = 3f;
    [SerializeField] private float minProgress = 0.05f;

    private readonly ActionTracker actionTracker = new();

    private ActionResponse trackedResponse;
    private float lastProgressDistance;

    public ActionTracker Tracker => actionTracker;

    public bool ActionCompleted => actionTracker.IsFinished;

    public Vector3 TargetPosition => targetPosition;

    public void ResetActionCompletion() {
        actionTracker.Reset();
    }

    private void HandleActionResponse() {
        ActionResponse response = apiClients.GetActionResponse;

        if (response == null) { return; }

        string action = response.action.ToLower();

        /*Move*/
        if (response.action == "move") {
            // Detect a newly received command
            if (!ReferenceEquals(trackedResponse, response)) {
                trackedResponse = response;

                if (response.position == null) {
                    actionTracker.BeginAction("move");
                    actionTracker.Fail("Missing target position");

                    CommonIntent.MoveDirection = Vector3.zero;
                    apiClients.ClearActionResponse();
                    return;
                }

                targetPosition = new Vector3(
                    response.position.x,
                    response.position.y,
                    response.position.z
                );

                Vector3 initialDirection =
                    targetPosition - playerTransform.position;

                initialDirection.y = 0f;

                lastProgressDistance = initialDirection.magnitude;

                actionTracker.BeginAction("move");
            }

            if (actionTracker.Status != ActionStatus.Running)
                return;

            actionTracker.Tick(Time.deltaTime);

            Vector3 direction =
                targetPosition - playerTransform.position;

            direction.y = 0f;

            float distance = direction.magnitude;

            // SUCCESS
            if (distance <= stopDistance) {
                actionTracker.Succeed();

                CommonIntent.MoveDirection = Vector3.zero;
                apiClients.ClearActionResponse();
                return;
            }

            // PROGRESS
            if (lastProgressDistance - distance >= minProgress) {
                lastProgressDistance = distance;
                actionTracker.ReportProgress();
            }

            // FAILURE
            if (actionTracker.TimeWithoutProgress >= stuckLimit) {
                actionTracker.Fail("STUCK");

                CommonIntent.MoveDirection = Vector3.zero;
                apiClients.ClearActionResponse();
                return;
            }

            // CONTINUE MOVING
            CommonIntent.MoveDirection = direction.normalized;
        }


    }

    private void Update() {
        HandleActionResponse(); 
    }
}