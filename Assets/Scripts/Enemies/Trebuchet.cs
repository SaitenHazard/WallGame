using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Animator))]
    public class Trebuchet : MonoBehaviour
    {
        
        [Range(0.01f, 2f)]
        public float _reloadSpeed = 1;

        public ProjectileSettings _projectileSettings;

        public Transform releasePoint;

        public Transform targetPoint;

        private TargetProjectile projectile;

        private Animator anim;

        public bool ready = true;

        private int _animIDReadyWait;
        private int _animIDReloadSpeed;
        private int _animIDDeath;

        private Vector3 lastTarget;
        private int lastSelectedID;

        void Start()
        {
            anim = GetComponent<Animator>();
        
            _animIDReadyWait = Animator.StringToHash("ReadySpeed");
            _animIDReloadSpeed = Animator.StringToHash("RewindSpeed");
            _animIDDeath = Animator.StringToHash("Death");

            anim.SetFloat(_animIDReadyWait, Random.Range(0.5f, 1.5f));
        }

        public void SetUp(ProjectileSettings projectileSettings, float reloadAnimSpeed)
        {
            _projectileSettings = projectileSettings;
            _reloadSpeed = reloadAnimSpeed;
            Invoke("SetReloadSpeed", 0.1f);
        }

        public void SetFlightTime(float flightTime)
        {
            _projectileSettings.flightTime = flightTime;
        }

        public void SetParabolaHeight(float parabolaHeight)
        {
            _projectileSettings.parabolaHeight = parabolaHeight;
        }

        public void Kill()
        {
            anim.SetTrigger(_animIDDeath);
        }

        private void SetReloadSpeed()
        {
            anim.SetFloat(_animIDReloadSpeed, _reloadSpeed);
        }

        public void SetSelection(Vector3 target, int index)
        {
            lastTarget = target;
            lastSelectedID = index;
        }

        public void AnimEvent_DoneReloading()
        {
            ready = true;
        }
        
        public bool Launch()
        {
            if (lastSelectedID == -1 || lastTarget == Vector3.zero)
            {
                Debug.LogWarning("A Trebuchet was instructed to launch without a call to SetSelection first! This should not happen! Check Behaviour in ArmyController.cs!");
                return false;
            } else { 
                anim.SetTrigger("Launch");
                ready = false;
                return true;
            }
        }

        public void AnimEvent_Launch()
        {
            TargetProjectile projectile = Instantiate(_projectileSettings.prefab, releasePoint.position, Quaternion.identity);
            
            projectile.SetDestination(lastTarget);

            projectile.SetSettings(_projectileSettings);
            Invoke("WallPieceHit", _projectileSettings.flightTime);
        }

        private void WallPieceHit()
        {
            //Debug.Log("Wall piece " + lastSelected + " hit!");
            EventManager.RaiseOnWallPieceHit(lastSelectedID);
            lastTarget = Vector3.zero;
            lastSelectedID = -1;
        }
    }
}