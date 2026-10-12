
using UnityEngine;
using TMPro;

public class SentinelDebugMonitor : MonoBehaviour {
    [Header("Data Sources")]
    [SerializeField] private AIIntent aiIntent;
    [SerializeField] private Transform playerTransform;

    [Header("UI Values")]
    [SerializeField] private TMP_Text playerPositionValue;
    [SerializeField] private TMP_Text targetPositionValue;
    [SerializeField] private TMP_Text distanceToTargetValue;
    [SerializeField] private TMP_Text currentActionValue;
    [SerializeField] private TMP_Text actionStatusValue;
    [SerializeField] private TMP_Text actionResultValue;

    private void Update() {
        if (aiIntent == null || playerTransform == null)
            return;

        UpdatePlayerPosition();
        UpdateActionInformation();
    }

    private void UpdatePlayerPosition() {
        Vector3 position = playerTransform.position;

        playerPositionValue.text =
            $"({position.x:F2}, {position.y:F2}, {position.z:F2})";
    }

    private void UpdateActionInformation() {
        ActionTracker tracker = aiIntent.Tracker;

        ActionStatus status = tracker.Status;

        // Current action
        currentActionValue.text =
            tracker.ActionName ?? "NONE";

        // Action status
        actionStatusValue.text = status.ToString().ToUpperInvariant();

        if (actionResultValue != null) {
            actionResultValue.text = tracker.LastResult.HasValue
                ? tracker.LastResult.Value.ToString().ToUpperInvariant()
                : "-";

            if (tracker.LastResult == ActionStatus.Failed &&
                !string.IsNullOrEmpty(tracker.LastFailureReason)) {
                actionResultValue.text += $" ({tracker.LastFailureReason})";
            }
        }

        // No active or completed action
        if (status == ActionStatus.Idle) {
            targetPositionValue.text = "-";
            distanceToTargetValue.text = "-";
            return;
        }

        // Currently MOVE is the only supported AI action.
        if (tracker.ActionName == "move") {
            Vector3 target = aiIntent.TargetPosition;

            targetPositionValue.text =
                $"({target.x:F2}, {target.y:F2}, {target.z:F2})";

            // Match AIIntent's horizontal distance calculation.
            Vector3 direction = target - playerTransform.position;
            direction.y = 0f;

            distanceToTargetValue.text =
                $"{direction.magnitude:F2} units";
        } else {
            targetPositionValue.text = "-";
            distanceToTargetValue.text = "-";
        }
    }
}
