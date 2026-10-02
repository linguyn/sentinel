using UnityEngine;
using UnityEngine.InputSystem;

public class HumanIntent : MonoBehaviour {

    
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    [SerializeField] private Transform cameraTransform;

    private Vector3 moveDirection;
    private Vector2 moveInput;

    private PlayerInput playerInput; 
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction crouchAction;


    private void InputToDirection() {
        moveInput = moveAction.ReadValue<Vector2>();
        cameraForward = new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z).normalized;
        cameraRight = new Vector3(cameraTransform.right.x, 0f, cameraTransform.right.z).normalized;
        CommonIntent.MoveDirection = (moveInput.x * cameraRight + moveInput.y * cameraForward).normalized;
    }
    
    private void ActionRequest() {

        /*Continuos actions*/
        if (sprintAction.IsPressed()) {
            CommonIntent.HumanAction |= PlayerAction.Sprint;
        }

        /*One-shot actions*/
        if (jumpAction.WasPressedThisFrame()) {
            CommonIntent.HumanAction |= PlayerAction.Jump; 
        }

        if (crouchAction.IsPressed()) {
            CommonIntent.HumanAction |= PlayerAction.Crouch;
        } else { 
            CommonIntent.HumanAction &= ~PlayerAction.Crouch; 
        }
    }


    private void Start() {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];
        crouchAction = playerInput.actions["Crouch"];
    }

    private void OnEnable() {
        moveAction?.Enable();
        sprintAction?.Enable();
        jumpAction?.Enable();
        crouchAction?.Enable();
    }

    private void OnDisable() {
        moveAction?.Disable();
        sprintAction?.Disable();
        jumpAction?.Disable();
        crouchAction.Disable();

        CommonIntent.MoveDirection = Vector3.zero;
        CommonIntent.HumanAction = PlayerAction.None;
    }


    private void Update() {
        InputToDirection();
        ActionRequest();
    }
}
