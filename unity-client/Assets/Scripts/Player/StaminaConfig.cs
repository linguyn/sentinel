using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StaminaConfig",
                menuName = "Game/Player/StaminaConfig")]
public class StaminaConfig : ScriptableObject
{

    [Header("Stamina Thresholds")]
    [Min(1f)]
    [SerializeField] private float maximumStamina = 100f; 
    public float MaximumStamina => maximumStamina;

    [SerializeField] private float sprintRestartThreshold = 15f;
    public float SprintRestartThreshold => sprintRestartThreshold;

    private void OnValidate() //Called when the script is loaded or a value is changed in the inspector
    {
        sprintRestartThreshold = Mathf.Clamp(sprintRestartThreshold, 0f, maximumStamina);
    }


    [Header("Regeneration")]
    [Min(0f)]
    [SerializeField] private float regenerationDelay = 1.5f; // Time in seconds before stamina starts regenerating after last use
    public float RegenerationDelay => regenerationDelay;
    [Min(0f)]
    [SerializeField] private float regenerationPersec = 10f; // Amount of stamina regenerated per second
    public float RegenerationPersec => regenerationPersec;


    
    [Header("Drain Rates")]
    [Min(0f)]
    [SerializeField] private float sprintDrainPerSec = 18f;
    public float SprintDrainPerSec => sprintDrainPerSec;

    [Min(0f)]
    [SerializeField] private float heavyPushDrainPerSec = 8f;
    public float HeavyPushDrainPerSec => heavyPushDrainPerSec;

    [Min(0f)]
    [SerializeField] private float doorHoldDrainPerSec = 8f;
    public float DoorHoldDrainPerSec => doorHoldDrainPerSec;

}
