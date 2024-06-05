using System;
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

        private List<WallSegment> _wallSegments;
        private Transform _playerTransform;
        private bool _nearWall;
        private static WallManager _instance;
        private WallSegment _closestSegment;
        private int _width = 5;
        private Vector3 _closestGizmo;
    


        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
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
        }

        private void OnDestroy()
        {
            Inputs.Select -= SetSelection;
        }

        private void Update()
        {
            if (!_nearWall)
            {
                return;
            }
            _closestSegment = _wallSegments[GetClosestWallSegment(_playerTransform.position)];
            _closestGizmo = _closestSegment.transform.position;
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
            _wallSegments.Clear();

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
    
    
        public int GetClosestWallSegment(Vector3 position)
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

            return _selection switch
            {
                Selection.OverheadScaffolding => closestSegment + 5,
                Selection.Wall => closestSegment,
                Selection.Scaffolding => closestSegment,
                _ => closestSegment
            };
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

        public void SetSoldierPresent(int index, bool present)
        {
            if (index >= 0 && index < _wallSegments.Count)
            {
                _wallSegments[index].SetSoldierPresent(present);
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