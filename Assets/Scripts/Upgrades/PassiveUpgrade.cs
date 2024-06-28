

using UnityEngine;

public class PassiveUpgrade : Upgrade
{
    public override bool Activate()
    {
        Debug.Log("The " + name + " took effect.");
        return true;
    }

    public override void UpgradeUpdate()
    {
        // Passive
    }
}

