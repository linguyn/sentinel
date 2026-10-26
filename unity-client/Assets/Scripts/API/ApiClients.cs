using System.Collections.Generic;
using System.Text.Json.Serialization;
using Unity.VisualScripting;
using UnityEngine; 
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

/*ApiClients: responsible for backend communication*/

public class ApiClients : MonoBehaviour {

    string webURL = "http://127.0.0.1:8000";
    private ActionResponse actionResponse; 

    public async Awaitable SendObservation(List<EntityObservation> observations, List<ActionHistory> history) {

        var payload = new {
            observations,
            history
        };

        string json = JsonConvert.SerializeObject(payload, new StringEnumConverter(new CamelCaseNamingStrategy()));


        string jsonResponse = await AsyncPostRequest(json);
        if (jsonResponse == null) { return; }

        actionResponse = JsonUtility.FromJson<ActionResponse>(jsonResponse);
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
