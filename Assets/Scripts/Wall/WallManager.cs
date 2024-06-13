using System;
using System.Collections.Generic;
using AnimationCotrollers;
using Input;
using Interaction;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

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

        public int wallRows;
        public int wallColumns;

        public Selection _selection;
        public Transform segmentsParent;
        public Transform doorsParent;
        public Transform soldiersParent;
        public static WallManager instance;
        public float spawnDelay = 3f;
        public int soldierBuffer;
        public FriendlySoldier soldierPrefab;

        private float _spawnTimer;
        private List<WallSegment> _wallSegments;
        private List<DoorAnimationController> _doorControllers;
        private Queue<FriendlySoldier> _availableSoldiers;
        private Transform _playerTransform;
        private bool _nearWall;
        private WallSegment _closestSegment;
        private Vector3 _closestGizmo;
        private Queue<WallSegment> _requestedSoldierPositions;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Inputs.Select += SetSelection;
            EventManager.OnWallPieceHit += DamageWallSegment;
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            if (_playerTransform == null) throw new MissingFieldException("The player transform is needed ");
            InitializeWallSegments();
            InitializeDoorControllers();
            _requestedSoldierPositions = new Queue<WallSegment>();
            _availableSoldiers = new Queue<FriendlySoldier>();

            for (var i = 0; i < soldierBuffer; i++)
            {
                var soldier = Instantiate(soldierPrefab, soldiersParent);
                soldier.gameObject.SetActive(false);
                _availableSoldiers.Enqueue(soldier);
            }
            // InvokeRepeating(nameof(DamageRandomSegment), 1f, 5f);
        }

        private void OnDestroy()
        {
            Inputs.Select -= SetSelection;
            EventManager.OnWallPieceHit -= DamageWallSegment;
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

        private void SetSelection(Vector2 value)
        {
            _selection += (int)value.y;
            if ((int)_selection == -1) _selection = Selection.Scaffolding;
            if ((int)_selection == 3) _selection = Selection.OverheadScaffolding;
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
        private void InitializeDoorControllers()
        {
            // Clear the existing list
            _doorControllers = new List<DoorAnimationController>();

            // Iterate through all children and add their WallSegment component to the list
            foreach (Transform child in doorsParent)
            {
                var door = child.GetComponent<DoorAnimationController>();
                if (door != null)
                {
                    _doorControllers.Add(door);
                }
            }
        }


        private int GetClosestWallSegment(Vector3 position)
        {
            var closestIndex = GetClosestWallSegmentDirect(position);
            switch (_selection)
            {
                case Selection.OverheadScaffolding:
                    if (closestIndex + 5 < _wallSegments.Count) return closestIndex + 5;
                    _selection = Selection.Wall;
                    return closestIndex;
                case Selection.Wall:
                    return closestIndex;
                case Selection.Scaffolding:
                    return closestIndex;
                default:
                    return closestIndex;
            }
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

        private int GetClosestDoor(Vector3 position)
        {
            var closestDoor = -1;
            var closestDistance = Mathf.Infinity;
            
            for (var index = 0; index < _doorControllers.Count; index++)
            {
                var distance = Vector3.Distance(position, _doorControllers[index].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestDoor = index;
                }
            }

            return closestDoor;

        }

        public Vector3 GetWallSegmentPosition(int index)
        {
            return _wallSegments[index].transform.position; 
        }
            
        public bool IsWalkable(Vector3 position)
        {
            return _wallSegments[GetClosestWallSegmentDirect(position)].isScaffoldingIntact;
        }

        private void TrySpawnSoldier()
        {
            if (_requestedSoldierPositions.Count <= 0) return;
            if (!(_spawnTimer <= 0)) return;
            SpawnSoldier(_requestedSoldierPositions.Dequeue());
            _spawnTimer = spawnDelay; // Reset the spawn timer
            print("Dequeued and spawned; remaining: " + _requestedSoldierPositions.Count);
        }

        
        private void SpawnSoldier(WallSegment segment)
        {
            var door = GetClosestDoor(segment.transform.position);
            var soldier = _availableSoldiers.Dequeue();
            soldier.transform.position = _doorControllers[door].spawnpoint.transform.position;
            soldier.gameObject.SetActive(true);
            print(soldier.isActiveAndEnabled);
            _doorControllers[door].Open();
            soldier.MoveTo(segment);
        }

        public void RecycleSoldier(FriendlySoldier soldier)
        {
            soldier.transform.SetParent(soldiersParent);
            _availableSoldiers.Enqueue(soldier);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(_closestGizmo, 0.5f);
        }

        public void Interact(ThirdPersonController player)
        {
            switch (_selection)
            {
                case Selection.OverheadScaffolding:
                    if (player.CanRepairWood())
                    {
                        DamageScaffoldingSegment(_closestSegment);
                        player.IncrementWood(-1);
                    }

                    break;
                case Selection.Wall:
                    if (player.CanRepairStone())
                    {
                        RepairWallSegment(_closestSegment);
                        player.IncrementStone(-1);
                    }

                    break;
                case Selection.Scaffolding:
                    if (player.CanRepairWood())
                    {
                        DamageScaffoldingSegment(_closestSegment);
                        player.IncrementWood(-1);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public void RequestSoldier(WallSegment segment)
        {
            if (segment.soldierRequested || segment.isSoldierPresent) return;
            _requestedSoldierPositions.Enqueue(segment);
            print("Requested; remaining: " + _requestedSoldierPositions.Count);
        }

        private void DamageRandomSegment()
        {
            DamageWallSegment(_wallSegments[Random.Range(0, 4)]);
        }

        public bool RepairWallSegment(WallSegment segment)
        {
            if (segment != null && segment.wallPiece.activeSelf)
            {
                return segment.RepairWall();
            }

            return false;
        }

        public void DamageWallSegment(WallSegment segment)
        {
            if (segment != null && segment.wallPiece.activeSelf)
            {
                segment.DamageWall();
            }
        }

        public bool RepairScaffoldingSegment(WallSegment segment)
        {
            if (segment != null && segment.scaffoldingPiece.activeSelf)
            {
                return segment.RepairScaffolding();
            }

            return false;
        }

        public void DamageScaffoldingSegment(WallSegment segment)
        {
            if (segment != null && segment.scaffoldingPiece.activeSelf)
            {
                segment.DamageScaffolding();
            }
        }
        public void RepairWallSegment(int index) 
        {
            if (_wallSegments.Count > index)
            {
                RepairWallSegment(_wallSegments[index]);
            }
        }

        public void DamageWallSegment(int index)
        {
            if (_wallSegments.Count > index)
            {
                DamageWallSegment(_wallSegments[index]);
            }
            
        }

        public void RepairScaffoldingSegment(int index)
        {
            if (_wallSegments.Count > index)
            {
                RepairScaffoldingSegment(_wallSegments[index]);
            }
            
        }

        public void DamageScaffoldingSegment(int index)
        {
            if (_wallSegments.Count > index)
            {
                DamageScaffoldingSegment(_wallSegments[index]);
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