using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class GlowAnimation : MonoBehaviour
{
    [Header("Settings")]
    public Image glowImage;
    public float pulseDuration = 1f;
    public float minAlpha = 0.3f;
    public float maxAlpha = 0.8f;

    private Sequence pulseSequence;

    void Start()
    {
        StartPulseAnimation();
    }

    void StartPulseAnimation()
    {
        pulseSequence = Sequence.Create()
            .Chain(Tween.Alpha(glowImage, minAlpha, maxAlpha, pulseDuration, Ease.InOutSine))
            .Chain(Tween.Alpha(glowImage, maxAlpha, minAlpha, pulseDuration, Ease.InOutSine));

        pulseSequence.SetRemainingCycles(-1);
    }

    void OnDestroy()
    {
        if (pulseSequence.isAlive)
        {
            pulseSequence.Stop();
        }
    }

    public void StopPulse()
    {
        if (pulseSequence.isAlive)
        {
            pulseSequence.Stop();
        }
    }

    public void RestartPulse()
    {
        StopPulse();
        StartPulseAnimation();
    }
}