using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action OnEnterCatapult;
    public static event Action OnExitCatapult;
    
    public delegate void FloatEvent(float value);
   
    public static event FloatEvent OnPlayerStunned;
    

    public static void RaiseEnterCatapult()
    {
        OnEnterCatapult?.Invoke();
    }
    
    public static void RaiseExitCatapult()
    {
        OnExitCatapult?.Invoke();
    }

    public static void RaisePlayerStunned(float duration)
    {
        OnPlayerStunned?.Invoke(duration);
    }
}
