using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SentinelUIController : MonoBehaviour {
    [Header("Session")]
    [SerializeField] private SessionManager sessionManager;

    [Header("Play / Stop Button")]
    [SerializeField] private Button aiButton;
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite stopSprite;

    [Header("AI Avatar")]
    [SerializeField] private Image avatarImage;
    [SerializeField] private Button avatarButton;
    [SerializeField] private Sprite[] avatarFrames;
    [SerializeField] private float frameDelay = 0.15f;

    [Header("Debug Panels")]
    [SerializeField] private CanvasGroup debugMonitorGroup;
    [SerializeField] private CanvasGroup timerGraphGroup;

    private Coroutine avatarCoroutine;

    private void Awake() {
        aiButton.onClick.AddListener(ToggleAI);

        // Initial UI
        aiButton.image.sprite = playSprite;

        if (avatarFrames.Length > 0) {
            avatarImage.sprite = avatarFrames[0];
        }
    }

    private void OnEnable() {
        sessionManager.OnSessionStateChanged += HandleSessionStateChanged;

        if (avatarButton != null) {
            avatarButton.onClick.AddListener(ToggleDebugPanels);
        }
    }

    private void OnDisable() {
        sessionManager.OnSessionStateChanged -= HandleSessionStateChanged;

        if (avatarButton != null) {
            avatarButton.onClick.RemoveListener(ToggleDebugPanels);
        }
    }

    private void ToggleDebugPanels() {
        if (debugMonitorGroup == null || timerGraphGroup == null)
            return;

        bool visible = debugMonitorGroup.alpha <= 0f;
        SetPanelVisibility(debugMonitorGroup, visible);
        SetPanelVisibility(timerGraphGroup, visible);
    }

    private static void SetPanelVisibility(CanvasGroup panel, bool visible) {
        // Hide the visuals without stopping the graph's rolling sample history.
        panel.alpha = visible ? 1f : 0f;
        panel.interactable = visible;
        panel.blocksRaycasts = visible;
    }

    private void ToggleAI() {
        if (sessionManager.IsRunning) {
            sessionManager.StopSession();
        } else {
            sessionManager.StartSession();
        }
    }

    private void HandleSessionStateChanged(bool isRunning) {
        if (isRunning) {
            aiButton.image.sprite = stopSprite;

            if (avatarCoroutine == null && avatarFrames.Length > 0) {
                avatarCoroutine = StartCoroutine(AnimateAvatar());
            }
        } else {
            aiButton.image.sprite = playSprite;

            if (avatarCoroutine != null) {
                StopCoroutine(avatarCoroutine);
                avatarCoroutine = null;
            }

            if (avatarFrames.Length > 0) {
                avatarImage.sprite = avatarFrames[0];
            }
        }
    }

    private IEnumerator AnimateAvatar() {
        int currentFrame = 0;

        while (sessionManager.IsRunning) {
            avatarImage.sprite = avatarFrames[currentFrame];

            currentFrame++;

            if (currentFrame >= avatarFrames.Length) {
                currentFrame = 0;
            }

            yield return new WaitForSeconds(frameDelay);
        }

        avatarCoroutine = null;
    }
}
