using System;
using UnityEngine;

public class TextureBlendController : MonoBehaviour
{
    public Material blendMaterial;
    public float blendSpeed = 1.0f;
    private bool _blendForward = true;
    private float _blendValue;
    private int _animIDBlend;

    private void Start()
    {
        _animIDBlend = Animator.StringToHash("Blend");
    }

    private void Update()
    {
        if (_blendForward)
        {
            _blendValue += Time.deltaTime * blendSpeed;
            if (_blendValue >= 1.0f)
            {
                _blendValue = 1.0f;
                _blendForward = false;
            }
        }
        else
        {
            _blendValue -= Time.deltaTime * blendSpeed;
            if (_blendValue <= 0.0f)
            {
                _blendValue = 0.0f;
                _blendForward = true;
            }
        }

        blendMaterial.SetFloat(_animIDBlend, _blendValue);
    }
}