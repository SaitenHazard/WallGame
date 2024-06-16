using System;
using System.Collections.Generic;
using AnimationCotrollers;
using Input;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Wall
{
    public class WallManager : MonoBehaviour
    {
        public enum Selection
        {
            None,
            Left,
            Right,
            Up
        }

        public enum HighlightingMode
        {
            Always,
            OnAdjacent,
            Never
        }

        public HighlightingMode highlightingMode;

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
        public ThirdPersonController player;
        public int width = 5;

        private float _spawnTimer;
        [SerializeField]
        private List<WallSegment> _wallSegments; // Only Serializable for debug purposes!!!
        private List<DoorAnimationController> _doorControllers;
        private Queue<FriendlySoldier> _availableSoldiers;
        private Transform _playerTransform;
        private bool _nearWall;
        private WallSegment _closestSegment;
        private WallSegment _previousClosestSegment;
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

            _requestedSoldierPositions = new Queue<WallSegment>();
            _availableSoldiers = new Queue<FriendlySoldier>();
        }

        public List<WallSegment> GetWallSegments()
        {
            return _wallSegments;
        }

        private void Start()
        {
            Inputs.Select += SetSelection;
            Inputs.RepairWood += RepairScaffoldingSegment;
            Inputs.RepairStone += RepairWallSegment;
            EventManager.OnWallPieceHit += DamageWallSegment;
            EventManager.OnScaffoldingHit += DamageScaffoldingSegment;
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
            _playerTransform = player.transform;
            if (_playerTransform == null) throw new MissingFieldException("The player transform is needed ");
            InitializeWallSegments();
            InitializeDoorControllers();

            for (var i = 0; i < soldierBuffer; i++)
            {
                var soldier = Instantiate(soldierPrefab, soldiersParent);
                soldier.gameObject.SetActive(false);
                _availableSoldiers.Enqueue(soldier);
            }
            // InvokeRepeating(nameof(DamageRandomSegment), 0f, 2f);
        }

        private void OnDestroy()
        {
            Inputs.Select -= SetSelection;
            Inputs.RepairWood -= RepairScaffoldingSegment;
            Inputs.RepairStone -= RepairWallSegment;
            EventManager.OnWallPieceHit -= DamageWallSegment;
            EventManager.OnScaffoldingHit -= DamageScaffoldingSegment;
        }

        private void Update()
        {
            if (highlightingMode == HighlightingMode.Always && _nearWall)
            {
                var closestSegmentDirect = GetClosestSegmentDirect(_playerTransform.position);
                var indexSelection = IndexToSelection(closestSegmentDirect);
                _closestSegment = indexSelection == -1 || !_wallSegments[indexSelection].WallDamaged() ? _wallSegments[closestSegmentDirect] : _wallSegments[indexSelection];

                if (!_previousClosestSegment || !_previousClosestSegment.Equals(_closestSegment))
                {
                    if (_previousClosestSegment)
                    {
                        _previousClosestSegment.SetPreview(false);
                    }
                    _closestSegment.SetPreview(true);
                    _previousClosestSegment = _closestSegment;
                }
            }

            TrySpawnSoldier();
            _spawnTimer -= Time.deltaTime;
        }

        private void SetSelection(Vector2 value)
        {
            _selection = (value.x, value.y) switch
            {
                (-1, 0) => Selection.Left,
                (1, 0) => Selection.Right,
                (0, 1) => Selection.Up,
                _ => Selection.None
            };
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

        private int GetClosestSegmentSelection(Vector3 position)
        {
            return IndexToSelection(GetClosestSegmentDirect(position));
            
        }

        private int IndexToSelection(int index)
        {
            return _selection switch
            {
                Selection.Left => index - 1 < 0 || (index - 1) % width > index % width ? -1 : index - 1,
                Selection.Right => index + 1 > _wallSegments.Count || (index + 1) % width < index % width ? -1 : index + 1,
                Selection.Up => index + width > _wallSegments.Count ? -1 : index + width,
                Selection.None => index,
                _ => index
            };
        }

        public int GetClosestSegmentDirect(Vector3 position)
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
            return _wallSegments[GetClosestSegmentDirect(position)].isScaffoldingIntact;
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
            _doorControllers[door].Open();
            soldier.MoveTo(segment);
        }

        public void RecycleSoldier(FriendlySoldier soldier)
        {
            soldier.transform.SetParent(soldiersParent);
            _availableSoldiers.Enqueue(soldier);
        }


        public void RequestSoldier(WallSegment segment)
        {
            if (segment.soldierRequested || segment.isSoldierPresent) return;
            _requestedSoldierPositions.Enqueue(segment);
            print("Requested; remaining: " + _requestedSoldierPositions.Count);
        }

        private void DamageRandomSegment()
        {
            if (Random.Range(0, 2) == 0) DamageScaffoldingSegment(_wallSegments[Random.Range(0, 9)]);
            else DamageWallSegment(_wallSegments[Random.Range(0, 9)]);
        }

        // If Closest Segment is not updated every frame, replace with _playerTransform.position
        private void RepairWallSegment()
        {
            var closest = GetClosestSegmentSelection(_playerTransform.position);
            if (closest == -1) print("Can't repair that");
            else RepairWallSegment(closest);
        }

        private bool RepairWallSegment(int index)
        {
            return RepairWallSegment(_wallSegments[index]);
        }

        private bool RepairWallSegment(WallSegment segment)
        {
            if (segment == null || !player.CanRepairStone()) return false;
            if (!segment.RepairWall()) return false;
            EventManager.RaiseOnRepairedStone();
            return true;
        }

        private void DamageWallSegment(WallSegment segment)
        {
            if (segment != null && segment.wallPiece.activeSelf)
            {
                segment.DamageWall();
            }
        }

        private void DamageWallSegment(int index)
        {
            if (_wallSegments.Count > index)
            {
                DamageWallSegment(_wallSegments[index]);
            }
        }

        private void RepairScaffoldingSegment()
        {
            var closest = GetClosestSegmentSelection(_playerTransform.position);
            if (closest == -1) print("Can't repair that");
            else print(RepairScaffoldingSegment(closest));
        }

        private bool RepairScaffoldingSegment(int index)
        {
            return RepairScaffoldingSegment(_wallSegments[index]);
        }

        private bool RepairScaffoldingSegment(WallSegment segment)
        {
            if (segment == null || !player.CanRepairWood()) return false;
            if (!segment.RepairScaffolding()) return false;
            EventManager.RaiseOnRepairedWood();
            return true;
        }

        public void DamageScaffoldingSegment(WallSegment segment)
        {
            if (segment != null)
            {
                segment.DamageScaffolding();
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
                _previousClosestSegment.SetPreview(false);
                _previousClosestSegment = null;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(_closestGizmo, 0.5f);
        }
    }
}