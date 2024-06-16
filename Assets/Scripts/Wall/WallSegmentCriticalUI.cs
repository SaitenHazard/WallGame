using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wall;

public class WallSegmentCriticalUI : MonoBehaviour
{
    public List<WallSegment> _criticalWallSegments = new List<WallSegment>();

    private void Start()
    {
        Invoke("DelayedSetUp", 0.1f);
    }

    private bool setUp = false;

    public void DelayedSetUp()
    {
        setUp = true;
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
        Debug.Log("OnWallSegmentCritical");
        _criticalWallSegments.Add(segment);
        gameObject.SetActive(true);
    }

    private void OnWallSegmentNotCritical(WallSegment segment)
    {
        Debug.Log("OnWallSegmentNotCritical");
        _criticalWallSegments.Remove(segment);
    }

    private void Update()
    {
        if (!setUp) return;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * pulseSpeed, 1.0f));
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

        if (_criticalWallSegments.Count == 0) gameObject.SetActive(false);
    }

    public float pulseSpeed = 1.0f; // Adjust speed of pulsation
    public float minAlpha = 0.5f; // Minimum opacity during pulse
    public float maxAlpha = 1.0f; // Maximum opacity during pulse
    private Image image;
}
