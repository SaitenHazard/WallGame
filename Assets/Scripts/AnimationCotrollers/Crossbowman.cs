using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This functionality has been merged with FriendlySoldier.cs
/// </summary>
[RequireComponent(typeof(Animator))]
public class Crossbowman : MonoBehaviour
{
    private void Update()
    {
        print("Use FriendlySoldier.cs instead");
    }
    // private Animator anim;
    //
    // ArmyController boltReceiver;
    //
    // public TargetProjectile boltPrefab;
    //
    // public Transform releasePoint;
    //
    // [SerializeField]
    // private List<GameObject> helmetVariants;
    //
    // void Start()
    // {
    //     anim = GetComponent<Animator>();
    //     boltReceiver = FindObjectOfType<ArmyController>();
    //
    //     randomizeLook();
    //     randomizeSpeed(0.95f, 1.05f);
    // }
    //
    // void Update()
    // {
    //     
    // }
    //
    // public void AnimEvent_ShotFired()
    // {
    //     Invoke("boltArrives", 2);
    //
    //     TargetProjectile boltGO = Instantiate(boltPrefab, releasePoint.position, releasePoint.rotation);
    //     boltGO.SetDestination(boltReceiver.GetFootsoldierPosition());
    //     boltGO.SetFlightTime(2.0f);
    //
    //     randomizeSpeed(0.95f, 1.05f); // Each shot is done with a little bit of different speed
    // }
    //
    // bool debug_noReceiver = false;
    //
    // private void boltArrives()
    // {
    //     if (debug_noReceiver) return;
    //
    //     if (boltReceiver == null)
    //     {
    //         boltReceiver = FindObjectOfType<ArmyController>();
    //         if (boltReceiver == null)
    //         {
    //             Debug.LogWarning("CrossbowAnimator could not find MockupBoldReceiver and will not send updates to it. - Tobi");
    //             debug_noReceiver = true;
    //         }
    //     }
    //     boltReceiver.Kill(1);
    // }
    //
    // public void StopRunning()
    // {
    //     anim.SetTrigger("StopRunning");
    // }
    //
    // public void StartRunning()
    // {
    //     anim.SetTrigger("StartRunning");
    // }
    //
    //
    //
    // private void randomizeLook()
    // {
    //     bool leftHanded = Random.Range(0.0f, 1.0f) < 0.3f;
    //     transform.localScale = new Vector3((leftHanded? -1 : 1)* transform.localScale.x, transform.localScale.y, transform.localScale.z);
    //     int helmet = Random.Range(0, helmetVariants.Count);
    //     Debug.Log("Helmet " + helmet);
    //     for (int i = 0; i < helmetVariants.Count; i++)
    //     {
    //         helmetVariants[i].SetActive(i == helmet);
    //     }
    // }
    //
    // private void randomizeSpeed(float minSpeed, float maxSpeed)
    // {
    //     anim.SetFloat("InherentSpeedup", Random.Range(minSpeed, maxSpeed));
    // }
}
