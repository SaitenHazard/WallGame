using System.Collections.Generic;
using UnityEngine;

namespace Wall
{
    public class FriendlySoldier : MonoBehaviour
    {
        public float moveSpeed = 5f; // Speed at which the soldier moves
        private Vector3 _targetPosition;
        private bool _isMoving = false;
        private WallSegment _targetSegment;
        private Animator _anim;
        [SerializeField]
        private List<GameObject> helmetVariants;

        void Start()
        {
            _anim = GetComponent<Animator>();
            _anim.SetTrigger("StartRunning");

            RandomizeLook();
            RandomizeSpeed(0.95f, 1.05f);
        }
        
        public void MoveTo(WallSegment destination)
        {
            _targetSegment = destination;
            _targetPosition = _targetSegment.transform.position;
            _isMoving = true;
        }

        private void Update()
        {
            if (!_isMoving) return;
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
            transform.LookAt(_targetPosition);

            if (!WallManager.instance.IsWalkable(transform.position))
            {
                Die();
            }
            if (Vector3.Distance(transform.position, _targetPosition) < 0.001f)
            {
                _isMoving = false;
                _anim.SetTrigger("StopRunning");
                _anim.SetBool("RightPlace", true);
                transform.rotation = Quaternion.Euler(Vector3.zero);
                _targetSegment.AssignSoldier(this);
                print("Reached my target");
            }
        }

        public void Die()
        {
            _anim.SetTrigger("Death");
            _isMoving = false;
        }
        private void FriendlySoldierDeath()
        {
            Destroy(gameObject); // Destroy the soldier if the scaffolding is not walkable
            print("I dieded");
            _isMoving = false;
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
            _anim.SetFloat("InherentSpeedup", Random.Range(minSpeed, maxSpeed));
        }
    }
}
