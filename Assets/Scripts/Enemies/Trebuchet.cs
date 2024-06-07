using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Animator))]
    public class Trebuchet : MonoBehaviour
    {
        public float flightTime = 1;
        public List<GameObject> wallPieces;

        public GameObject projectilePrefab;

        public Transform releasePoint;

        public Transform targetPoint;

        private TargetProjectile projectile;

        private Animator anim;

        private int _animIDReady;
        private int _animIDRewindSpeed;

        public int TrebuchetID = 0;

        private float[] starts = { 0, 0.5f, 1, 1.5f, 2, 2.5f, 3, 3.5f, 4 };

        void Start()
        {
            anim = GetComponent<Animator>();
            _animIDReady = Animator.StringToHash("Ready");
            _animIDRewindSpeed = Animator.StringToHash("RewindSpeed");

            Ready();
            anim.SetFloat(_animIDRewindSpeed, Random.Range(0.5f, 1.5f));
        }

        private void Ready()
        {
            anim.SetTrigger(_animIDReady);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void AnimEvent_DoneReloading()
        {
            anim.SetFloat(_animIDRewindSpeed, Random.Range(0.8f, 1.2f));
        }

        public void AnimEvent_Launch()
        {
            GameObject gO = Instantiate(projectilePrefab, releasePoint.position, Quaternion.identity);
            projectile = gO.GetComponent<TargetProjectile>();
            lastSelected = SelectWallPiece();
            projectile.SetDestination(wallPieces[lastSelected].transform.position);
            projectile.SetFlightTime(flightTime);
            Invoke("WallPieceHit", flightTime);
        }

        private void WallPieceHit()
        {
            Debug.Log("Wall piece " + lastSelected + " hit!");
        }

        int lastSelected = -1;

        private int SelectWallPiece()
        {
            return Random.Range(0, wallPieces.Count);
        }
    }
}
