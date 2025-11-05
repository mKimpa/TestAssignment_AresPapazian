using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

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

    void Start()
    {
        starButton.onClick.AddListener(OnStarButtonClick);
        originalScale = starIcon.transform.localScale;

        sparkleController = sparkleParticles?.GetComponent<SparkleController>();
        glowAnimation = glowImage?.GetComponent<GlowAnimation>();

        if (LevelCompleteScreen != null)
            LevelCompleteScreen.SetActive(false);
    }

    void OnStarButtonClick()
    {
        
        if (clickSequence.isAlive)
            clickSequence.Stop();

        
        glowAnimation?.StopPulse();

        //OnClick Animation
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
            LevelCompleteScreen.SetActive(true);

            AnimateLevelCompleteApearance();
            sparkleParticles.Stop();
            sparkleParticles.Clear();
            
        }
    }

    void AnimateLevelCompleteApearance()
    {
        
        if (LevelCompleteScreen.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
           
            Sequence.Create()
                .Group(Tween.Alpha(canvasGroup, 0f, 1f, 0.5f))
                .Group(Tween.Scale(LevelCompleteScreen.transform, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack));

        }
        else
        {
            
            Tween.Scale(LevelCompleteScreen.transform, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack);
        }
    }

    void OnDestroy()
    {
        if (clickSequence.isAlive)
            clickSequence.Stop();
    }
}