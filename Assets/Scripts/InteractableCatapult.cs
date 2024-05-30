using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableCatapult : MonoBehaviour, IInteractable
{
    private bool _inCatapult = false;
    public void Interact()
    {
        print("TEST");
        EventManager.RaisePlayerStunned(1f);
        if (_inCatapult)
        {
            EventManager.RaiseExitCatapult();
        }
        else
        {
            EventManager.RaiseEnterCatapult();
        }
    }
}
