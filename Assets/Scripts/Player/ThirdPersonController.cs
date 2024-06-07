using System.Collections;
using AnimationCotrollers;
using Input;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class ThirdPersonController : MonoBehaviour
    {
        [FormerlySerializedAs("MoveSpeed")] [Tooltip("Move speed of the character in m/s")]
        public float moveSpeed = 2.0f;

        [FormerlySerializedAs("SprintSpeed")] [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed = 5.335f;

        [FormerlySerializedAs("RotationSmoothTime")] [Tooltip("How fast the character turns to face movement direction")] [Range(0.0f, 0.3f)]
        public float rotationSmoothTime = 0.12f;

        [FormerlySerializedAs("SpeedChangeRate")] [Tooltip("Acceleration and deceleration")]
        public float speedChangeRate = 10.0f;

        [FormerlySerializedAs("JumpHeight")] [Space(10)] [Tooltip("The height the player can jump")]
        public float jumpHeight = 1.2f;

        [FormerlySerializedAs("Gravity")] [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float gravity = -15.0f;

        [FormerlySerializedAs("JumpTimeout")] [Space(10)] [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float jumpTimeout = 0.1f;

        [FormerlySerializedAs("FallTimeout")] [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float fallTimeout = 0.15f;

        [FormerlySerializedAs("Grounded")]
        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool grounded = true;

        [FormerlySerializedAs("GroundedOffset")] [Tooltip("Useful for rough ground")] 
        public float groundedOffset = -0.14f;

        [FormerlySerializedAs("GroundedRadius")] [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float groundedRadius = 0.28f;

        [FormerlySerializedAs("GroundLayers")] [Tooltip("What layers the character uses as ground")]
        public LayerMask groundLayers;

        [FormerlySerializedAs("CinemachineCameraTarget")]
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject cinemachineCameraTarget;

        [FormerlySerializedAs("TopClamp")] [Tooltip("How far in degrees can you move the camera up")]
        public float topClamp = 70.0f;

        [FormerlySerializedAs("BottomClamp")] [Tooltip("How far in degrees can you move the camera down")]
        public float bottomClamp = -30.0f;

        [FormerlySerializedAs("CameraAngleOverride")] [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
        public float cameraAngleOverride;

        [FormerlySerializedAs("LockCameraPosition")] [Tooltip("For locking the camera position on all axis")]
        public bool lockCameraPosition = true;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private readonly float _terminalVelocity = 53.0f;

        // timeout
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private PlayerInput _playerInput;
        private GameObject _walther;
        private CharacterController _controller;
        private Inputs _input;
    
        private GameObject _mainCamera;

        private PlayerAnimationController _animController;
        private bool _hasAnimController;

        private enum PlayerState
        {
            Normal,
            InCatapult,
            Launched,
            Stunned
        }

        private PlayerState _currentState = PlayerState.Normal;

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            EventManager.OnEnterCatapult += EnterCatapult;
            EventManager.OnExitCatapult += ExitCatapult;
            EventManager.OnPlayerStunned += PauseMovement;
            EventManager.OnCatapultFire += GetLaunched;
            Inputs.Jump += Jump;

        }

        private void OnDestroy()
        {
            EventManager.OnEnterCatapult -= EnterCatapult;
            EventManager.OnExitCatapult -= ExitCatapult;
            EventManager.OnPlayerStunned -= PauseMovement;
            EventManager.OnCatapultFire -= GetLaunched;
            Inputs.Jump -= Jump;
        }

        private void Start()
        {
            _animController = GetComponentInChildren<PlayerAnimationController>();
            _hasAnimController = _animController != null;
            _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<Inputs>();
            _playerInput = GetComponent<PlayerInput>();

            AssignActionMapIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
        }

        private void Update()
        {
            if (_currentState == PlayerState.Launched)
            {
                Fly();
                return;
            }

            if (_currentState != PlayerState.Normal) return;
            GroundedCheck();
            Falling();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignActionMapIDs()
        {
            // TODO
        }
    

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
                transform.position.z);
            grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            // if (_hasAnimController)
            // {
            //     animator.SetBool(_animIDGrounded, !Grounded);
            // }
        }

        private void CameraRotation()
        {
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride,_cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;
            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error-prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * speedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error-prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    rotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimController)
            {
                // animator.SetFloat(_animIDSpeed, _animationBlend);
                // animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                _animController.SetSpeed(math.remap(0, 4, 0, 1, _speed));
            }
        }

        private void Falling()
        {
            if (grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = fallTimeout;

                // update animator if using character
                if (_hasAnimController)
                {
                    // animator.SetTrigger(_animIDJump);
                    // _animator.SetBool(_animIDFreeFall, false);
                    _animController.SetFalling(false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = jumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimController)
                    {
                        // _animator.SetBool(_animIDFreeFall, true);
                        _animController.SetFalling(true);
                    }
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        private void Jump()
        {
            if (_currentState == PlayerState.Normal && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                // update animator if using character
                if (_hasAnimController)
                {
                    _animController.SetJump();
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = grounded ? transparentGreen : transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
                groundedRadius);
        }

        private void EnterCatapult(Transform catapultBowl)
        {
            CancelVelocity();
            //Switch to Action map Catapult
            _playerInput.actions.FindActionMap("Catapult", true).Enable();
            _playerInput.actions.FindActionMap("Normal", true).Disable();
            _controller.enabled = false;
            _currentState = PlayerState.InCatapult;
            transform.position = catapultBowl.position;
            transform.parent = catapultBowl;
            transform.rotation = catapultBowl.parent.rotation;
            _animController.EnterCatap();
        }

        private void ExitCatapult(Transform dropOffPoint)
        {
            CancelVelocity();
            //Switch to action map Normal
            _playerInput.actions.FindActionMap("Normal", true).Enable();
            _playerInput.actions.FindActionMap("Catapult", true).Disable();
            _currentState = PlayerState.Normal;
            transform.position = dropOffPoint.position;
            transform.parent = null;
            _controller.enabled = true;
        }

        private float _launchTime;
        private Vector3[] _launchPath;
        private int _launchPathLength;
        public float launchDuration = 1.0f;
        private void GetLaunched(Vector3[] path, int vertexCount)
        {
            CancelVelocity();
            EventManager.RaisePlayerStunned(launchDuration);
            //Switch to action map Normal
            _launchTime = Time.time;
            _playerInput.actions.FindActionMap("Normal", true).Enable();
            _playerInput.actions.FindActionMap("Catapult", true).Disable();
            _currentState = PlayerState.Launched;
            _launchPath = path;
            _launchPathLength = vertexCount;
            Invoke(nameof(DoneLaunching), launchDuration);
            transform.parent = null;
            _controller.enabled = true;
            _animController.Launching(true);
        }

        private void Fly()
        {
            float currentProgress = (Time.time - _launchTime) / launchDuration;
            //The Mid-Air Movement is divided into y and xz because in a normal flying object, horizontal and vertical speed are also disconnected.

            Vector3 horiz = Vector3.Lerp(_launchPath[0], _launchPath[_launchPathLength - 1], currentProgress);

            Vector3 lastPoint = _launchPath[Mathf.FloorToInt(Mathf.Clamp(currentProgress * _launchPathLength, 0, _launchPathLength - 1))];
            Vector3 nextPoint = _launchPath[Mathf.CeilToInt(Mathf.Clamp(currentProgress * _launchPathLength, 0, _launchPathLength - 1))];
            float uber = currentProgress * _launchPathLength - Mathf.FloorToInt(currentProgress * _launchPathLength);
            transform.position = new Vector3(horiz.x, Mathf.Lerp(lastPoint.y, nextPoint.y, uber), horiz.z);
        }

        private void DoneLaunching()
        {
            _currentState = PlayerState.Normal;
            _controller.SimpleMove(_launchPath[_launchPathLength - 1] - _launchPath[_launchPathLength - 2]);
            _animController.Launching(false);
        }

        private void PauseMovement(float duration)
        {
            CancelVelocity();
            print(this);
            StartCoroutine(StunCoroutine(duration));
        }

        private void CancelVelocity()
        {
            _controller.SimpleMove(Vector3.zero);
            _speed = 0;
            _animController.SetSpeed(0);
        }

        private IEnumerator StunCoroutine(float duration)
        {
            _currentState = PlayerState.Stunned;
            yield return new WaitForSeconds(duration);
            _currentState = PlayerState.Normal;
        }
    }
}