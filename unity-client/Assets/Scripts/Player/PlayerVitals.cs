using UnityEngine;
using UnityEngine.UI;
public class PlayerVitals : MonoBehaviour {

    // Current stamina value
    [SerializeField] private float currentStamina;
    public float CurrentStamina => currentStamina;

    // Configuration for stamina behavior
    [SerializeField] private StaminaConfig config;

    // Exhaustion state
    [SerializeField] private bool isExhausted;
    public bool IsExhausted => isExhausted;

    //Drain rates getters
    public float SprintDrainPerSec => config.SprintDrainPerSec;
    public float HeavyPushDrainPerSec => config.HeavyPushDrainPerSec;
    public float DoorHoldDrainPerSec => config.DoorHoldDrainPerSec;

    //Time since last stamina use
    private float timeSinceLastStaminaUse = 0f;

    private void Awake() {
        currentStamina = config.MaximumStamina;
    }

    public bool TrySpend(float staminaCost) {

        isExhausted = currentStamina <= 0 ? true : false;

        if (staminaCost <= 0 || isExhausted) { return false; }

        currentStamina = Mathf.Max(0f, currentStamina - staminaCost);

        timeSinceLastStaminaUse = 0f;

        if (isExhausted) { return false; }

        return true;
    }

    private void TryRegenerate() {
        if (timeSinceLastStaminaUse < config.RegenerationDelay) {
            return;
        }

        if (currentStamina >= config.MaximumStamina) {
            return;
        }

        currentStamina += config.RegenerationPersec * Time.deltaTime;

        currentStamina = Mathf.Min(currentStamina, config.MaximumStamina);

        if (isExhausted && currentStamina >= config.SprintRestartThreshold) {
            isExhausted = false;
        }
    }


    private void Update() {
        timeSinceLastStaminaUse += Time.deltaTime;
        TryRegenerate();
    }

}














