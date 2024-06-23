using UnityEngine;

namespace AnimationControllers
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [Range(0.0f, 1.0f)] public float speed;

        public bool falling;
        private float _airTime;
        private int _animIDAirTime;
        private int _animIDFalling;
        private int _animIDJump;
        private int _animIDSpeed;
        private int _animIDEnterCatapult;
        private Animator _anim;

        public bool Locked { get; private set; }

        // Start is called before the first frame update
        private void Start()
        {
            _anim = GetComponent<Animator>();
            AssignAnimationIDs();
        }

        // Update is called once per frame
        private void Update()
        {
            if (falling) _airTime += Time.deltaTime;
        }

        private void AssignAnimationIDs()
        {
            _animIDJump = Animator.StringToHash("Jump");
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDFalling = Animator.StringToHash("Falling");
            _animIDAirTime = Animator.StringToHash("AirTime");
            _animIDEnterCatapult = Animator.StringToHash("EnterCatapult");
        }

        public void AnimEvent_DoneLanding()
        {
            _airTime = 0;
            AnimEvent_DoneAnimating();
        }

        public void Launching(bool launching)
        {
            _anim.SetTrigger(launching ? "Launching" : "DoneLaunching");
        }

        public void AnimEvent_DontInterrupt()
        {
            Locked = true;
        }

        public void EnterCatap()
        {
            _anim.SetTrigger(_animIDEnterCatapult);
        }

        private void AnimEvent_DoneAnimating()
        {
            Locked = false;
        }

        public void SetJump()
        {
            _anim.SetTrigger(_animIDJump);
        }

        public void SetSpeed(float value)
        {
            speed = value;
            _anim.SetFloat(_animIDSpeed, value);
        }

        public void SetFalling(bool value)
        {
            falling = value;
            _anim.SetBool(_animIDFalling, value);
        }

        public void SetAirTime(float value)
        {
            _airTime = value;
            _anim.SetFloat(_animIDAirTime, value);
        }
    }
}