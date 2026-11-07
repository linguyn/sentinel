using UnityEngine;

public enum ControlSource {
    Human = 0, 
    AI = 1 
}

public class ControlMode : MonoBehaviour {

    private ControlSource currentMode = ControlSource.Human;

    [SerializeField]
    private Behaviour[] humanInputComponents; 

    public ControlSource CurrentMode {
        get { return currentMode; }
        set { 
            currentMode = value;
            HandleModeSwitch(); 
        }
    }

    private void HandleModeSwitch() {
        bool humanInControl = currentMode == ControlSource.Human; 

        foreach (Behaviour b in humanInputComponents) {
            if (b != null) {
                b.enabled = humanInControl;
            }
        }
    }

    private void Awake() {
        HandleModeSwitch();
    }
}
