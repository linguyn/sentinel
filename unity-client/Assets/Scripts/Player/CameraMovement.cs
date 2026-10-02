using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Transform cameraSocket;
    [SerializeField] private Transform cameraAnchor;


    [Header("Player Input")]
    [SerializeField] private PlayerInput playerInput;
    private InputAction lookAction;
    private Vector2 lookInput;


    [Header("Camera Root Input")]
    [SerializeField] private Transform cameraRoot;
    private float yaw;


    [Header("Camera Pitch Input")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    private float pitch;


    [Header("Camera Collision")]
    [SerializeField] private float sphereCastRadius;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float collisionOffset;
    private Vector3 desiredSocketPosition;
    private Vector3 actualSocketPosition;
    private Vector3 sphereCastDirection;
    private Vector3 velocity;
    private RaycastHit hit;



    [Header("Smooth Rotation")]
    [SerializeField] private float smoothTime;
    [SerializeField] private float sensitivity;


    [Header("Shoulder Camera")]
    [SerializeField] private Vector3 shoulderOffset;

    [Header("Crouch Camera")]
    [SerializeField] private PlayerCrouch playerCrouch;
    [SerializeField] private Transform cameraHeightRoot;
    [SerializeField] private float crouchCameraOffset = 0.65f;
    [SerializeField] private float crouchTransitionSpeed = 3.5f;

    private Vector3 standingCameraPosition;


    //Functions
    private void HandleInput()
    {
        lookInput = lookAction.ReadValue<Vector2>();
        yaw += lookInput.x * sensitivity * Time.deltaTime;
        pitch -= lookInput.y * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }


    private void HandleRotation()
    {
        cameraRoot.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }


    private void HandleCameraCollision()
    {
        desiredSocketPosition = cameraAnchor.position;
        sphereCastDirection = (desiredSocketPosition - cameraPivot.position).normalized;
        float maxDistance = Vector3.Distance(cameraPivot.position, desiredSocketPosition);
        
        bool isHit = Physics.SphereCast(
            cameraPivot.position,
            sphereCastRadius,
            sphereCastDirection,
            out hit,
            maxDistance,
            collisionMask
        );

        if (isHit)
        {
            actualSocketPosition = cameraPivot.position + (hit.distance - sphereCastRadius - collisionOffset)*sphereCastDirection;
        }
        else
        {
            actualSocketPosition = desiredSocketPosition;
        }
    }


    private void HandleCameraMovement()
    {
        cameraSocket.position = Vector3.SmoothDamp(cameraSocket.position, actualSocketPosition, ref velocity, smoothTime);
    }


    private void HandleShoulderOffset()
    {
        cameraAnchor.localPosition = shoulderOffset;
    }

    private void HandleCrouchCamera() {
        Vector3 targetPosition = standingCameraPosition; 

        if (playerCrouch.IsCrouching) {
            targetPosition.y -= crouchCameraOffset;
        }

        cameraHeightRoot.localPosition = Vector3.MoveTowards(
            cameraHeightRoot.localPosition, 
            targetPosition, 
            crouchTransitionSpeed * Time.deltaTime);
    }

    //Cycle
    private void Awake()
    {
        lookAction = playerInput.actions["Look"];

        standingCameraPosition = cameraHeightRoot.localPosition;
    }


    private void Update()
    {
        HandleInput();
    }


    private void LateUpdate()
    {
        HandleRotation();
        HandleShoulderOffset();
        HandleCameraCollision();
        HandleCameraMovement();
        HandleCrouchCamera();
    }
}
