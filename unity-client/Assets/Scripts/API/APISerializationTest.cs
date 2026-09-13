using UnityEngine; 

public class APISerializationTest : MonoBehaviour {

    private void Start() {
        Observation observation = new Observation {
            has_key = true,
            door_locked = false
        };

        string json = JsonUtility.ToJson(observation);

        Debug.Log(json);
    }
}
