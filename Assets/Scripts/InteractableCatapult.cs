using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableCatapult : MonoBehaviour, IInteractable
{
    private bool inCatapult = false;
    public void Interact()
    {
        if (inCatapult)
        {
            EventManager.RaiseExitCatapult();
        }
        else
        {
            EventManager.RaiseEnterCatapult();
        }
    }
}
