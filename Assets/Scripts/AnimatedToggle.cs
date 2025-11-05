using PT = PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnimatedToggle : MonoBehaviour, IPointerClickHandler
{
    [Header("Toggle References")]
    [SerializeField] private RectTransform toggleBackground;
    [SerializeField] private RectTransform toggleHandle;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image handleImage;

    [Header("Colors")]
    [SerializeField] private Color backgroundOnColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color backgroundOffColor = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] private Color handleOnColor = Color.white;
    [SerializeField] private Color handleOffColor = Color.white;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private PT.Ease easeType = PT.Ease.OutCubic;

    [Header("Toggle Settings")]
    [SerializeField] private bool isOn = false;
    [SerializeField] private bool interactable = true;

    // Handles positions
    private Vector2 handleOnPosition;
    private Vector2 handleOffPosition;
    private float backgroundWidth;

    
    private PT.Tween handlePositionTween;
    private PT.Tween handleColorTween;
    private PT.Tween backgroundColorTween;

    
    public System.Action<bool> OnValueChanged;

    private void Awake()
    {
        InitializeToggle();

        //Initial state without animation
        UpdateVisualState(false); 
    }

    private void InitializeToggle()
    {
        if (toggleBackground != null && toggleHandle != null)
        {
            // toggle positions calculation
            backgroundWidth = toggleBackground.rect.width;
            float handleOffset = (toggleBackground.rect.width - toggleHandle.rect.width) * 0.5f;
            handleOnPosition = new Vector2(handleOffset, 6f);
            handleOffPosition = new Vector2(-handleOffset, 6f);
        }
    }

    public void SetValue(bool value, bool animate = true)
    {
        if (isOn != value)
        {
            isOn = value;
            UpdateVisualState(animate);
            OnValueChanged?.Invoke(isOn);
        }
    }

    public void Toggle(bool animate = true)
    {
        SetValue(!isOn, animate);
    }

    private void UpdateVisualState(bool animate)
    {
        StopAllTweens();

        if (animate)
        {
            // Handle animation
            if (toggleHandle != null)
            {
                handlePositionTween = PT.Tween.UIAnchoredPosition(
                    toggleHandle,
                    isOn ? handleOnPosition : handleOffPosition,
                    animationDuration,
                    easeType
                );
            }

            // Handle color animation
            if (handleImage != null)
            {
                handleColorTween = PT.Tween.Color(
                    handleImage,
                    isOn ? handleOnColor : handleOffColor,
                    animationDuration,
                    easeType
                );
            }

            // Background color animation
            if (backgroundImage != null)
            {
                backgroundColorTween = PT.Tween.Color(
                    backgroundImage,
                    isOn ? backgroundOnColor : backgroundOffColor,
                    animationDuration,
                    easeType
                );
            }
        }
        else
        {
            // setting state without animation
            if (toggleHandle != null)
            {
                toggleHandle.anchoredPosition = isOn ? handleOnPosition : handleOffPosition;
            }

            if (handleImage != null)
            {
                handleImage.color = isOn ? handleOnColor : handleOffColor;
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = isOn ? backgroundOnColor : backgroundOffColor;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (interactable)
        {
            Toggle();
        }
    }

    public void SetInteractable(bool value)
    {
        interactable = value;

        // Changing opacity for non interactible 
        if (backgroundImage != null)
        {
            var color = backgroundImage.color;
            color.a = value ? 1f : 0.5f;
            backgroundImage.color = color;
        }
    }

    public bool IsOn => isOn;
    public bool IsInteractable => interactable;

    private void StopAllTweens()
    {
        handlePositionTween.Stop();
        handleColorTween.Stop();
        backgroundColorTween.Stop();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateVisualState(false);
        }
    }
#endif
}