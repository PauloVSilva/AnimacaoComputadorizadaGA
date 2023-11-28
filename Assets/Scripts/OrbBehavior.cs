using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbBehavior : MonoBehaviour
{
    public float baseIntensity = 0;
    public float intensityMultiplier = 5;
    public int powerMultiplier = 3;

    private void Start()
    {
        MusicPulsation.OnGetSpectrum += ScaleWithMusic;

        Destroy(gameObject, 30);
    }


    private void OnDestroy()
    {
        MusicPulsation.OnGetSpectrum -= ScaleWithMusic;
    }

    private void ScaleWithMusic(float scale)
    {
        float newScale = baseIntensity + Mathf.Pow((scale - 1) * intensityMultiplier, powerMultiplier);

        if (gameObject != null) transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}
