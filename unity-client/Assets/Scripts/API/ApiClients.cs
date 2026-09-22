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
        jsonResponse = await AsyncPostRequest(json);
        Debug.Log(jsonResponse); 
    }

    public string GetActionResponse()
    {
        ActionResponse jsonToObj = JsonUtility.FromJson<ActionResponse>(jsonResponse); 
        return jsonToObj.action; 
    }
    

    private async Awaitable<string> AsyncPostRequest(string json) {
        using(UnityWebRequest request = UnityWebRequest.Post(webURL + "/action", json, "application/json"))
        {
            UnityWebRequestAsyncOperation asyncOP = request.SendWebRequest();
            Awaitable awaitable = Awaitable.FromAsyncOperation(asyncOP);
            await awaitable; 

            if (request.result != UnityWebRequest.Result.Success) {
                Debug.Log(request.error);
                Debug.Log(request.responseCode);
            } else {
                Debug.Log(request.responseCode); 
            }

            return request.downloadHandler.text;
        }
    }

}
