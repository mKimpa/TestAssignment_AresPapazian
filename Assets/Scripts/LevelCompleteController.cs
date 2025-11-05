using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using System.Collections;
using TMPro;

public class LevelCompleteController : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem[] celebrationParticles;

    [Header("Currency Animation")]
    public TextMeshProUGUI currencyText;
    public GameObject currencyIcon;
    public int earnedCurrency = 100; // Gained Stars
    public float countDuration = 1.5f;
    public float scaleIntensity = 1.2f;

    [Header("Animation Settings")]
    public float delayBetweenEffects = 0.2f;
    public float showDelay = 0.1f;

    [Header("UI Elements")]
    public Transform titleTransform;

    private int currentCurrencyValue = 0;
    private Sequence celebrationSequence;
    private Coroutine currencyCoroutine;

    private void OnEnable()
    {
        StartCelebrationSequence();
    }

    private void StartCelebrationSequence()
    {
        ResetPanelState();

        celebrationSequence = Sequence.Create()
            .Chain(Tween.Scale(titleTransform, Vector3.zero, Vector3.one, 0.7f, Ease.OutBack))
            .ChainCallback(() => PlayMainParticles())
            .Chain(Tween.Delay(showDelay))
            .ChainCallback(() => StartCurrencyAnimation())
            .Chain(Tween.Delay(delayBetweenEffects))
            .OnComplete(() => Debug.Log("Level complete celebration finished!"));
    }

    private void StartCurrencyAnimation()
    {

        Sequence iconSequence = Sequence.Create()
           .Chain(Tween.Scale(currencyIcon.transform, Vector3.zero, Vector3.one * 1.3f, 0.4f, Ease.OutBack))
           .Chain(Tween.Scale(currencyIcon.transform, Vector3.one * 1.3f, Vector3.one, 0.2f, Ease.InOutSine))
           .Chain(Tween.Rotation(currencyIcon.transform, Vector3.zero, new Vector3(0, 0, 15f), 0.1f))
           .Chain(Tween.Rotation(currencyIcon.transform, new Vector3(0, 0, 15f), new Vector3(0, 0, -10f), 0.1f))
           .Chain(Tween.Rotation(currencyIcon.transform, new Vector3(0, 0, -10f), Vector3.zero, 0.1f))
           .OnComplete(() => {
               
               if (this.isActiveAndEnabled)
               {
                   currencyCoroutine = StartCoroutine(AnimateCurrencyCounter());
               }
               else
               {
                   currencyText.text = earnedCurrency.ToString();
               }
           });
    }

    private IEnumerator AnimateCurrencyCounter()
    {
        currencyText.text = "0";
        currentCurrencyValue = 0;

        float timer = 0f;
        int startValue = 0;
        int endValue = earnedCurrency;

        
        while (timer < countDuration && this.isActiveAndEnabled)
        {
            timer += Time.deltaTime;
            float progress = timer / countDuration;

            currentCurrencyValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, progress));
            currencyText.text = currentCurrencyValue.ToString();

            yield return null;
        }

        
        if (this.isActiveAndEnabled)
        {
            currentCurrencyValue = endValue;
            currencyText.text = currentCurrencyValue.ToString();

            Sequence.Create()
                .Group(Tween.Scale(currencyText.transform, Vector3.one * scaleIntensity, Vector3.one, 0.3f, Ease.OutBack))
                .Group(Tween.Color(currencyText, Color.yellow, Color.white, 0.3f));
        }
    }

    private void PlayMainParticles()
    {

        foreach (var ps in celebrationParticles)
        {
            if (ps != null)
            {
                ps.Clear();
                ps.Play();
            }
        }
    }

    private void ResetPanelState()
    {
        if (titleTransform != null)
        {
            titleTransform.localScale = Vector3.zero;
        }

        if (currencyText != null)
        {
            currencyText.text = "0";
            currencyText.transform.localScale = Vector3.one;
            currencyText.color = Color.white; 
        }

        if (currencyIcon != null)
        {
            currencyIcon.transform.localScale = Vector3.zero;
            currencyIcon.transform.rotation = Quaternion.identity;
        }

        StopAllParticles();
    }

    private void StopAllParticles()
    {
        foreach (var ps in celebrationParticles)
        {
            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    private void OnDisable()
    {
        
        if (celebrationSequence.isAlive)
            celebrationSequence.Stop();

        if (currencyCoroutine != null)
        {
            StopCoroutine(currencyCoroutine);
            currencyCoroutine = null;
        }

        StopAllParticles();
        StopAllCoroutines(); 
    }

    public void SetEarnedCurrency(int amount)
    {
        earnedCurrency = amount;
    }

    public void ShowLevelComplete(int currencyEarned = 0)
    {
        if (currencyEarned > 0)
        {
            earnedCurrency = currencyEarned;
        }
        gameObject.SetActive(true);
    }
}