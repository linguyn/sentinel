using UnityEngine; 
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

/*ApiClients: responsible for backend communication*/

public class ApiClients : MonoBehaviour {

    UnityWebRequestAsyncOperation asyncOP; 
    string webURL = "http://127.0.0.1:8000";

    async Task Start() {
        Observation observation = new Observation {
            has_key = true,
            door_locked = false
        };

        string json = JsonUtility.ToJson(observation); 

        var result = await asyncPostRequest(json);
        Debug.Log(result); 
    }

    private async Task<string> asyncPostRequest(string json) {
        UnityWebRequest request = UnityWebRequest.Post(webURL + "/action", json, "application/json");
        asyncOP = request.SendWebRequest();
        Awaitable awaitable = Awaitable.FromAsyncOperation(asyncOP);
        await awaitable; 

        if (request.result != UnityWebRequest.Result.Success) {
            Debug.Log(request.error);
            Debug.Log(request.responseCode);
            return asyncOP.webRequest.downloadHandler.text;
        } else {
            Debug.Log(request.responseCode);
            return asyncOP.webRequest.downloadHandler.text; 
        }
        
    }
}
