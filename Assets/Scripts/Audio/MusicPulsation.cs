using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class MusicPulsation : MonoBehaviour
{
    private AudioSource audioSource;

    public List<AudioClip> clipList = new List<AudioClip>();

    [Range(0, 1)] public int channel = 0;
    public FFTWindow window;

    public float scaleMultiplier = 1000.0f;

    public static event Action<float> OnGetSpectrum;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!audioSource.isPlaying || audioSource.clip == null) SortNewRandomMusic();

        float[] spectrumData = new float[256];

        audioSource.GetSpectrumData(spectrumData, channel, window);

        float averageAmplitude = 0.0f;
        for (int i = 0; i < spectrumData.Length; i++)
        {
            averageAmplitude += spectrumData[i];
        }

        averageAmplitude /= spectrumData.Length;

        float newScale = 1.0f + averageAmplitude * scaleMultiplier;

        OnGetSpectrum?.Invoke(newScale);
    }


    private void SortNewRandomMusic()
    {
        int random = UnityEngine.Random.Range(0, clipList.Count);

        audioSource.clip = clipList[random];

        audioSource.Play();
    }
}