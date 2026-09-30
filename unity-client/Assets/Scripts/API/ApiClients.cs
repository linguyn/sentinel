using Unity.VisualScripting;
using UnityEngine; 
using UnityEngine.Networking;

/*ApiClients: responsible for backend communication*/

public class ApiClients : MonoBehaviour {

    string webURL = "http://127.0.0.1:8000";
    string jsonResponse;

    async Awaitable Start() {
        Observation observation = new Observation {
            has_key = true,
            door_locked = false
        };

        string json = JsonUtility.ToJson(observation);

        var response = await AsyncPostRequest(json);
        if (response == null) { return; }
        Debug.Log("Response: " + response);

        string objectData = GetActionResponseString(response);
        Debug.Log("Action: " + objectData);
    }


    public string GetActionResponseString(string json) {
        ActionResponse actionResponse = JsonUtility.FromJson<ActionResponse>(json);

        return actionResponse.action;
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
