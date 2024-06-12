using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Wall
{
    public class FriendlySoldier : MonoBehaviour
    {
        public float moveSpeed = 5f; // Speed at which the soldier moves
        private Vector3 _targetPosition;
        private bool _isMoving;
        private WallSegment _targetSegment;
        private Animator _anim;
        [SerializeField]
        private List<GameObject> helmetVariants;
        
        // Animation IDs
        private int _animIDStartRunning;
        private int _animIDStopRunning;
        private int _animIDRightPlace;
        private int _animIDDeath;
        private int _animIDInherentSpeedup;

        private void Awake()
        {
            AssignAnimationIDs();
            _anim = GetComponent<Animator>();
            _anim.SetTrigger(_animIDStopRunning);
            _anim.SetBool(_animIDRightPlace, true);
            RandomizeLook();
            RandomizeSpeed(0.95f, 1.05f);
        }

        private void AssignAnimationIDs()
        {
            _animIDStartRunning = Animator.StringToHash("StartRunning");
            _animIDStopRunning = Animator.StringToHash("StopRunning");
            _animIDRightPlace = Animator.StringToHash("RightPlace");
            _animIDDeath = Animator.StringToHash("Death");
            _animIDInherentSpeedup = Animator.StringToHash("InherentSpeedup");
        }
        
        public void MoveTo(WallSegment destination)
        {
            _anim.SetTrigger(_animIDStartRunning);
            _targetSegment = destination;
            _targetPosition = _targetSegment.transform.position + new Vector3(0, 0, -0.5f);
            _isMoving = true;
        }

        private void Update()
        {
            if (!_isMoving) return;
            if (!_targetSegment.IsIntact()) Die();
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
            transform.LookAt(_targetPosition);

            if (!WallManager.instance.IsWalkable(transform.position))
            {
                Die();
            }
            if (Vector3.Distance(transform.position, _targetPosition) < 0.001f)
            {
                _isMoving = false;
                _anim.SetTrigger(_animIDStopRunning);
                _anim.SetBool(_animIDRightPlace, true);
                transform.rotation = Quaternion.Euler(Vector3.zero);
                _targetSegment.AssignSoldier(this);
                print("Reached my target");
            }
        }

        public void Die()
        {
            _anim.SetTrigger(_animIDDeath);
            _isMoving = false;
        }
        
        // Invoked by Soldier Animator after Death Animation has finished
        private void FriendlySoldierDeath()
        {
            print("I dieded");
            gameObject.SetActive(false); // Destroy the soldier if the scaffolding is not walkable
            WallManager.instance.RecycleSoldier(this);
        }

        public void AnimEvent_ShotFired()
        {
            // Debug.Log("NEEDAD TO BE FIZED!");
           //Invoke("boltArrives", 2);
        //
          // TargetProjectile boltGO = Instantiate(boltPrefab, releasePoint.position, releasePoint.rotation);
        //   boltGO.SetDestination(boltReceiver.GetFootsoldierPosition());
          // boltGO.SetFlightTime(2.0f);
        //
        //   RandomizeSpeed(0.95f, 1.05f); // Each shot is done with a little bit of different speed
        }

        private void RandomizeLook()
        {
            var leftHanded = Random.Range(0.0f, 1.0f) < 0.3f;
            transform.localScale = new Vector3((leftHanded? -1 : 1)* transform.localScale.x, transform.localScale.y, transform.localScale.z);
            var helmet = Random.Range(0, helmetVariants.Count);
            for (var i = 0; i < helmetVariants.Count; i++)
            {
                helmetVariants[i].SetActive(i == helmet);
            }
        }
        
        private void RandomizeSpeed(float minSpeed, float maxSpeed)
        {
            _anim.SetFloat(_animIDInherentSpeedup, Random.Range(minSpeed, maxSpeed));
        }
    }
}
