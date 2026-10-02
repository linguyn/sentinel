using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouch : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction crouchAction;
    private CharacterController characterController;

    [Header("Crouch Configuration")]
    [SerializeField] private float crouchHeight = 0.91f;
    [SerializeField] private float crouchTransitionSpeed = 4f;

    //Standing config of the character controller
    [SerializeField] private float standingHeight;
    private Vector3 standingCenter;
    private float standingBottom;

    //Crouch Runtime
    public bool CrouchRequested { get; private set; }
    public bool IsCrouching { get; private set; }

    private void UpdateCrouchState() {

        CrouchRequested = crouchAction.IsPressed();

        IsCrouching = CrouchRequested ? true : false; 

    }

    private void UpdateControllerHeight() {

        // Determine the target height based on whether the player is crouching or standing
        float targetHeight =
            IsCrouching
                ? crouchHeight
                : standingHeight;

        float newHeight = Mathf.MoveTowards(
            characterController.height,
            targetHeight,
            crouchTransitionSpeed * Time.deltaTime
        );

        characterController.height = newHeight;

        // Adjust the center of the character controller to keep the bottom of the capsule at the same position
        Vector3 newCenter = standingCenter;

        newCenter.y =
            standingBottom +
            newHeight / 2f;

        characterController.center = newCenter;
    }

    private void OnEnable() {
        crouchAction.Enable(); 
    }

    private void OnDisable() {
        crouchAction.Disable();
    }

    void Awake() {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();

        crouchAction = playerInput.actions["Crouch"];

        standingHeight = characterController.height;
        standingCenter = characterController.center;
        standingBottom = standingCenter.y - (standingHeight / 2f);
    }

    void Update()
    {
        UpdateCrouchState();
        UpdateControllerHeight();
    }
}
