using PrimeTween;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sequence = PrimeTween.Sequence;

public class BottomButton : MonoBehaviour
{

    private Button button;
    private RectTransform rectTransform;
    private bool isSelected = false;


    private Tween iconRizeTween;
    private Tween highlightOffsetTween;
    private Tween highlightOpacityTween;
    private Tween buttonSizeTween;
    private Tween labelOpacityTween;

    [Header("References")]
    [SerializeField] private Image icon;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Image highlight;
    [SerializeField] private TextMeshProUGUI label;

    [Header("Button Settings")]
    [SerializeField] private bool isEnabled;
    [SerializeField] private int ButtonIndex;

    [Header("Selection Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutBack;

    [Header("Shake Animation Settings")]
    [SerializeField] private float shakeStrength = 15f;
    [SerializeField] private float shakeDuration = 0.6f;
    [SerializeField] private int shakeVibrato = 3;
    [SerializeField] private float shakeElasticity = 0.5f;
    [SerializeField] private float scalePunch = 0.3f;
    [SerializeField] private float scaleDuration = 0.4f;
    [SerializeField] private Color shakeColor = Color.blue;
    [SerializeField] private float colorFlashDuration = 0.2f;

    [Header("Target Values")]
    [SerializeField] private float selectedWidth = 700f;
    [SerializeField] private float highlightRise = 200f;
    [SerializeField] private float iconRise = 150f;
    [SerializeField] private float defaultHighlightWidth = 300f;

    private float startButtonWidth;
    private float startIconYPos;
    private Vector2 startIconAnchoredPos;

    private Sequence lockShakeSequence;
    private Vector3 originalLockPosition;
    private Vector3 originalLockScale;
    private Color originalLockColor;


    private void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();

        startButtonWidth = rectTransform.sizeDelta.x;
        startIconYPos = icon.rectTransform.anchoredPosition.y;

        originalLockPosition = lockIcon.transform.localPosition;
        originalLockScale = lockIcon.transform.localScale;
        originalLockColor = lockIcon.color;

        icon.gameObject.SetActive(isEnabled);
        lockIcon.gameObject.SetActive(!isEnabled);       

        if (highlight != null)
        {
            highlight.gameObject.SetActive(false);
        }
        if (label != null)
        {
            label.gameObject.SetActive(false);
        }

    }

    private void OnEnable()
    {
        button.onClick.AddListener(button_OnClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(button_OnClick);
    }

    private void button_OnClick()
    {
        BottomBarView.instance.ButtonClick(ButtonIndex, isEnabled);
    }

    public void SelectButton(bool isSelected)
    {
        if ( isEnabled)
        {
            this.isSelected = isSelected;
            AnimateButton(isSelected);
        }
        else
        {
            AnimateLockShake();
        }
        
    }

    private void AnimateLockShake()
    {
        if (lockShakeSequence.isAlive)
            lockShakeSequence.Stop();

        lockShakeSequence = Sequence.Create();
        lockShakeSequence.Chain(Tween.Scale(lockIcon.transform, originalLockScale,
            originalLockScale * (1f + scalePunch), scaleDuration * 0.3f, Ease.OutBack));

        lockShakeSequence.Group(
            // Горизонтальная встряска
            Tween.LocalPositionX(lockIcon.transform, originalLockPosition.x,
                originalLockPosition.x + shakeStrength, shakeDuration * 0.25f, Ease.OutSine)
        );

        lockShakeSequence.Group(
            // Изменение цвета на красный
            Tween.Color(lockIcon, originalLockColor, shakeColor, colorFlashDuration * 0.5f)
        );

        // 3. Обратная встряска
        lockShakeSequence.Group(
            Tween.LocalPositionX(lockIcon.transform, originalLockPosition.x + shakeStrength,
                originalLockPosition.x - shakeStrength * 0.7f, shakeDuration * 0.25f, Ease.InOutSine)
        );

        // 4. Возврат к исходной позиции
        lockShakeSequence.Group(
            Tween.LocalPositionX(lockIcon.transform, originalLockPosition.x - shakeStrength * 0.7f,
                originalLockPosition.x, shakeDuration * 0.25f, Ease.InSine)
        );

        lockShakeSequence.Group(
            // Возврат к оригинальному цвету
            Tween.Color(lockIcon, shakeColor, originalLockColor, colorFlashDuration * 0.5f)
        );

        // 5. Возврат масштаба с упругим эффектом
        lockShakeSequence.Group(
            Tween.Scale(lockIcon.transform, originalLockScale * (1f + scalePunch),
                originalLockScale, scaleDuration * 0.7f, Ease.OutElastic)
        );

        // Дополнительные эффекты
        lockShakeSequence
            .OnComplete(() => {
                // Гарантируем возврат к исходным значениям
                ResetLockTransform();
            });
    }

    private void ResetLockTransform()
    {
        lockIcon.transform.localPosition = originalLockPosition;
        lockIcon.transform.localScale = originalLockScale;
        lockIcon.color = originalLockColor;
    }

    private void AnimateButton(bool Select)
    {
        StopAllTweens();

        // Icon Rizing animation 
        float IcontargetYPos = Select ? startIconYPos + iconRise : startIconYPos;
        iconRizeTween = Tween.UIAnchoredPositionY(
            icon.rectTransform,
            IcontargetYPos,
            animationDuration,
            easeType
            );

        // Button width animation
        float targetButtonWidth = Select ? selectedWidth : startButtonWidth;
        buttonSizeTween = Tween.UISizeDelta(
          rectTransform,
          new Vector2(targetButtonWidth, rectTransform.sizeDelta.y),
          animationDuration,
          easeType
      );

        // Highlight offset animation
        float HighlighttargetYPos = Select ? highlightRise : 0;
        iconRizeTween = Tween.UISizeDelta(
            highlight.rectTransform,
            new Vector2(0, HighlighttargetYPos),
            animationDuration,
            easeType
            );

        if (Select)
        {
            // Highlight bg image appearing animation
            var highlightColor = highlight.color;
            highlightColor.a = 0f;
            highlight.color = highlightColor;

            highlight.gameObject.SetActive(true);

            highlightOpacityTween = Tween.Alpha(
                highlight,
                1f,
                animationDuration,
                easeType
            );

            // Button name appearing animation
            var textColor = label.color;
            textColor.a = 0f;
            label.color = textColor;

            label.gameObject.SetActive(true);

            labelOpacityTween = Tween.Alpha(
                label,
                1f,
                animationDuration,
                easeType
            );
        } else
        {
            // Highlight bg and Button name dissapearing animation
            highlightOpacityTween = Tween.Alpha(highlight, 0f, animationDuration * 0.5f)
               .OnComplete(() => highlight.gameObject.SetActive(false));

            labelOpacityTween = Tween.Alpha(label, 0f, animationDuration * 0.5f)
                 .OnComplete(() => label.gameObject.SetActive(false));
        }
        
    }

    private void StopAllTweens()
    {
        iconRizeTween.Stop();
        highlightOffsetTween.Stop();
        buttonSizeTween.Stop();
        highlightOpacityTween.Stop();
        labelOpacityTween.Stop();
    }

}
