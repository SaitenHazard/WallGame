using System;
using UnityEngine;

public abstract class Upgrade : MonoBehaviour
{
    /* Activates the Upgrade if possible and returns false if not possible, true otherwise. */
    public abstract bool Activate();

    public abstract void UpgradeUpdate();

    public UpgradeName UpgradeName;

    public bool DEBUG_Activate = false;

    
    void Update()
    {
        if (DEBUG_Activate)
        {
            bool success = Activate();
            if (success)
            {
                Debug.Log("Activation Successful.");
            } else
            {
                Debug.Log("Activation Failed!");
            }
            DEBUG_Activate = false;
        }
        UpgradeUpdate();
    }

}