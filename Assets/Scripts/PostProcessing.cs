using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessing : MonoBehaviour
{
    private Volume _postProcessVolume;
    private Vignette _vignette;
    public float maxDestruction;
    
    private void Start()
    {
        _postProcessVolume = GetComponent<Volume>();
        print(_postProcessVolume);
        _postProcessVolume.profile.TryGet(out _vignette);
    }

    public void UpdateVignetteIntensity(float intensity)
    {
        if (_vignette != null)
        {
            if (intensity > 0.4f)
            {
                _vignette.intensity.value = 0f;
                return;
            }
            _vignette.intensity.value = (intensity - maxDestruction) / (0 - maxDestruction) * (0.5f - 0.4f) + 0.4f;
        }
    }
}
