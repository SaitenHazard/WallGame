using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wall;

public class WallSegmentCriticalUI : MonoBehaviour
{
    List<WallSegment> _criticalWallSegments = new List<WallSegment>();

    private void Start()
    {
        image = GetComponent<Image>();

        List<WallSegment> _wallSegments = WallManager.instance.GetWallSegments();

        foreach (var segment in _wallSegments)
        {
            segment.onWallSegmentCritical.AddListener(OnWallSegmentCritical);
            segment.onWallNotSegmentCritical.AddListener(OnWallSegmentNotCritical);
        }
    }

    private void OnWallSegmentCritical(WallSegment segment)
    {
        _criticalWallSegments.Add(segment);
        gameObject.SetActive(true);
    }

    private void OnWallSegmentNotCritical(WallSegment segment)
    {
        _criticalWallSegments.Remove(segment);
    }

    private void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * pulseSpeed, 1.0f));
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

        if (_criticalWallSegments.Count == 0) gameObject.SetActive(false);
    }

    public float pulseSpeed = 1.0f; // Adjust speed of pulsation
    public float minAlpha = 0.5f; // Minimum opacity during pulse
    public float maxAlpha = 1.0f; // Maximum opacity during pulse
    private Image image;
}
