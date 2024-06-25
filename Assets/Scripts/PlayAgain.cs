using Input;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgain : MonoBehaviour
{
    private void Awake()
    {
        Inputs.YPressed += Restart;
    }

    private void OnDestroy()
    {
        Inputs.YPressed -= Restart;
    }

    private void Restart()
    {
        SceneManager.LoadScene("Tobi");
    }
}
