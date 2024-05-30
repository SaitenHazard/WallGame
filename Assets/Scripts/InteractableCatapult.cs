using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableCatapult : MonoBehaviour, IInteractable
{
    private bool _inCatapult = false;
    public Transform dropInPoint;
    public Transform dropOffPoint;
    public void Interact()
    {
        if (_inCatapult)
        {
            _inCatapult = false;
            EventManager.RaiseExitCatapult(dropOffPoint.position);
        }
        else
        {
            _inCatapult = true;
            EventManager.RaiseEnterCatapult(dropInPoint.position);
        }
    }
}
