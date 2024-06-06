using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallSegment : MonoBehaviour
{
    [SerializeField][Range(0, 3)] private int health = 3;
    [SerializeField] private int row;

    private Crossbowman crossbowman;
    //private Collider collider;

    // Start is called before the first frame update
    private void Start()
    {
        crossbowman = transform.GetChild(0).GetComponent<Crossbowman>();
        //collider = GetComponent<Collider>();
    }

    private int GetHealht()
    {
        return health;
    }

    //private void Update()
    //{
    //    ManageStateChange();
    //}

    //private void ManageStateChange()
    //{
    //    //if(health > 0)
    //    //{
    //    //    collider.enabled = false;
    //    //}

    //    if (health == 3)
    //    {
    //        crossbowman.SetState(CrossbowmanState.revive);
    //        return;
    //    }

    //    if (health == 0)
    //    {
    //        //collider.enabled = true;
    //        crossbowman.SetState(CrossbowmanState.dead);
    //        return;
    //    }
    //}

    public int GetRow()
    {
        return row;
    }

    public int GetHealth()
    {
        return health;
    }

    public void DoDamage(int i = 1)
    {
        health -= i;

        if (health < 0)
            health = 0;
    }

    public void DoRepair(int i = 1)
    {
        health += i;

        if(health > 3)
            health = 3;
    }
}
