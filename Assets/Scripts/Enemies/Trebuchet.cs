using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    [RequireComponent(typeof(Animator))]
    public class Trebuchet : MonoBehaviour
    {
        [Range(0.01f, 2f)] public float reloadSpeed = 1;

        public ProjectileSettings projectileSettings;

        public Transform releasePoint;

        public Transform targetPoint;

        public bool ready = true;
        private int _animIDDeath;

        private int _animIDReadyWait;
        private int _animIDReloadSpeed;
        private int _animIDLaunch;

        private Animator _anim;
        private int _lastSelectedID;

        private Vector3 _lastTarget;

        private TargetProjectile _projectile;

        private void Start()
        {
            _anim = GetComponent<Animator>();

            _animIDReadyWait = Animator.StringToHash("ReadySpeed");
            _animIDReloadSpeed = Animator.StringToHash("RewindSpeed");
            _animIDDeath = Animator.StringToHash("Death");
            _animIDLaunch= Animator.StringToHash("Launch");
            

            _anim.SetFloat(_animIDReadyWait, Random.Range(0.5f, 1.5f));
        }

        public void SetUp(ProjectileSettings projectileSettings, float reloadAnimSpeed)
        {
            this.projectileSettings = projectileSettings;
            reloadSpeed = reloadAnimSpeed;
            Invoke(nameof(SetReloadSpeed), 0.1f);
        }

        public void SetFlightTime(float flightTime)
        {
            projectileSettings.flightTime = flightTime;
        }

        public void SetParabolaHeight(float parabolaHeight)
        {
            projectileSettings.parabolaHeight = parabolaHeight;
        }

        public void Kill()
        {
            _anim.SetTrigger(_animIDDeath);
        }

        private void SetReloadSpeed()
        {
            _anim.SetFloat(_animIDReloadSpeed, reloadSpeed);
        }

        public void SetSelection(Vector3 target, int index)
        {
            _lastTarget = target;
            _lastSelectedID = index;
        }

        public void AnimEvent_DoneReloading()
        {
            ready = true;
        }

        public bool Launch()
        {
            if (_lastSelectedID == -1 || _lastTarget == Vector3.zero)
            {
                Debug.LogWarning(
                    "A Trebuchet was instructed to launch without a call to SetSelection first! This should not happen! Check Behaviour in ArmyController.cs!");
                return false;
            }

            _anim.SetTrigger(_animIDLaunch);
            ready = false;
            return true;
        }

        public void AnimEvent_Launch()
        {
            var projectile = Instantiate(projectileSettings.prefab, releasePoint.position, Quaternion.identity);

            projectile.SetDestination(_lastTarget);

            projectile.SetSettings(projectileSettings);
            Invoke(nameof(WallPieceHit), projectileSettings.flightTime);
        }

        private void WallPieceHit()
        {
            //Debug.Log("Wall piece " + lastSelected + " hit!");
            EventManager.RaiseOnWallPieceHit(_lastSelectedID);
            _lastTarget = Vector3.zero;
            _lastSelectedID = -1;
        }
    }
}