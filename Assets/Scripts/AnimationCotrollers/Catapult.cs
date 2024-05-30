using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(StarterAssetsInputs))]
public class Catapult : MonoBehaviour
{
    private Animator anim;
    private StarterAssetsInputs playerInput;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerInput = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        float input = playerInput.move.x;
        anim.SetFloat("Rotation", input);
        anim.SetFloat("RotSpeed", Mathf.Abs(input));
    }
}
