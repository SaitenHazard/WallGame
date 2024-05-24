using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Der Animation Controller reagiert auf die Werte "falling" (bool), "speed" (float[0,1]) und "jumping" (bool)
 * Wenn diese Werte im Charactercontroller vorliegen, einfach in den AnimationController schmeiﬂen:
 * 
 * speed ist public, modifizier die einfach wie du lustig bist
 * 
 * falling auch, modifizieren und auf true setzen wenn der Spieler sich im freien Fall befindet (Raycast nach unten?)
 * 
 * jumping nicht, da da Animations-Trigger am Werk sind, hier einfach die Funktion public void Jump() verwenden.
 */
[RequireComponent(typeof(Animator))]
public class WalltherAnimationController : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float speed;

    public bool falling;

    private Animator anim;

    private float airTime = 0.0f;

    public bool locked { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        /* For Tobi's debugging pleasure

        if (Input.GetKeyUp(KeyCode.Space) && !locked)
        {
            Jump();
        }
        speed = Mathf.Abs(Input.GetAxis("Vertical"));
        if (Input.GetKey(KeyCode.F))
        {
            falling = true;
        }
        else
        {
            falling = false;
        }*/

        anim.SetFloat("Speed", speed);
        anim.SetBool("Falling", falling);
        anim.SetFloat("AirTime", airTime);
        
        if (falling)
        {
            airTime += Time.deltaTime;
        }
    }

    public void Jump()
    {
        anim.SetTrigger("Jump");
    }

    public void AnimEvent_DoneLanding()
    {
        airTime = 0;
        AnimEvent_DoneAnimating();
    }

    public void AnimEvent_DontInterrupt()
    {
        locked = true;
    }
    public void AnimEvent_DoneAnimating()
    {
        locked = false;
    }
}
