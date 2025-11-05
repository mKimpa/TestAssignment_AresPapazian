using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using System.Collections;

public class StarButtonController : MonoBehaviour
{
    [Header("References")]
    public Button starButton;
    public ParticleSystem sparkleParticles;
    public Image starIcon;
    public Image glowImage;

    [Header("Reward Element")]
    public GameObject LevelCompleteScreen;

    [Header("Animation Settings")]
    public float clickScale = 0.8f;
    public float clickDuration = 0.3f;
    public float rotationAmount = 10f;

    private SparkleController sparkleController;
    private GlowAnimation glowAnimation;
    private Vector3 originalScale;
    private Sequence clickSequence;
    private bool hasInitialized = false;

    void Start()
    {
        starButton.onClick.AddListener(OnStarButtonClick);
        originalScale = starIcon.transform.localScale;

        sparkleController = sparkleParticles?.GetComponent<SparkleController>();
        glowAnimation = glowImage?.GetComponent<GlowAnimation>();

        if (LevelCompleteScreen != null && !hasInitialized)
        {
            LevelCompleteScreen.SetActive(true);
            StartCoroutine(InitializeScreen());
        }
    }

    private IEnumerator InitializeScreen()
    {
        yield return new WaitForEndOfFrame();
        ResetLevelCompleteScreenState();
        LevelCompleteScreen.SetActive(false);
        hasInitialized = true;
    }

    void OnStarButtonClick()
    {
        if (clickSequence.isAlive)
            clickSequence.Stop();

        glowAnimation?.StopPulse();

        clickSequence = Sequence.Create()
            .Group(Tween.Scale(starIcon.transform, originalScale, originalScale * clickScale, clickDuration * 0.3f, Ease.OutQuad))
            .Group(Tween.Rotation(starIcon.transform, Vector3.zero, new Vector3(0, 0, -rotationAmount), clickDuration * 0.3f))
            .Group(Tween.Scale(starIcon.transform, originalScale * clickScale, originalScale, clickDuration * 0.7f, Ease.OutBack))
            .Group(Tween.Rotation(starIcon.transform, new Vector3(0, 0, -rotationAmount), Vector3.zero, clickDuration * 0.7f))
            .OnComplete(OnClickAnimationComplete);
    }

    void OnClickAnimationComplete()
    {
        if (glowImage != null)
        {
            Sequence.Create()
                .Chain(Tween.Alpha(glowImage, glowImage.color.a, 1f, 0.1f))
                .Chain(Tween.Alpha(glowImage, 1f, 0f, 0.4f))
                .OnComplete(ShowLevelComplete);
        }
        else
        {
            ShowLevelComplete();
        }
    }

    void ShowLevelComplete()
    {
        if (LevelCompleteScreen != null)
        {
            if (!hasInitialized)
            {
                ResetLevelCompleteScreenState();
                hasInitialized = true;
            }

            LevelCompleteScreen.SetActive(true);
            StartCoroutine(AnimateAfterFrame());

            if (sparkleParticles != null)
            {
                sparkleParticles.Stop();
                sparkleParticles.Clear();
            }
        }
    }

    private IEnumerator AnimateAfterFrame()
    {
        yield return null;
        AnimateLevelCompleteAppearance();
    }

    void AnimateLevelCompleteAppearance()
    {
        if (LevelCompleteScreen.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup.alpha = 0f;
            LevelCompleteScreen.transform.localScale = Vector3.zero;

            Sequence.Create()
                .Group(Tween.Alpha(canvasGroup, 0f, 1f, 0.5f))
                .Group(Tween.Scale(LevelCompleteScreen.transform, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack));
        }
        else
        {
            LevelCompleteScreen.transform.localScale = Vector3.zero;
            Tween.Scale(LevelCompleteScreen.transform, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack);
        }
    }

    void ResetLevelCompleteScreenState()
    {
        if (LevelCompleteScreen == null) return;

        if (LevelCompleteScreen.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup.alpha = 0f;
        }
        LevelCompleteScreen.transform.localScale = Vector3.zero;

        var levelCompleteController = LevelCompleteScreen.GetComponent<LevelCompleteController>();
        if (levelCompleteController != null)
        {
            levelCompleteController.ResetPanelState();
        }
    }

    void OnDestroy()
    {
        if (clickSequence.isAlive)
            clickSequence.Stop();
    }
}