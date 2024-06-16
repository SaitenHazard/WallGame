using System;
using System.Collections.Generic;
using Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Wall
{
    public class WallSegment : MonoBehaviour
    {
        public GameObject wallPiece;
        public GameObject scaffoldingPiece;
        public FriendlySoldier soldier;

        public int maxHealth = 3;
        public int health;
        public bool isScaffoldingIntact = true;
        public bool soldierRequested = false;
        public bool isSoldierPresent;
        public Mesh normalWall;
        public Mesh chippedWall;
        public Mesh damagedWall;
        public Mesh destroyedWall;
        public int level; //To be used to request soldier at the correct level
        public bool chosenOne;
        public Material translucent;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Material _wallMaterial;

        public WallSegmentCriticalEvent onWallSegmentCritical = new WallSegmentCriticalEvent();
        public WallSegmentNotCriticalEvent onWallNotSegmentCritical = new WallSegmentNotCriticalEvent();


        private void Start()
        {
            _meshFilter = GetComponentInChildren<MeshFilter>();
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _wallMaterial = _meshRenderer.material;
            normalWall = _meshFilter.mesh;

            health = maxHealth;
            // UpdateSoldierState();
        }

        /////////////////for Debug Only

        //bool ciritcalInvooked = false;
        //private void Update()
        //{
        //    if (health == 0 && ciritcalInvooked == false)
        //    {
        //        ciritcalInvooked = true;
        //        onWallSegmentCritical.Invoke(this);
        //    }
        //    if (health > 0 && ciritcalInvooked == true)
        //    {
        //        ciritcalInvooked = false;
        //        onWallNotSegmentCritical.Invoke(this);
        //    }
        //}
        /// /////////////////////
        /// </summary>
   

        public bool WallDamaged()
        {
            return health < maxHealth;
        }
        public bool ScaffoldingDamaged()
        {
            return isScaffoldingIntact;
        }

        public bool SetPreview(bool enabled)
        {
            if (enabled && health < maxHealth)
            {
                ChangeWallState(health + 1);
                _meshRenderer.material = translucent;
                return true;
            }
            else
            {
                ChangeWallState(health);
                _meshRenderer.material = _wallMaterial;
                return false;
            }
        }

        private void ChangeWallState(int state)
        {
            _meshFilter.mesh = state switch
            {
                0 => destroyedWall,
                1 => damagedWall,
                2 => chippedWall,
                3 => normalWall,
                _ => _meshFilter.mesh
            };
        }

        public void DamageScaffolding()
        {
            print("TEST");
            isScaffoldingIntact = false;
            scaffoldingPiece.SetActive(false);
            if (isSoldierPresent)
            {
                soldier.Die();
                isSoldierPresent = false;
            }
        }

        public bool RepairScaffolding()
        {
            if (isScaffoldingIntact) return false;
            isScaffoldingIntact = true;
            scaffoldingPiece.SetActive(true);
            RequestSoldier();
            return true;
        }

        public void DamageWall()
        {
            health -= 1;
            if (health < 0)
            {
                EventManager.RaiseGameOver();
                return;
            }
            ChangeWallState(health);
            if (isSoldierPresent)
            {
                soldier.Die();
                isSoldierPresent = false;
            }
            if(health == 0)
            {
                onWallSegmentCritical.Invoke(this);
            }
        }

        public bool RepairWall()
        {
            if (health == maxHealth) return false;
            health = Mathf.Min(maxHealth, health + 1);
            ChangeWallState(health);
            RequestSoldier();
            onWallNotSegmentCritical.Invoke(this);
            return true;
        }

        private void RequestSoldier()
        {
            if (health == maxHealth && isScaffoldingIntact && !soldierRequested) WallManager.instance.RequestSoldier(this);
        }

        private GUIStyle _style = new GUIStyle();

        private void OnDrawGizmos()
        {
            _style.fontSize = 32;
            if (chosenOne) Handles.Label(transform.position + new Vector3(0, 3, 0), "Wall Health: " + health, _style);
        }

        public void AssignSoldier(FriendlySoldier incomingSoldier)
        {
            soldier = incomingSoldier;
            isSoldierPresent = true;
            soldier.transform.SetParent(transform);
        }

        public bool IsIntact()
        {
            return isScaffoldingIntact && health == maxHealth;
        }
    }
}
