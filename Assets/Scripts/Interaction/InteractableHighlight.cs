using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class InteractableHighlight : MonoBehaviour
{
    private SpriteRenderer sRenderer;

    float fadingSpeed = 0.1f;
    float maxOpacity = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        sRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float alphaNow = sRenderer.color.a;
        sRenderer.color = new Color(1, 1, 1, Mathf.Clamp(alphaNow + (close ? fadingSpeed : -fadingSpeed), 0, maxOpacity));   
    }

    bool close = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<ThirdPersonController>()) return;

        close = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<ThirdPersonController>()) return;

        close = false;
    }
}
