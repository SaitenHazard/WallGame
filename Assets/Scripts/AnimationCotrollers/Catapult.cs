using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class Catapult : MonoBehaviour
{
    private Animator anim;

    [SerializeField]
    private float rotSpeed;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetRotation != 0)
        {
            Debug.Log("Rotating by " + (targetRotation * Time.deltaTime));
            transform.Rotate(new Vector3(0, rotSpeed * targetRotation * Time.deltaTime, 0));
        }
    }

    public void Aim(Vector2 input)
    {
        //Debug.Log("Aiming!" + input);
        float clamped = Mathf.Abs(input.x) > 0.1 ? input.x : 0;
        anim.SetFloat("Rotation", -1 * clamped);
        targetRotation = clamped;
    }

    public float targetRotation = 0;
}
