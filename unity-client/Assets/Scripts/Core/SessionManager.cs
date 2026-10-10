using UnityEngine; 

public class SessionManager : MonoBehaviour {
    ControlMode controlMode;


    private void Awake() {
        controlMode = GetComponent<ControlMode>();
    }

    private void Update() {
        controlMode.CurrentMode = ControlSource.AI; 
    }
}