using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Speed variables
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float crouchSpeed = 1.5f;

    private float currentSpeed;
    private float targetSpeed;

    // Speed change rate variables
    [SerializeField] private float acceleration = 7f;
    [SerializeField] private float deceleration = 10f;

    // Movement variables
    [SerializeField] private bool canMove = true;
    private InputAction moveAction;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private CharacterController charController;
    private Vector3 moveVector;
    private Vector3 cameraForward;
    private Vector3 cameraRight;

    // Rotation variables 
    [SerializeField] private float rotationSpeed = 5f;
    private Quaternion targetRotation;

    // Sprint variables
    private InputAction sprintAction;
    private bool sprintRequested;
    private bool isSprinting;

    //Jump variables
    private InputAction jumpAction;
    [SerializeField] private float jumpHeight = 2f;

    // Crouch variables
    private PlayerCrouch playerCrouch;

    //Gravity variables
    private float verticalVelocity = 0f;
    [SerializeField] private float gravity = -9.81f;

    //Camera
    [SerializeField] private Transform cameraTransform;

    //Animation variables
    private Animator animator;

    //Stamina variables
    private PlayerVitals playerStamina;

    private void HandleGravity() //Function to handle gravity
    {
        if (charController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f; //Keep the player grounded
        }
        verticalVelocity += gravity * Time.deltaTime;
    }
    private void HandleMovement() //Function to handle player movement
    {
        moveInput = moveAction.ReadValue<Vector2>();
        cameraForward = new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z).normalized;
        cameraRight = new Vector3(cameraTransform.right.x, 0f, cameraTransform.right.z).normalized;
        moveDirection = (moveInput.x * cameraRight + moveInput.y * cameraForward).normalized;
    }

    private void HandlePlayer() //Function to move the player
    {
        moveVector = new Vector3(moveDirection.x * currentSpeed, verticalVelocity, moveDirection.z * currentSpeed); 
        charController.Move(moveVector * Time.deltaTime);
    }

    private void HandleRotation() //Function to handle player rotation
    {
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

    }

    private void HandleSprint() //Function to handle sprinting
    {
        bool isMoving = moveInput != Vector2.zero;

        sprintRequested = sprintAction.IsPressed();

        isSprinting = sprintRequested && !playerStamina.IsExhausted && isMoving && !playerCrouch.IsCrouching;

        if (isSprinting)
        {

            isSprinting = playerStamina.TrySpend(playerStamina.SprintDrainPerSec * Time.deltaTime);
        }

        if (playerCrouch.IsCrouching) {
            targetSpeed = crouchSpeed; 
        } else {
            targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
        }
        
        float speedChangeRate = currentSpeed < targetSpeed ? acceleration : deceleration;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed, 
            targetSpeed, 
            speedChangeRate * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (charController.isGrounded && jumpAction.WasPressedThisFrame()) {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleAnimation() //Function to handle player animation (Idle, Walking, Sprinting)
    {
        if (isSprinting) {
            animator.SetFloat("Speed", 1f, 0.15f, Time.deltaTime); // Sprinting animation
        } else if (moveDirection.sqrMagnitude > 0.001f) {
            animator.SetFloat("Speed", 0.5f, 0.15f, Time.deltaTime); // Walking animation
        } else {
            animator.SetFloat("Speed", 0f, 0.15f, Time.deltaTime); // Idle animation
        }
    }
    
    public void SetMovementEnabled(bool value) //Function to enable/disable player movement
    {
        canMove = value;
        charController.enabled = value;
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        charController = GetComponent<CharacterController>();
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];
        animator = GetComponentInChildren<Animator>();
        playerStamina = GetComponent<PlayerVitals>();
        playerCrouch = GetComponent<PlayerCrouch>();
    }
    private void OnEnable()
    {
        moveAction.Enable();
        sprintAction.Enable();
        jumpAction.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
        sprintAction.Disable();
        jumpAction.Disable();
    }
    private void Update()
    {
        if (canMove) {
            HandleMovement();
            HandleJump();
            HandleGravity();
            HandleSprint();
            HandlePlayer();
            HandleRotation();
            HandleAnimation();
        }
    }
}
