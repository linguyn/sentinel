using UnityEngine;

public class SessionManager : MonoBehaviour {
    [SerializeField] private ApiClients apiClients;
    [SerializeField] private AIIntent aiIntent;
    [SerializeField] private ControlMode controlMode;

    public bool IsRunning { get; private set; }

    public event System.Action<bool> OnSessionStateChanged;

    public async void StartSession() {
        if (IsRunning)
            return;

        IsRunning = true;
        OnSessionStateChanged?.Invoke(true);

        controlMode.CurrentMode = ControlSource.AI;

        Debug.Log("Sentinel session started");

        await RunSessionLoop();
    }

    public void StopSession() {
        if (!IsRunning)
            return;

        IsRunning = false;
        OnSessionStateChanged?.Invoke(false);

        CommonIntent.MoveDirection = Vector3.zero;
        CommonIntent.Actions = PlayerAction.None;

        apiClients.ClearActionResponse();

        controlMode.CurrentMode = ControlSource.Human;

        Debug.Log("Sentinel session stopped");
    }

    private async Awaitable RunSessionLoop() {
        while (IsRunning) {
  
            aiIntent.ResetActionCompletion();

            WorldObservationManager.SetWorldObservation();

            var observations =
                WorldObservationManager.GetWorldObservation();


            await apiClients.SendObservation(observations);

            if (!IsRunning)
                break;

            if (apiClients.GetActionResponse == null) {
                Debug.LogWarning("No ActionResponse received");
                break;
            }

            while (IsRunning && !aiIntent.ActionCompleted) {
                await Awaitable.NextFrameAsync();
            }

            Debug.Log(
                $"Action result: {aiIntent.Tracker.Status}, " +
                $"Reason: {aiIntent.Tracker.FailureReason}"
            );
        }
    }
}