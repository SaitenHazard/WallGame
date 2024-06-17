using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    public delegate void NoParamsEvent();
    public delegate void FloatEvent(float value);
    public delegate void IntEvent(int value);
    public delegate void TransformEvent(Transform value);
    public delegate void PathEvent(Vector3[] path, int vertexCount);

    public static event TransformEvent OnEnterCatapult;
    public static event TransformEvent OnExitCatapult;
    public static event PathEvent OnCatapultFire;
    public static event FloatEvent OnPlayerStunned;

    public static event IntEvent OnReplenishWood;
    public static event IntEvent OnReplenishStone;

    public static event NoParamsEvent OnRepairedWood;
    public static event NoParamsEvent OnRepairedStone;

    public static event IntEvent OnWallPieceHit;
    public static event IntEvent OnScaffoldingHit;


    public static event NoParamsEvent OnGameOver;

    


    public static void RaiseCatapultFire(Vector3[] path, int vertexCount)
    {
        OnCatapultFire?.Invoke(path, vertexCount);
    }

    public static void RaiseEnterCatapult(Transform catapultBowl)
    {
        OnEnterCatapult?.Invoke(catapultBowl);
    }
    
    public static void RaiseExitCatapult(Transform exitPoint)
    {
        OnExitCatapult?.Invoke(exitPoint);
    }

    public static void RaisePlayerStunned(float duration)
    {
        OnPlayerStunned?.Invoke(duration);
    }

    public static void RaiseOnReplenishWood(int amount)
    {
        OnReplenishWood?.Invoke(amount);
    }
    public static void RaiseOnReplenishStone(int amount)
    {
        OnReplenishStone?.Invoke(amount);
    }

    public static void RaiseOnWallPieceHit(int index)
    {
        OnWallPieceHit?.Invoke(index);
    }

    public static void RaiseOnScaffoldingHit(int index)
    {
        OnScaffoldingHit?.Invoke(index);
    }

    public static void RaiseOnRepairedWood()
    {
        OnRepairedWood?.Invoke();
    }

    public static void RaiseOnRepairedStone()
    {
        OnRepairedStone?.Invoke();
    }

    public static void RaiseGameOver()
    {
        SceneManager.LoadScene("GameOver");
        //print("GAME OVER");

        OnGameOver?.Invoke();
    }
}
