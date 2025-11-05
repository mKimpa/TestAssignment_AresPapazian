using PT = PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CoinAnimationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private RectTransform coinTarget; // Coins homescreen top panel Widget
    [SerializeField] private TextMeshProUGUI coinText; 
    [SerializeField] private Button addCoinButton; 
    [SerializeField] private Transform spawnArea; 

    [Header("Animation Settings")]
    [SerializeField] private int coinsPerSpawnedCoin = 10;
    [SerializeField] private int coinsToSpawn = 5;
    [SerializeField] private float spawnRadius = 100f;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float spreadDuration = 0.3f;
    [SerializeField] private PT.Ease easeType = PT.Ease.OutCubic;
    [SerializeField] private PT.Ease spreadEaseType = PT.Ease.OutBack;

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem coinSpawnParticle;
    [SerializeField] private ParticleSystem coinCollectParticle;

    private Queue<PT.Tween> activeTweens = new Queue<PT.Tween>();

    private void Start()
    {
        if (addCoinButton != null)
        {
            addCoinButton.onClick.AddListener(OnAddCoinsClick);
        }
    }

    private void OnAddCoinsClick()
    {
        SpawnCoinsAnimation();
    }

    public void SpawnCoinsAnimation()
    {
        // Spawn coin particle effect
        if (coinSpawnParticle != null)
        {
            coinSpawnParticle.Play();
        }

        // Spawn several coins
        for (int i = 0; i < coinsToSpawn; i++)
        {
            SpawnSingleCoin(i * 0.1f); 
        }
    }

    private void SpawnSingleCoin(float delay)
    {
        // New coin creation
        var coin = Instantiate(coinPrefab, spawnArea);
        var coinRect = coin.GetComponent<RectTransform>();
        var coinImage = coin.GetComponent<Image>();

        // Random spawn position
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        coinRect.anchoredPosition = randomPos;

        // coins jump out
        var startScale = Vector3.zero;
        var endScale = Vector3.one;

        // Cpoins apperence animation
        var scaleTween = PT.Tween.Scale(coinRect, endScale, spreadDuration, spreadEaseType, startDelay: delay);
        var spreadTween = PT.Tween.UIAnchoredPosition(coinRect, randomPos * 1.5f, spreadDuration, spreadEaseType, startDelay: delay);

        // Flying to the coins panel widget
        PT.Tween.Delay(spreadDuration + delay + 0.2f)
            .OnComplete(() => MoveToTarget(coinRect, coinImage));
    }

    private void MoveToTarget(RectTransform coinRect, Image coinImage)
    {
        // fly animation
        coinRect.SetParent(coinTarget, true);
        var moveTween = PT.Tween.UIAnchoredPosition(coinRect, Vector2.zero, animationDuration, easeType);

        // scale animation
        var scaleTween = PT.Tween.Scale(coinRect, Vector3.zero, animationDuration * 0.5f, easeType, startDelay: animationDuration * 0.5f);

        // rotation animation
        var rotationTween = PT.Tween.EulerAngles(coinRect, Vector3.forward, Vector3.forward * 360f, animationDuration, PT.Ease.InOutCubic);

        
        PT.Tween.Delay(animationDuration)
            .OnComplete(() => OnCoinReachedTarget(coinRect.gameObject));
    }

    private void OnCoinReachedTarget(GameObject coin)
    {
        
        if (coinCollectParticle != null)
        {
            coinCollectParticle.Play();
        }
        Destroy(coin);

        if (AreAllCoinsCollected())
        {
            GameManager.instance.AddCoins(coinsPerSpawnedCoin);
        }
    }

    private bool AreAllCoinsCollected()
    {
        // we could add some logic here later
        return true;
    }

    public void SetCoinTarget(RectTransform target)
    {
        coinTarget = target;
    }

    private void OnDestroy()
    {
        // Stopping all tweens
        foreach (var tween in activeTweens)
        {
            if (tween.isAlive) tween.Stop();
        }

        if (addCoinButton != null)
        {
            addCoinButton.onClick.RemoveListener(OnAddCoinsClick);
        }
    }
}