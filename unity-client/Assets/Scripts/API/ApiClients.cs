using System.Collections.Generic;
using System.Text.Json.Serialization;
using Unity.VisualScripting;
using UnityEngine; 
using UnityEngine.Networking;
using Newtonsoft.Json;

/*ApiClients: responsible for backend communication*/

public class ApiClients : MonoBehaviour {

    string webURL = "http://127.0.0.1:8000";
    private ActionResponse actionResponse; 

    async Awaitable Start() {

        /*Get the World Observation and send WO to backend*/
        WorldObservationManager.SetWorldObservation();

        var observations = WorldObservationManager.GetWorldObservation();

        string json = JsonConvert.SerializeObject(observations);

        string jsonResponse = await AsyncPostRequest(json);
        if (jsonResponse == null) { return; }

        actionResponse = JsonUtility.FromJson<ActionResponse>(jsonResponse);

        Debug.Log("Action: " + actionResponse.action);
        Debug.Log("Position: x: " + actionResponse.position.x 
                        + ", y: " + actionResponse.position.y + ", z: " + actionResponse.position.z);    
    }


    public ActionResponse GetActionResponse {
        get { return actionResponse; }
    }

    public void ClearActionResponse() {
        actionResponse = null;
    }

    private async Awaitable<string> AsyncPostRequest(string json) {
        using(UnityWebRequest request = UnityWebRequest.Post(webURL + "/action", json, "application/json"))
        {
            UnityWebRequestAsyncOperation asyncOP = request.SendWebRequest();
            Awaitable awaitable = Awaitable.FromAsyncOperation(asyncOP);
            await awaitable; 

            if (request.result != UnityWebRequest.Result.Success) {
                Debug.Log("Code: " + request.responseCode + "\nError: " + request.error);
            } else {
                Debug.Log("Code: " + request.responseCode);
                return request.downloadHandler.text;
            }

            return null;
        }
    }

}
