using System.Collections;
using UnityEngine;

public enum DoorState {
    Closed, 
    Open,    
}


public class AnimatedDoor : MonoBehaviour, IInteractable {
    [Header("Player Pivot")]
    [SerializeField] private Transform playerPivot;

    [Header("Door Settings")]
    [Tooltip("Adjust door opening speed")]
    [SerializeField, Min(0.1f)] private float doorOpenSpeed = 2.5f;
    [Tooltip("The starting state of the door, only change when the game starts")]
    [SerializeField] private bool startLocked;
    [Tooltip("The initial lock state of the door")]
    [SerializeField] private DoorState initialDoorState;
    private bool isLocked;

    [Header("Handle Settings")]
    [Tooltip("Adjust door handle rotation")]
    [SerializeField] private Transform handlePivot;
    [SerializeField] private float handleRotationAngle = 30f;
    [SerializeField, Min(0.1f)] private float handleOpenSpeed = 20f;

    private DoorState currentDoorState;

    private Quaternion openRotation;
    private Quaternion closedRotation;
    private Quaternion handleOpenRotation;
    private Quaternion handleCloseRotation;

    private Vector3 forward; 
    private Coroutine doorCoroutine;
    private Coroutine handleCoroutine; 


    public string GetPrompt() {
        if (isLocked) { return "Locked"; }
        return currentDoorState == DoorState.Closed ? "Open" : "Close";
    }

    public void Interact() {

        if (!CanInteract()) { return; }

        Vector3 doorToPlayer = (playerPivot.position - transform.position).normalized;
        float dot = Vector3.Dot(forward, doorToPlayer);
        if (dot > 0) {
            openRotation = closedRotation * Quaternion.Euler(new Vector3(0f, 0f, -90f));
        } 
        else if (dot < 0) {
            openRotation = closedRotation * Quaternion.Euler(new Vector3(0f, 0f, 90f));
        }

        if (CanInteract()) {
            if (doorCoroutine != null) {
                StopCoroutine(doorCoroutine); 
            }
            doorCoroutine = StartCoroutine(ToggleDoor());
        }
    }

    public bool CanInteract() {
        return !isLocked;
    }

    /*Set the locked state of the door*/
    public void SetLocked(bool locked) {
        if (currentDoorState == DoorState.Open) {
            return;
        }
        isLocked = locked;
    }

    /*Reset the door to its initial state*/
    public void ResetDoorState() {
        if (doorCoroutine != null) {
            StopCoroutine(doorCoroutine);
            doorCoroutine = null;
        } 

        if (handleCoroutine != null) {
            StopCoroutine(handleCoroutine);
            handleCoroutine = null;
        }

        isLocked = startLocked;
        currentDoorState = initialDoorState; 
        transform.localRotation = initialDoorState == DoorState.Open && !isLocked ? openRotation : closedRotation;
        handlePivot.localRotation = handleCloseRotation;
    }

    /*Door Opening/Closing Coroutine*/
    private IEnumerator ToggleDoor() {
        bool shouldOpen = currentDoorState == DoorState.Closed;

        currentDoorState =
            shouldOpen ? DoorState.Open : DoorState.Closed;

        Quaternion targetRotation =
            shouldOpen ? openRotation : closedRotation;

        if (shouldOpen) {
            if (handleCoroutine != null) {
                StopCoroutine(handleCoroutine);
            }

            handleCoroutine = StartCoroutine(AnimateHandle());
        }

        while (Quaternion.Angle(
                   transform.localRotation,
                   targetRotation) > 0.01f) {
            // Animate the door rotation
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                targetRotation,
                doorOpenSpeed * Time.deltaTime
            );

            yield return null;
        }
    
        transform.localRotation = targetRotation;
        doorCoroutine = null;
    }

    /*Door Handle Animation Coroutine*/
    private IEnumerator AnimateHandle() {

        while (Quaternion.Angle(
            handlePivot.localRotation, 
            handleOpenRotation) > 0.01f) {
            handlePivot.localRotation = Quaternion.Lerp(
                handlePivot.localRotation,
                handleOpenRotation,
                handleOpenSpeed * Time.deltaTime
            );

            yield return null;
        }

        handlePivot.localRotation = handleOpenRotation;

        while (Quaternion.Angle(
                handlePivot.localRotation, 
                handleCloseRotation) > 0.01f) {
            handlePivot.localRotation = Quaternion.Lerp(
                handlePivot.localRotation,
                handleCloseRotation,
                handleOpenSpeed * Time.deltaTime
            );
            yield return null;
        }

        handlePivot.localRotation = handleCloseRotation;
        handleCoroutine = null;
    }

    private void Awake() {
        forward = transform.up;
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(new Vector3(0f, -90f, 0f));

        handleCloseRotation = handlePivot.localRotation;
        handleOpenRotation = handleCloseRotation * Quaternion.Euler(handleRotationAngle, 0f, 0f);

        ResetDoorState();
    }
} 
