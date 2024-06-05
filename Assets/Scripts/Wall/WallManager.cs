using System;
using System.Collections;
using System.Collections.Generic;
using Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wall
{
    public class WallManager : MonoBehaviour, IInteractable
    {
        public enum Selection
        {
            Scaffolding,
            Wall,
            OverheadScaffolding

        }

        public Selection _selection;
        public Transform segmentsParent;
        public GameObject soldierPrefab;
        public GameObject spawnPoint;
        public static WallManager instance;
        public float spawnDelay = 3f;

        private float _spawnTimer;
        private List<WallSegment> _wallSegments;
        private Transform _playerTransform;
        private bool _nearWall;
        private WallSegment _closestSegment;
        private Vector3 _closestGizmo;
        private Queue<Vector3> _requestedSoldierPositions;
    

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Inputs.Select += SetSelection;
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            if (_playerTransform == null) throw new MissingFieldException("The player transform is needed ");
            InitializeWallSegments();
            _requestedSoldierPositions = new Queue<Vector3>();
            RequestSoldier(_wallSegments[0]);
        }

        private void OnDestroy()
        {
            Inputs.Select -= SetSelection;
        }

        private void Update()
        {
            if (_nearWall)
            {
                _closestSegment = _wallSegments[GetClosestWallSegment(_playerTransform.position)];
                _closestGizmo = _closestSegment.transform.position;
            }
            TrySpawnSoldier();
            _spawnTimer -= Time.deltaTime;
        }

        private void SetSelection(InputValue value)
        {
            var input = value.Get<Vector2>();
            _selection += (int) input.y;
            if ((int)_selection == -1) _selection = Selection.Scaffolding;
            if ((int)_selection == 3) _selection = Selection.OverheadScaffolding;
            print("Current selection: " + _selection);
        }

        private void InitializeWallSegments()
        {
            // Clear the existing list
            _wallSegments = new List<WallSegment>();

            // Iterate through all children and add their WallSegment component to the list
            foreach (Transform child in segmentsParent)
            {
                var segment = child.GetComponent<WallSegment>();
                if (segment != null)
                {
                    _wallSegments.Add(segment);
                }
            }
        }
    
    
        private int GetClosestWallSegment(Vector3 position)
        {
            return _selection switch
            {
                Selection.OverheadScaffolding => GetClosestWallSegmentDirect(position) + 5,
                Selection.Wall => GetClosestWallSegmentDirect(position),
                Selection.Scaffolding => GetClosestWallSegmentDirect(position),
                _ => GetClosestWallSegmentDirect(position)
            };
        }

        private int GetClosestWallSegmentDirect(Vector3 position)
        {
            var closestSegment = -1;
            var closestDistance = Mathf.Infinity;

            for (var index = 0; index < _wallSegments.Count; index++)
            {
                var distance = Vector3.Distance(position, _wallSegments[index].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSegment = index;
                }
            }

            return closestSegment;
        }

        public bool IsWalkable(Vector3 position)
        {
           return _wallSegments[GetClosestWallSegmentDirect(position)].isScaffoldingIntact;
        }

        private void TrySpawnSoldier()
        {
            if (_requestedSoldierPositions.Count <= 0) return;
            if (_spawnTimer <= 0)
            {
                SpawnSoldier(_requestedSoldierPositions.Dequeue());
                _spawnTimer = spawnDelay; // Reset the spawn timer
                print("Dequeued and spawned; remaining: " + _requestedSoldierPositions.Count);
            }
        }

        private void SpawnSoldier(Vector3 position)
        {
            if (!soldierPrefab) return;
            var newSoldier = Instantiate(soldierPrefab, spawnPoint.transform.position, Quaternion.identity);
            var soldierController = newSoldier.GetComponent<FriendlySoldier>();
            if (soldierController)
            {
                soldierController.MoveTo(position);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(_closestGizmo, 0.5f);
        }
    
        public void Interact(GameObject interactor)
        {
            switch (_selection)
            {
                case Selection.OverheadScaffolding:
                    DamageScaffoldingSegment(_closestSegment);
                    break;
                case Selection.Wall:
                    DamageWallSegment(_closestSegment);
                    break;
                case Selection.Scaffolding:
                    DamageScaffoldingSegment(_closestSegment);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void RequestSoldier(WallSegment segment)
        {
            _requestedSoldierPositions.Enqueue(segment.transform.position);
            print("Requested; remaining: " + _requestedSoldierPositions.Count);
        }
        public void RepairWallSegment(WallSegment segment)
        {
            if (segment != null)
            {
                segment.RepairWall();
            }
        }

        public void DamageWallSegment(WallSegment segment)
        {
            if (segment != null)
            {
                segment.DamageWall();
            }
        }

        public void RepairScaffoldingSegment(WallSegment segment)
        {
            if (segment != null)
            {
                segment.RepairScaffolding();
            }
        }

        public void DamageScaffoldingSegment(WallSegment segment)
        {
            if (segment != null)
            {
                segment.DamageScaffolding();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _nearWall = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _nearWall = false;
            }
        }
    }
}