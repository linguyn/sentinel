using UnityEngine;

public enum ControlSource {
    Human = 0, 
    AI = 1 
}

public class ControlMode : MonoBehaviour {

    private ControlSource currentMode = ControlSource.Human;

    [SerializeField]
    private HumanIntent humanIntent; 
    //[SerializeField]
    //private AIIntent aiIntent;

    public ControlSource CurrentMode {
        get { return currentMode; }
        set { 
            currentMode = value;
            HandleModeSwitch(); 
        }
    }

    private void HandleModeSwitch() {
        if (currentMode == ControlSource.AI) {
            humanIntent.enabled = false;  
        } else {
            humanIntent.enabled = true; 
        }
    }

    private void Awake() {
        humanIntent = GetComponent<HumanIntent>();
        HandleModeSwitch();
    }
}
