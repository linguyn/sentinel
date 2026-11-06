using UnityEngine;

public class MoveActionAdapter : MonoBehaviour, IActionAdapter {
    public string ActionName => "move";

    public void Begin(ActionResponse request, ActionTracker tracker) { }
    public void Tick(float deltaTime) { }
    public void Cancel() { }
}