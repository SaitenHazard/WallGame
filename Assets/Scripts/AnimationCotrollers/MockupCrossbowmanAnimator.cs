using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MockupCrossbowmanAnimator : MonoBehaviour
{
    [SerializeField]
    public int shotsFired;

    private Animator anim;

    MockupBoltReceiver boltReceiver;

    public GameObject bolt;

    void Start()
    {
        anim = GetComponent<Animator>();

        randomizeLook();
        randomizeSpeed(0.95f, 1.05f);
    }

    void Update()
    {
        
    }

    public void AnimEvent_ShotFired()
    {
        Invoke("boltArrives", 2);

        GameObject boltGO = Instantiate(bolt, transform.position + new Vector3(0,-36.5f,15f), Quaternion.identity);
        boltGO.transform.Rotate(-20.0f, Random.Range(-1.0f,1.0f), 0);
        Destroy(boltGO, 2);
        randomizeSpeed(0.95f, 1.05f); // Each shot is done with a little bit of different speed
    }

    bool debug_noReceiver = false;

    private void boltArrives()
    {
        if (debug_noReceiver) return;

        if (boltReceiver == null)
        {
            boltReceiver = FindObjectOfType<MockupBoltReceiver>();
            if (boltReceiver == null)
            {
                Debug.LogWarning("CrossbowAnimator could not find MockupBoldReceiver and will not send updates to it. - Tobi");
                debug_noReceiver = true;
            }
        }
        boltReceiver.EnqueueDamage(1);
    }

    public void StopRunning()
    {
        anim.SetTrigger("StopRunning");
    }

    public void StartRunning()
    {
        anim.SetTrigger("StartRunning");
    }



    private void randomizeLook()
    {
        bool leftHanded = Random.Range(0.0f, 1.0f) < 0.3f;
        transform.localScale = new Vector3(leftHanded? -1 : 1, 1, 1) * Random.Range(0.99f, 1.01f);
    }

    private void randomizeSpeed(float minSpeed, float maxSpeed)
    {
        anim.SetFloat("InherentSpeedup", Random.Range(minSpeed, maxSpeed));
    }
}
