using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public enum CrossbowmanState
{
    walk,
    shoot,
    wait,
    dead,
    revive,
    NULL
}

public class Crossbowman_towsif : MonoBehaviour
{
    [SerializeField] private CrossbowmanState _state;
    private CrossbowmenSpawnPoint _crossbowmenSpawnPoints;
    private readonly float _moveSpeed = 2f;

    private WallSegment wallSegment;
    private Scafolding scafolding;

    private bool wallDamaged;
    private bool schafoldingDamaged;

    private void Start()
    {
        _state = CrossbowmanState.shoot;
        wallSegment = transform.parent.GetComponent<WallSegment>();
        scafolding = transform.parent.GetChild(1).GetComponent<Scafolding>();
    }

    public void SetSpawnPoint(CrossbowmenSpawnPoint crossbowmenSpawnPoints)
    {
        //Debug.Log("IN");
        _crossbowmenSpawnPoints = crossbowmenSpawnPoints;
    }

    private void Update()
    {
        bool tmep = WallSegmentManager.instance.InfrontOfDamagedScafolding(transform, true);

        if (tmep != false)
            Debug.Log(tmep);

        UpdateWallHealth();
        UpdateScafoldingHealth();
        DoDead();
        DoRevive();
        DoWalk();
    }

    private void UpdateWallHealth()
    {
        if (wallSegment.GetHealth() == 3)
            wallDamaged = false;
        else
            wallDamaged = true;
    }

    private void UpdateScafoldingHealth()
    {
        if (scafolding.GetHealth() == 0)
            schafoldingDamaged = true;
        else 
            schafoldingDamaged = false;
    }

    private void DoWalk()
    {
        if( _state == CrossbowmanState.walk )
        {
            Vector3 wallSegmentPosition = transform.parent.position;

            float moveSpeed = _moveSpeed;
            bool infrontOfDamagedScafolding;

            if (_crossbowmenSpawnPoints.name == "Row2Left")
            {
                infrontOfDamagedScafolding = WallSegmentManager.instance.InfrontOfDamagedScafolding(transform, true);
            }
            else
            {
                infrontOfDamagedScafolding = WallSegmentManager.instance.InfrontOfDamagedScafolding(transform, false);
            }

            if (infrontOfDamagedScafolding == true)
                moveSpeed = 0;

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(wallSegmentPosition.x, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime);

            if (Mathf.Abs(wallSegmentPosition.x - transform.position.x) < 0.5f)
                SetState(CrossbowmanState.shoot);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("in");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("in");

    }

    public void SetState(CrossbowmanState state)
    {
        if (state == CrossbowmanState.dead && _state != CrossbowmanState.dead)
        {
            _state = state;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            return;
        }

        if(state == CrossbowmanState.revive)
        {
            if (_state == CrossbowmanState.dead && _state != CrossbowmanState.revive)
            {
                _state = state;
                _crossbowmenSpawnPoints.AddToReviveQueue(this);
                return;
            }
        }

        if(state == CrossbowmanState.walk)
        {
            if (_state == CrossbowmanState.revive || _state != CrossbowmanState.walk)
            {
                _state = state;
                return;
            }
        }

        if(state == CrossbowmanState.wait)
        {
            if (_state == CrossbowmanState.walk || _state != CrossbowmanState.wait)
            {
                _state = state;
                return;
            }
        }
    }

    private void DoDead()
    {
        if (wallDamaged || schafoldingDamaged)
        {
            SetState(CrossbowmanState.dead);
        }
    }

    private void DoRevive()
    {
        if (wallDamaged == false && schafoldingDamaged == false)
        {
            SetState(CrossbowmanState.revive);
        }
    }
}
