using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Animator))]
    public class Trebuchet : MonoBehaviour
    {
        public float _flightTime = 2;
        [Range(0.01f, 2f)]
        public float _reloadSpeed = 1;
        public List<GameObject> wallPieces;

        public GameObject projectilePrefab;

        public Transform releasePoint;

        public Transform targetPoint;

        private TargetProjectile projectile;

        private Animator anim;

        private int _animIDReadyWait;
        private int _animIDReloadSpeed;

        void Start()
        {
            anim = GetComponent<Animator>();
        
            _animIDReadyWait = Animator.StringToHash("ReadySpeed");
            _animIDReloadSpeed = Animator.StringToHash("RewindSpeed");

            anim.SetFloat(_animIDReadyWait, Random.Range(0.5f, 1.5f));
        }

        public void SetUp(float flightTime, float reloadTime)
        {
            _flightTime = flightTime;
            _reloadSpeed = reloadTime;
            Invoke("SetReloadSpeed", 0.1f);
        }

        private void SetReloadSpeed()
        {
            anim.SetFloat(_animIDReloadSpeed, _reloadSpeed);
        }

        public void AnimEvent_DoneReloading()
        {
            // nop
        }
        
        public void AnimEvent_Launch()
        {
            print("Launching");
            GameObject gO = Instantiate(projectilePrefab, releasePoint.position, Quaternion.identity);
            projectile = gO.GetComponent<TargetProjectile>();
            lastSelected = SelectWallPiece();
            if (wallPieces == null || wallPieces.Count == 0)
            {
                Debug.LogWarning("WallPieces Not Set! Launching towards world origin...");
                projectile.SetDestination(Vector3.zero);
            } else
            {
                projectile.SetDestination(wallPieces[lastSelected].transform.position);
            }
            
            projectile.SetFlightTime(_flightTime);
            Invoke("WallPieceHit", _flightTime);
        }

        private void WallPieceHit()
        {
            Debug.Log("Wall piece " + lastSelected + " hit!");
            EventManager.RaiseOnWallPieceHit(lastSelected);
        }

        int lastSelected = -1;

        private int SelectWallPiece()
        {
            return Random.Range(0, wallPieces.Count);
        }
    }
}