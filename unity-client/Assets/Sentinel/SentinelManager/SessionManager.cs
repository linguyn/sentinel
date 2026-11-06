using UnityEngine;
using System.Collections.Generic;

public class SessionManager : MonoBehaviour {
    [SerializeField] private ApiClients apiClients;
    [SerializeField] private AIIntent aiIntent;
    [SerializeField] private ControlMode controlMode;

    public bool IsRunning { get; private set; }

    public event System.Action<bool> OnSessionStateChanged;

    private readonly List<ActionHistory> actionHistoryList = new(); 


    public async void StartSession() {
        if (IsRunning)
            return;

        actionHistoryList.Clear();
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


            await apiClients.SendObservation(observations, actionHistoryList);

            if (!IsRunning)
                break;

            if (apiClients.GetActionResponse == null) {
                Debug.LogWarning("No ActionResponse received");
                break;
            }

            while (IsRunning && !aiIntent.ActionCompleted) {
                await Awaitable.NextFrameAsync();
            }

            if (!IsRunning)
                break;

            RecordActionHistory();
        }
    }

    private void RecordActionHistory() {
        ActionTracker tracker = aiIntent.Tracker;

        if (!tracker.IsFinished) { return; }

        ActionHistory history = new ActionHistory {
            action = tracker.ActionName,
            target_id = tracker.TargetId,
            action_status = tracker.Status,
            reason = tracker.FailureReason
        };

        actionHistoryList.Add(history);
        Debug.Log(
           $"History recorded: {history.action}, " +
           $"Target: {history.target_id}, " +
           $"Result: {history.action_status}, " +
           $"Reason: {history.reason}"
        );
    }
}