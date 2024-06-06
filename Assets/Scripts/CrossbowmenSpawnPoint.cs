using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowmenSpawnPoint : MonoBehaviour
{
    [SerializeField] Queue<Crossbowman> reviveQueue = new();

    private bool lockDoRevive = false;


    private void Awake()
    {
        reviveQueue = new();
    }

    private void Update()
    {

        //if(gameObject.name == "Row2Left")
        //    Debug.Log(reviveQueue.Count);

        if(reviveQueue.Count > 0 && lockDoRevive == false)
            StartCoroutine(DoRevives());
    }

    private IEnumerator DoRevives()
    {
        lockDoRevive = true;

        while (reviveQueue.Count > 0)
        {
            Crossbowman crossbowman = reviveQueue.Dequeue();
            crossbowman.SetState(CrossbowmanState.walk);
            yield return new WaitForSeconds(1);
        }

        lockDoRevive = false;
     }

    public void AddToReviveQueue(Crossbowman crossbowman)
    {
        HideAndRelocateCrossbowman(crossbowman);
        reviveQueue.Enqueue(crossbowman);
    }

    private void HideAndRelocateCrossbowman(Crossbowman crossbowman)
    {
        crossbowman.transform.transform.position = transform.position;
        crossbowman.transform.GetComponent<MeshRenderer>().enabled = true;
    }
}
