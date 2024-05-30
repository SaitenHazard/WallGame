using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class CatapultController : MonoBehaviour
{
    private Catapult catapult;
    
    void Start()
    {
        catapult = FindObjectOfType<Catapult>();
        if (catapult == null)
        {
            Debug.LogWarning("CatapultController.cs could not find a Catapult.cs script in the scene and is now deactivated. Make sure to include a Catapult prefab in the scene and activate its Catapult.cs component!");
            this.enabled = false;
        }
        
    }
    void Update()
    {
        
    }

    public void OnAim(InputValue input)
    {
        if (input != null)
        {
            catapult.Aim(input.Get<Vector2>());
        }
        Debug.Log("AIMING");
    }
}
