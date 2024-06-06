using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scafolding : MonoBehaviour
{
    [SerializeField][Range(0, 3)] private int health = 3;

    Crossbowman crossbowman = new Crossbowman();

    // Start is called before the first frame update
    void Start()
    {
        crossbowman = transform.parent.GetChild(0).GetComponent<Crossbowman>();
    }

    public int GetHealth()
    {
        return health;
    }

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
}