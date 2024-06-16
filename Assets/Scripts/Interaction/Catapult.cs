using System.Collections.Generic;

using Player;

using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(Animator))]
    public class Catapult : MonoBehaviour, IInteractable
    {
        [Header("AimingSpeed")]
        [SerializeField]
        [Tooltip("How many degrees per second can you rotate the Mangonel")]
        private float rotSpeed;

        [SerializeField]
        [Tooltip("How many meters per second can you change the Mangonel vertically")]
        private float aimUpDownSpeed;

        [Header("Important Transforms")]
        public Transform dropInPoint;
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
    

        private bool _ready = false;

        private bool _inCatapult = false;

        private Animator _anim;

        private LineRenderer _lineRenderer;

        private bool _currentlyAiming;

        public float MagicNum = -0.011f;

        public int bezierSmoothness = 5;

        private Vector3 _startPoint = Vector3.zero;
        private Vector3 _endPoint = Vector3.zero;

        // Setting Interpolation Points
        private Vector3 _startHandle = Vector3.zero;
        private Vector3 _endHandle = Vector3.zero;

        public float desiredAltitudeChange = 0;
        public float contAltitude = 3f;
        public float desiredRotationChange = 0;

        private void Start()
        {
            // Why is the Catapult subscribing to the event it invokes itself?
            _anim = GetComponent<Animator>();
            _lineRenderer = GetComponentInChildren<LineRenderer>();
            EventManager.OnEnterCatapult += StartAiming;
            EventManager.OnExitCatapult += StopAiming;
        }

        private void OnDestroy()
        {
            EventManager.OnEnterCatapult -= StartAiming;
            EventManager.OnExitCatapult -= StopAiming;
        }

        public void Interact(ThirdPersonController interactor)

        {
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
                } else
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

        // Update is called once per frame


        private void Update()
        {
            if (!_currentlyAiming) return;

        
        
            { // Vertical Aiming
                contAltitude += desiredAltitudeChange * Time.deltaTime * aimUpDownSpeed;
                contAltitude = Mathf.Clamp(contAltitude, firstRowY, firstRowY+(wallDepths.Count-1)*rowHeight);
                targetPos.position = new Vector3(targetPos.position.x, 
                    Mathf.Round(contAltitude / rowHeight) * rowHeight + vertOffset, 
                    targetPos.position.z);

            }
            var aimingAtRow = Mathf.RoundToInt((contAltitude - firstRowY) / rowHeight);
            // Debug.Log("Aiming at Row " + aimingAtRow);


            if (desiredRotationChange != 0)
            {
                //Debug.Log("Rotating by " + (targetRotation * Time.deltaTime));
            
                transform.Rotate(new Vector3(0, rotSpeed * desiredRotationChange * Time.deltaTime, 0));
                var unclampedRot = transform.rotation.eulerAngles.y;
                if (unclampedRot > 180) unclampedRot -= 360;
            
            
                unclampedRot = Mathf.Clamp(unclampedRot, -60, 60);
                transform.rotation = Quaternion.Euler(0, unclampedRot, 0);
            }
            var alpha = transform.rotation.eulerAngles.y;
            if (alpha > 180) alpha -= 360;
            targetPos.position = new Vector3(Mathf.Tan(alpha*MagicNum) * wallDepths[aimingAtRow] , targetPos.position.y, wallDepths[aimingAtRow]);
            _lineRenderer.SetPositions(new Vector3[] { dropInPoint.position, targetPos.position });
            SmoothBezier();
        }

        private void OnDrawGizmosSelected()
        {
            var darkRedColor = new Color(0.5f, 0.1f, 0.1f, 0.8f);
            Gizmos.color = darkRedColor;
            Gizmos.DrawLine(_startPoint, _startHandle);
            Gizmos.DrawLine(_endPoint, _endHandle);
        }

        private void SmoothBezier()
        {
            _startPoint = _lineRenderer.GetPosition(0)+new Vector3(0,1.6f,0);
            _endPoint = _lineRenderer.GetPosition(1);

            // Setting Interpolation Points
            _startHandle = _startPoint + new Vector3(0, 1, 0) * _endPoint.y;
            _endHandle = _startHandle;

            _lineRenderer.positionCount = bezierSmoothness + 1;
            for (var i = 0; i <= bezierSmoothness; i++)
            {
                var t = (float)i / (float)bezierSmoothness;
                _lineRenderer.SetPosition(i, Vector3.Lerp(Vector3.Lerp(_startPoint, _startHandle, t), Vector3.Lerp(_endHandle, _endPoint, t), t));
            }

        }

        public void Aim(Vector2 input)
        {
            //Debug.Log("Aiming!" + input);
            var clamped = Mathf.Abs(input.x) > 0.1 ? input.x : 0;
            _anim.SetFloat("Rotation", -1 * clamped);
            print(input);
            desiredRotationChange = clamped;
            desiredAltitudeChange = input.y;
        }

        public void ActionEvent_FullyRewound()
        {
            Debug.Log("Catapult ready");
            _ready = true;
        }

        public void OnFire(Vector3[] path, int vertexCount)
        {
            StopAiming(transform);
            _anim.SetTrigger("Shoot");
        }
    }
}
