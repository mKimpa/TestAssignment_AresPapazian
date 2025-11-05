using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class HomeButtonController : MonoBehaviour
{
    [Header("References")]
    public Button homeButton;
    public GameObject LevelCompleteScreen; 

    [Header("Animation Settings")]
    public float clickDuration = 0.2f;

    private Vector3 originalScale;

    void Start()
    {
        homeButton.onClick.AddListener(OnHomeButtonClick);
        originalScale = homeButton.transform.localScale;

        
        if (LevelCompleteScreen != null)
            LevelCompleteScreen.SetActive(false);
    }

    void OnHomeButtonClick()
    {
        
        Sequence.Create()
            .Chain(Tween.Scale(homeButton.transform, originalScale, originalScale * 0.9f, clickDuration * 0.5f))
            .Chain(Tween.Scale(homeButton.transform, originalScale * 0.9f, originalScale, clickDuration * 0.5f, Ease.OutBack))
            .OnComplete(HideReward);
    }

    void HideReward()
    {
        if (LevelCompleteScreen != null && LevelCompleteScreen.activeSelf)
        {
            
            if (LevelCompleteScreen.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                Sequence.Create()
                    .Group(Tween.Alpha(canvasGroup, 1f, 0f, 0.3f))
                    .Group(Tween.Scale(LevelCompleteScreen.transform, Vector3.one, Vector3.zero, 0.3f, Ease.InBack))
                    .OnComplete(() => LevelCompleteScreen.SetActive(false));
            }
            else
            {
                Tween.Scale(LevelCompleteScreen.transform, Vector3.one, Vector3.zero, 0.3f, Ease.InBack)
                    .OnComplete(() => LevelCompleteScreen.SetActive(false));
            }

        }
    }

    public void CloseReward()
    {
        HideReward();
    }
}