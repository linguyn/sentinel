using UnityEngine;


public class PlayerCrouch : MonoBehaviour
{
    private CharacterController characterController;

    [Header("Crouch Configuration")]
    [SerializeField] private float crouchHeight = 0.91f;
    [SerializeField] private float crouchTransitionSpeed = 4f;

    //Standing config of the character controller
    private float standingHeight;
    private Vector3 standingCenter;
    private float standingBottom;

    //Crouch Runtime
    public bool CrouchRequested { get; private set; }
    public bool IsCrouching { get; private set; }

    private void UpdateCrouchState() {

        CrouchRequested = CommonIntent.HumanAction.HasFlag(PlayerAction.Crouch);

        IsCrouching = CrouchRequested; 

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


    void Awake() {
        characterController = GetComponent<CharacterController>();

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
