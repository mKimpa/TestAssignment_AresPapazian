using UnityEngine;
using UnityEngine.UI;

public class SparkleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem sparkleParticles;

    public float emissionRate = 10f;

    void Start()
    {
        PlaySparkles();
    }

    public void PlaySparkles()
    {
        if (sparkleParticles != null && !sparkleParticles.isPlaying)
            sparkleParticles.Play();
    }
}