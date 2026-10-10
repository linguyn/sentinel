using UnityEngine;

public class WorldObservationTest : MonoBehaviour {
    private void Start() {

        WorldObservationManager.SetWorldObservation();

        var observations = WorldObservationManager.GetWorldObservation();

        Debug.Log("Observed entities: " + observations.Count);

        foreach (EntityObservation observation in observations) {
            Debug.Log(JsonUtility.ToJson(observation, true));
        }
    }
}