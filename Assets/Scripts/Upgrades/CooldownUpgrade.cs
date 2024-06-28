using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class CooldownUpgrade : Upgrade
{
    public float cooldown = 5.0f;
    public float standardAnimDur = 3.03f;
    private Animator anim;

    [SerializeField]
    private ReadyFlag readyFlag;

    bool ready = false;

    private float lastEngaged = -500.0f;

    public float remainingSeconds = 0; // For debugging, to see cooldown in window

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("Normalization", (standardAnimDur / cooldown));
    }

    public override void UpgradeUpdate()
    {
        if (ready) { return; }
        remainingSeconds = cooldown - (Time.time - lastEngaged);
        if (Time.time - lastEngaged >= cooldown)
        {
            ready = true;
            readyFlag.SwitchToGreen();
        }
    }

    public abstract void Engage();

    public override bool Activate()
    {
        if (ready)
        {
            lastEngaged = Time.time;
            ready = false;
            readyFlag.SwitchToRed();
            anim.SetTrigger("Engage");
            Engage();
            return true;
        }

        return false;
    }
}