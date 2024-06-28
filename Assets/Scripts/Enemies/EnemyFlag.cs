using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyFlag : MonoBehaviour
{
    private Animator anim;

    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("FlagSpeed", Random.Range(1.0f, 2.0f));
    }
}
