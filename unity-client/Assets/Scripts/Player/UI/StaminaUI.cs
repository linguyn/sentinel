using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("Stamina UI elements")]
    [SerializeField] private CanvasGroup staminaCanvasGroup;
    [SerializeField] private Image staminaProgressUI; 

    [SerializeField] private StaminaConfig staminaConfig;
    [SerializeField] private PlayerVitals playerStamina;

    private void UpdateStamina(int value)
    {

        if (value == 1)
        {
            staminaCanvasGroup.alpha = 1f; // Show the stamina UI
        } else
        {
            staminaCanvasGroup.alpha = 0f; // Hide the stamina UI
        }

        staminaProgressUI.fillAmount = playerStamina.CurrentStamina / staminaConfig.MaximumStamina;
    }

    
    void Update()
    {
        if (staminaProgressUI.fillAmount != 1) {
            UpdateStamina(1);
        } else {
            UpdateStamina(0);
        }
    }
}
