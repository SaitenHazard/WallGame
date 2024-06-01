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
    private int _animIDJump;
    private int _animIDSpeed;
    private int _animIDFalling;
    private int _animIDAirTime;

    [Range(0.0f, 1.0f)]
    public float speed;
    public bool falling;
    private float _airTime = 0.0f;
    private Animator anim;

    public bool locked { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        AssignAnimationIDs();
    }

    // Update is called once per frame
    void Update()
    {
        if (falling)
        {
            _airTime += Time.deltaTime;
        }
    }

    private void AssignAnimationIDs()
    {
        _animIDJump = Animator.StringToHash("Jump");
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDFalling = Animator.StringToHash("Falling");
        _animIDAirTime = Animator.StringToHash("AirTime");
    }

    public void AnimEvent_DoneLanding()
    {
        _airTime = 0;
        AnimEvent_DoneAnimating();
    }

    public void Launching(bool launching)
    {
        anim.SetTrigger(launching? "Launching" : "DoneLaunching");
    }

    public void AnimEvent_DontInterrupt()
    {
        locked = true;
    }

    public void EnterCatap()
    {
        anim.SetTrigger("EnterCatap");
    }

    public void AnimEvent_DoneAnimating()
    {
        locked = false;
    }

    public void SetJump()
    {
        anim.SetTrigger(_animIDJump);
    }

    public void SetSpeed(float value)
    {
        speed = value;
        anim.SetFloat(_animIDSpeed, value);
    }

    public void SetFalling(bool value)
    {
        falling = value;
        anim.SetBool(_animIDFalling, value);
    }

    public void SetAirTime(float value)
    {
        _airTime = value;
        anim.SetFloat(_animIDAirTime, value);
    }
}