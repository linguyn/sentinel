using UnityEngine;
using TMPro;
using System;

public class InteractionUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI interactionText;
    [SerializeField]
    private GameObject promptRoot; 

    public void Show(string message) {
        interactionText.text = message;
        promptRoot.SetActive(true);
    }

    public void Hide() {
        promptRoot.SetActive(false);
    }
}
