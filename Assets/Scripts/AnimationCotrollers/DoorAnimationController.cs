using UnityEngine;

[RequireComponent (typeof(Animator))]
public class DoorAnimationController : MonoBehaviour
{
    private Animator anim;

    public void Start()
    {
        anim = GetComponent<Animator> ();
    }

    public void Update ()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            Open();
        }
    }

    public void Open()
    {
        progress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

        Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name);

        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Tür Armature|DoorClose") 
            anim.Play("DoorOpen", 0, 1 - progress);
        else
            anim.SetTrigger("Open");
        
        Debug.Log("Progress: " + progress);
    }

    float progress = 0;
    
}