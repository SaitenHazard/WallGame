using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Interactable : MonoBehaviour, IInteractable
{
    private Renderer _cubeRenderer;
    private void Start()
    {
        _cubeRenderer = GetComponent<Renderer>();
    }

    public void Interact()
    {
        var color = new Color(Random.Range(0, 256) / 255f, Random.Range(0, 256) / 255f, Random.Range(0, 256) / 255f);
        print("COLORING :" + color );
        _cubeRenderer.material.SetColor("_BaseColor", color);
    }
}
