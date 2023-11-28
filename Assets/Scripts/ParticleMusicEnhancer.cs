using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMusicEnhancer : MonoBehaviour
{
    public List<ParticleSystem> _particleSystems;

    public float baseIntensity = 0;
    public float intensityMultiplier = 5;
    public int powerMultiplier = 3;

    private void Start()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }


    private void SubscribeToEvents()
    {
        MusicPulsation.OnGetSpectrum += AdaptToMusic;
    }

    private void UnsubscribeFromEvents()
    {
        MusicPulsation.OnGetSpectrum -= AdaptToMusic;
    }


    private void AdaptToMusic(float intensity)
    {
        foreach (ParticleSystem particleSystem in _particleSystems)
        {
            var mainModule = particleSystem.main;

            mainModule.startLifetime = baseIntensity + Mathf.Pow((intensity - 1) * intensityMultiplier, powerMultiplier);
        }
    }
}
