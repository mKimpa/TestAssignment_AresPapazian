using PT = PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Settings References")]
    [SerializeField] private RectTransform settingsPanel;
    [SerializeField] private Image backgroundOverlay;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private PT.Ease openningEaseType = PT.Ease.OutBack;
    [SerializeField] private PT.Ease closingEaseType = PT.Ease.OutExpo;

    private PT.Tween panelScaleTween;
    private PT.Tween overlayTween;

    private void Awake()
    {
        Instance = this;
        
        // Initialization - hiding popup
        if (settingsPanel != null)
        {
            settingsPanel.gameObject.SetActive(false);
            settingsPanel.localScale = Vector3.zero;
        }

        // bg overlay setup
        if (backgroundOverlay != null)
        {
            backgroundOverlay.gameObject.SetActive(false);
            var color = backgroundOverlay.color;
            color.a = 0f;
            backgroundOverlay.color = color;
        }

        // Close button subscription
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseSettings);
        }
    }

    
    public void OpenSettings()
    {
        StopAllTweens();

        // Overlay activaton
        if (backgroundOverlay != null)
        {
            backgroundOverlay.gameObject.SetActive(true);
            overlayTween = PT.Tween.Alpha(backgroundOverlay, 0.7f, animationDuration, openningEaseType);
        }

        // Settings popup activation and animation
        if (settingsPanel != null)
        {
            settingsPanel.gameObject.SetActive(true);
            panelScaleTween = PT.Tween.Scale(settingsPanel, Vector3.one, animationDuration, openningEaseType);
        }
    }

    
    public void CloseSettings()
    {
        StopAllTweens();

        // Hiding settings popup
        if (settingsPanel != null)
        {
            panelScaleTween = PT.Tween.Scale(settingsPanel, Vector3.zero, animationDuration, closingEaseType)
                .OnComplete(() => settingsPanel.gameObject.SetActive(false));
        }

        // Hiding overlay
        if (backgroundOverlay != null)
        {
            overlayTween = PT.Tween.Alpha(backgroundOverlay, 0f, animationDuration, openningEaseType)
                .OnComplete(() => backgroundOverlay.gameObject.SetActive(false));
        }
    }

    // Closing on overlay click
    public void OnOverlayClick()
    {
        CloseSettings();
    }

    private void StopAllTweens()
    {
        panelScaleTween.Stop();
        overlayTween.Stop();
    }

    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseSettings);
        }
    }
}
