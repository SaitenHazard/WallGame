using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Wall;

public class WallSegmentCriticalUI : MonoBehaviour
{
    public List<WallSegment> criticalWallSegments = new();

    public float pulseSpeed = 1.0f; // Adjust speed of pulsation
    public float minAlpha = 0.5f; // Minimum opacity during pulse
    public float maxAlpha = 1.0f; // Maximum opacity during pulse
    private Image _image;

    private bool _setUp;

    private void Start()
    {
        Invoke(nameof(DelayedSetUp), 0.1f);
    }

    private void Update()
    {
        if (!_setUp) return;
        var alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * pulseSpeed, 1.0f));
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, alpha);

        if (criticalWallSegments.Count == 0) gameObject.SetActive(false);
    }

    public void DelayedSetUp()
    {
        _setUp = true;
        _image = GetComponent<Image>();

        var wallSegments = WallManager.instance.GetWallSegments();

        foreach (var segment in wallSegments)
        {
            segment.OnWallSegmentCritical.AddListener(OnWallSegmentCritical);
            segment.OnWallNotSegmentCritical.AddListener(OnWallSegmentNotCritical);
        }
    }

    private void OnWallSegmentCritical(WallSegment segment)
    {
        Debug.Log("OnWallSegmentCritical");
        criticalWallSegments.Add(segment);
        gameObject.SetActive(true);
    }

    private void OnWallSegmentNotCritical(WallSegment segment)
    {
        Debug.Log("OnWallSegmentNotCritical");
        criticalWallSegments.Remove(segment);
    }
}