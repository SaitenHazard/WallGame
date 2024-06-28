using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ReadyFlag : MonoBehaviour
{
    private Animator anim;

    public bool DEBUG_SwitchToRed;
    public bool DEBUG_SwitchToGreen;

#if UNITY_EDITOR
    public void Update()
    {
        if (DEBUG_SwitchToGreen)
        {
            SwitchToGreen();
            DEBUG_SwitchToGreen = false;
        }  
        if (DEBUG_SwitchToRed)
        {
            SwitchToRed();
            DEBUG_SwitchToRed = false;
        }
    }
#endif

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SwitchToGreen()
    {
        anim.SetTrigger("Green");
    }

    public void SwitchToRed()
    {
        anim.SetTrigger("Red");
    }
}