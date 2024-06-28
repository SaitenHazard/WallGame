using UnityEngine;

public class AutomatedSelfService : CooldownUpgrade
{
    public override void Engage()
    {
        Debug.LogWarning("I Can't repair myself yet ;(");
    }
}