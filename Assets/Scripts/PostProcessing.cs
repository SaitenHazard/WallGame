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
    private Coroutine _currentCoroutine;
    public int smoothingDuration;
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
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }
            _currentCoroutine = StartCoroutine(SmoothChangeVignetteIntensity(Mathf.Lerp(0.5f, 0.0f, (intensity - maxDestruction) / (1.0f - maxDestruction)), smoothingDuration));
        }
    }
    
    private IEnumerator SmoothChangeVignetteIntensity(float targetIntensity, float duration)
    {
        float startIntensity = _vignette.intensity.value;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / duration);
            yield return null;
        }

        _vignette.intensity.value = targetIntensity;
    }
}
