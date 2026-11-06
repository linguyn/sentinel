using UnityEngine;

public class InteractActionAdapter : MonoBehaviour, IActionAdapter {
    public string ActionName => "interact";

    public void Begin(ActionResponse request, ActionTracker tracker) { }
    public void Tick(float deltaTime) { }
    public void Cancel() { }

    private void OnEnable() {
        ActionRegistry.Register(this);
    }

    private void OnDisable() {
        ActionRegistry.Unregister(this);
    }
}