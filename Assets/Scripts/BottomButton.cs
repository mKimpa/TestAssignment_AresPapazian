using PrimeTween;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    [SerializeField] private Image highlight;
    [SerializeField] private TextMeshProUGUI label;

    [Header("Button Settings")]
    [SerializeField] private int ButtonIndex;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutBack;

    [Header("Target Values")]
    [SerializeField] private float selectedWidth = 700f;
    [SerializeField] private float highlightRise = 200f;
    [SerializeField] private float iconRise = 150f;
    [SerializeField] private float defaultHighlightWidth = 300f;

    private float startButtonWidth;
    private float startIconYPos;
    private Vector2 startIconAnchoredPos;


    private void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();

        startButtonWidth = rectTransform.sizeDelta.x;
        startIconYPos = icon.rectTransform.anchoredPosition.y;

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
        BottomBarView.instance.ButtonClick(ButtonIndex);
    }

    public void SelectButton(bool isSelected)
    {
        this.isSelected = isSelected;
        AnimateButton(isSelected);        
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

    public bool IsAnimating()
    {
        return iconRizeTween.isAlive || highlightOffsetTween.isAlive || buttonSizeTween.isAlive || highlightOpacityTween.isAlive || labelOpacityTween.isAlive;
    }

}
