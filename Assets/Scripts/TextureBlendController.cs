using UnityEngine;

public class TextureBlendController : MonoBehaviour
{
    public Material blendMaterial;
    public float blendSpeed = 1.0f;
    private float blendValue = 0.0f;
    private bool blendForward = true;

    void Update()
    {
        if (blendForward)
        {
            blendValue += Time.deltaTime * blendSpeed;
            if (blendValue >= 1.0f)
            {
                blendValue = 1.0f;
                blendForward = false;
            }
        }
        else
        {
            blendValue -= Time.deltaTime * blendSpeed;
            if (blendValue <= 0.0f)
            {
                blendValue = 0.0f;
                blendForward = true;
            }
        }

        blendMaterial.SetFloat("_Blend", blendValue);
    }
}