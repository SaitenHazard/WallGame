using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public delegate void FloatEvent(float value);
    public delegate void TransformEvent(Vector3 value);
    public static event TransformEvent OnEnterCatapult;
    public static event TransformEvent OnExitCatapult;

    public static event FloatEvent OnPlayerStunned;
    

    public static void RaiseEnterCatapult(Vector3 position)
    {
        OnEnterCatapult?.Invoke(position);
    }
    
    public static void RaiseExitCatapult(Vector3 position)
    {
        OnExitCatapult?.Invoke(position);
    }

    public static void RaisePlayerStunned(float duration)
    {
        OnPlayerStunned?.Invoke(duration);
    }
}
