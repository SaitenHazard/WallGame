using UnityEngine;

namespace AnimationControllers
{
    [RequireComponent(typeof(Animator))]
    public class DoorAnimationController : MonoBehaviour
    {
        public GameObject spawnpoint;
        private Animator _anim;

        private float _progress;

        public void Start()
        {
            _anim = GetComponent<Animator>();
        }

        public void Open()
        {
            _progress = _anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (_anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Tür Armature|DoorClose")
                _anim.Play("DoorOpen", 0, 1 - _progress);
            else
                _anim.SetTrigger(Animator.StringToHash("Open"));
        }
    }
}