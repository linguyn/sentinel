public enum ActionStatus {
    Idle,
    Running,
    Succeeded,
    Failed
}

public class ActionTracker {
    public ActionStatus Status { get; private set; }
        = ActionStatus.Idle;

    public string ActionName { get; private set; }
    public string TargetId { get; private set; }
    public string FailureReason { get; private set; }

    // Keep the completed result available while the session starts its next action.
    public ActionStatus? LastResult { get; private set; }
    public string LastFailureReason { get; private set; }

    public float ElapsedTime { get; private set; }
    public float TimeWithoutProgress { get; private set; }

    public bool IsFinished =>
        Status == ActionStatus.Succeeded ||
        Status == ActionStatus.Failed;

    public void BeginAction(string actionName, string targetId = null) {
        ActionName = actionName;
        TargetId = targetId;
        FailureReason = null;

        ElapsedTime = 0f;
        TimeWithoutProgress = 0f;

        Status = ActionStatus.Running;
    }

    public void Tick(float deltaTime) {
        if (Status != ActionStatus.Running)
            return;

        ElapsedTime += deltaTime;
        TimeWithoutProgress += deltaTime;
    }

    public void ReportProgress() {
        if (Status != ActionStatus.Running)
            return;

        TimeWithoutProgress = 0f;
    }

    public void Succeed() {
        if (Status != ActionStatus.Running)
            return;

        Status = ActionStatus.Succeeded;
        LastResult = Status;
        LastFailureReason = null;
    }

    public void Fail(string reason) {
        if (Status != ActionStatus.Running)
            return;

        FailureReason = reason;
        Status = ActionStatus.Failed;
        LastResult = Status;
        LastFailureReason = reason;
    }

    public void Reset() {
        Status = ActionStatus.Idle;
        ActionName = null;
        TargetId = null;
        FailureReason = null;

        ElapsedTime = 0f;
        TimeWithoutProgress = 0f;
    }
}
