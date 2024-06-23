using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interaction
{
    [RequireComponent(typeof(Animator))]
    public class Catapult : MonoBehaviour, IInteractable
    {
        [Header("AimingSpeed")] [SerializeField] [Tooltip("How many degrees per second can you rotate the Mangonel")]
        private float rotSpeed;

        [SerializeField] [Tooltip("How many meters per second can you change the Mangonel vertically")]
        private float aimUpDownSpeed;

        [Header("Important Transforms")] public Transform dropInPoint;

        public Transform dropOffPoint;
        public Transform targetPos;

        [Header("Wall Configuration")]
        [SerializeField]
        [Tooltip("How far the mangonel will shoot wallther on the z axis depending on the row")]
        public List<float> wallDepths;

        [Tooltip("The height of the lowest Row Wallther can be fired onto.")]
        public float firstRowY = 3.0f;

        [Tooltip("The unit height of each row.")]
        public float rowHeight = 1.5f;

        [Tooltip("The vertical offset of the target positions from the floor of the row")]
        public float vertOffset = 1.2f;

        public float magicNum = -0.011f;

        public int bezierSmoothness = 5;

        public float desiredAltitudeChange;
        public float contAltitude = 3f;
        public float desiredRotationChange;

        private Animator _anim;
        private int _animIDShoot;

        private bool _currentlyAiming;
        private Vector3 _endHandle = Vector3.zero;
        private Vector3 _endPoint = Vector3.zero;

        private bool _inCatapult;

        private LineRenderer _lineRenderer;


        private bool _ready;

        // Setting Interpolation Points
        private Vector3 _startHandle = Vector3.zero;

        private Vector3 _startPoint = Vector3.zero;

        private void Start()
        {
            // Why is the Catapult subscribing to the event it invokes itself?
            targetPos.parent = null;
            _anim = GetComponent<Animator>();
            _lineRenderer = GetComponentInChildren<LineRenderer>();
            EventManager.OnEnterCatapult += StartAiming;
            EventManager.OnExitCatapult += StopAiming;
            EventManager.OnCatapultFire += OnFire;
            _animIDShoot = Animator.StringToHash("Shoot");
        }

        // Update is called once per frame


        private void Update()
        {
            if (!_currentlyAiming) return;

            {
                // Vertical Aiming
                contAltitude += desiredAltitudeChange * Time.deltaTime * aimUpDownSpeed;
                contAltitude = Mathf.Clamp(contAltitude, firstRowY, firstRowY + (wallDepths.Count - 1) * rowHeight);
                targetPos.position = new Vector3(
                    targetPos.position.x,
                    Mathf.Round(contAltitude / rowHeight) * rowHeight + vertOffset,
                    targetPos.position.z);
            }
            var aimingAtRow = Mathf.RoundToInt((contAltitude - firstRowY) / rowHeight);
            // Debug.Log("Aiming at Row " + aimingAtRow);


            if (desiredRotationChange != 0)
            {
                targetPos.position = new Vector3(
                    targetPos.position.x + rotSpeed * desiredRotationChange * Time.deltaTime,
                    targetPos.position.y,
                    targetPos.position.z);
                transform.LookAt(new Vector3(targetPos.position.x, 0, targetPos.position.z));
            }

            targetPos.position = new Vector3(
                targetPos.position.x,
                targetPos.position.y,
                wallDepths[aimingAtRow]);

//            targetPos.position = new Vector3(Mathf.Tan(alpha*MagicNum) * wallDepths[aimingAtRow] , targetPos.position.y, wallDepths[aimingAtRow]);
            _lineRenderer.SetPositions(new[] { dropInPoint.position, targetPos.position });
            SmoothBezier();
        }

        private void OnDestroy()
        {
            EventManager.OnEnterCatapult -= StartAiming;
            EventManager.OnExitCatapult -= StopAiming;
            EventManager.OnCatapultFire -= OnFire;
        }

        private void OnDrawGizmosSelected()
        {
            var darkRedColor = new Color(0.5f, 0.1f, 0.1f, 0.8f);
            Gizmos.color = darkRedColor;
            Gizmos.DrawLine(_startPoint, _startHandle);
            Gizmos.DrawLine(_endPoint, _endHandle);
        }

        public void Interact(ThirdPersonController interactor)

        {
            Debug.Log("Interacting with Catapult...");
            if (_inCatapult)
            {
                print("Invoking");
                _inCatapult = false;
                _ready = false;
                var path = new Vector3[_lineRenderer.positionCount];
                _lineRenderer.GetPositions(path);

                EventManager.RaiseCatapultFire(path, _lineRenderer.positionCount);
            }
            else
            {
                if (_ready)
                {
                    _inCatapult = true;
                    EventManager.RaiseEnterCatapult(dropInPoint);
                }
                else
                {
                    Debug.Log("Catapult Not Ready Yet.");
                }
            }
        }

        private void StartAiming(Transform t)
        {
            Debug.Log("StartAiming");
            _currentlyAiming = true;
            _lineRenderer.enabled = true;
        }

        private void StopAiming(Transform t)
        {
            Debug.Log("StopAiming");
            _currentlyAiming = false;
            _lineRenderer.enabled = false;
        }

        private void SmoothBezier()
        {
            _startPoint = _lineRenderer.GetPosition(0) + new Vector3(0, 1.6f, 0);
            _endPoint = _lineRenderer.GetPosition(1);

            // Setting Interpolation Points
            _startHandle = _startPoint + new Vector3(0, 1, 0) * _endPoint.y;
            _endHandle = _startHandle;

            _lineRenderer.positionCount = bezierSmoothness + 1;
            for (var i = 0; i <= bezierSmoothness; i++)
            {
                var t = i / (float)bezierSmoothness;
                _lineRenderer.SetPosition(i,
                    Vector3.Lerp(Vector3.Lerp(_startPoint, _startHandle, t), Vector3.Lerp(_endHandle, _endPoint, t),
                        t));
            }
        }

        public void Aim(Vector2 input)
        {
            desiredRotationChange = input.x;
            desiredAltitudeChange = Mathf.Abs(input.y) < 0.2f ? 0 : input.y;
        }

        public void ActionEvent_FullyRewound()
        {
            Debug.Log("Catapult ready");
            _ready = true;
        }

        public void OnFire(Vector3[] path, int vertexCount)
        {
            Debug.Log("Firing!");
            StopAiming(transform);
            _anim.SetTrigger(_animIDShoot);
        }
    }
}