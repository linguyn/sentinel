using UnityEngine;

public class AIIntent : MonoBehaviour {

    ApiClients apiClients;

    private Vector3 targetPosition;
    [SerializeField] private float stopDistance = 0.2f;
    [SerializeField] private Transform playerTransform;

    public bool ActionCompleted { get; private set; }

    private void HandleActionResponse() {
        ActionResponse response= apiClients.GetActionResponse;

        if (response == null) { return; }

        string action = response.action.ToLower();
        
        /*Move*/
        if (action == "move") {
            targetPosition.x = response.position.x;
            targetPosition.y = response.position.y;
            targetPosition.z = response.position.z;

            Vector3 direction = targetPosition - playerTransform.position;

            direction.y = 0f;

            if (direction.magnitude <= stopDistance) {
                CommonIntent.MoveDirection = Vector3.zero;
                ActionCompleted = true;
                apiClients.ClearActionResponse();
                return;
            }

            ActionCompleted = false;
            CommonIntent.MoveDirection = direction.normalized;
        }


    }

    private void Awake() {
        apiClients = GetComponent<ApiClients>();
    }

    private void Update() {
        HandleActionResponse(); 
    }
}