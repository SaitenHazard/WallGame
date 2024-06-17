using System;
using System.Collections;
using System.Collections.Generic;
using Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Wall
{
    public class WallSegment : MonoBehaviour
    {
        public GameObject wallPiece = null;
        public GameObject scaffoldingPiece = null;
        public FriendlySoldier soldier;

        public int wallMaxHealth = 3;
        public int wallHealth;
        public int scaffoldingMaxHealth = 2;
        public int scaffoldingHealth;
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
        private MeshRenderer _wallMeshRenderer;
        private MeshRenderer _scaffoldingMeshRenderer;
        private Material _wallMaterial;
        private Material _scaffoldingMaterial;

        public WallSegmentCriticalEvent onWallSegmentCritical = new WallSegmentCriticalEvent();
        public WallSegmentNotCriticalEvent onWallNotSegmentCritical = new WallSegmentNotCriticalEvent();


        private void Start()
        {
            _meshFilter = GetComponentInChildren<MeshFilter>();
            _wallMeshRenderer = wallPiece.GetComponentInChildren<MeshRenderer>();
            _wallMaterial = _wallMeshRenderer.material;
            normalWall = _meshFilter.mesh;
            if (scaffoldingPiece)
            {
                _scaffoldingMeshRenderer = scaffoldingPiece.GetComponentInChildren<MeshRenderer>();
                scaffoldingHealth = scaffoldingMaxHealth;
                _scaffoldingMaterial = _scaffoldingMeshRenderer.material;
            }

            wallHealth = wallMaxHealth;
            // UpdateSoldierState();
        }

        /////////////////for Debug Only

        bool ciritcalInvooked = false;

        public void Update()
        {
            if (wallHealth == 0 && !ciritcalInvooked)
            {
                //Debug.Log("Hello");
                ciritcalInvooked = true;
                onWallSegmentCritical.Invoke(this);
            }
            if (wallHealth > 0 && ciritcalInvooked)
            {
                //Debug.Log("It's me");
                ciritcalInvooked = false;
                onWallNotSegmentCritical.Invoke(this);
            }
        }
        // /////////////////////
        // </summary>


        public bool WallDamaged()
        {
            return wallHealth < wallMaxHealth;
        }

        public bool ScaffoldingDamaged()
        {
            return scaffoldingPiece && !isScaffoldingIntact;
        }

        private IEnumerator JuicyRepair()
        {
            for (float x = 0; x < 1; x += Time.deltaTime * 4)
            {
                transform.localScale = Vector3.one * (1 + (1 - Mathf.Cos(x * 3.14f * 2)) / 2.5f);
                yield return null;
            }
        }

        public bool SetPreview(bool enable)
        {
            if (enable)
            {
                if (WallDamaged() && _wallMeshRenderer.material != translucent)
                {
                    ChangeWallState(wallHealth + 1);
                    _wallMeshRenderer.material = translucent;
                }

                if (scaffoldingPiece && ScaffoldingDamaged() && _scaffoldingMeshRenderer.material != translucent)
                {
                    _scaffoldingMeshRenderer.material = translucent;
                }

                return true;
            }
            else
            {
                if (_wallMeshRenderer.material != _wallMaterial)
                {
                    ChangeWallState(wallHealth);
                    _wallMeshRenderer.material = _wallMaterial;
                }
                if (scaffoldingPiece && _scaffoldingMeshRenderer.material != _scaffoldingMaterial)
                {
                    _scaffoldingMeshRenderer.material = _scaffoldingMaterial;
                }
                return true;
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
            if (!scaffoldingPiece) return;
            //print("DAmaged me" + scaffoldingHealth);
            scaffoldingHealth -= 1;
            if (scaffoldingHealth <= 0) scaffoldingPiece.SetActive(false);
            if (isSoldierPresent)
            {
                soldier.Die();
                isSoldierPresent = false;
            }
        }

        public bool RepairScaffolding()
        {
            if (!scaffoldingPiece) return false;
            if (scaffoldingHealth == scaffoldingMaxHealth) return false;
            scaffoldingHealth = Mathf.Min(scaffoldingMaxHealth, scaffoldingHealth + 1);
            scaffoldingPiece.SetActive(true);
            RequestSoldier();
            StartCoroutine("JuicyRepair");
            return true;
        }

        public void DamageWall()
        {
            wallHealth -= 1;
            if (wallHealth < 0)
            {
                EventManager.RaiseGameOver();
                return;
            }

            ChangeWallState(wallHealth);
            if (isSoldierPresent)
            {
                soldier.Die();
                isSoldierPresent = false;
            }

            if (wallHealth == 0)
            {
                onWallSegmentCritical.Invoke(this);
            }
        }

        public bool RepairWall()
        {
            if (wallHealth == wallMaxHealth) return false;
            wallHealth = Mathf.Min(wallMaxHealth, wallHealth + 1);
            ChangeWallState(wallHealth);
            RequestSoldier();
            onWallNotSegmentCritical.Invoke(this);
            StartCoroutine("JuicyRepair");
            return true;
        }

        private void RequestSoldier()
        {
            if (wallHealth == wallMaxHealth && scaffoldingHealth == scaffoldingMaxHealth && !soldierRequested)
                WallManager.instance.RequestSoldier(this);
        }

        private GUIStyle _style = new GUIStyle();

        private void OnDrawGizmos()
        {
            _style.fontSize = 32;
            if (chosenOne)
                Handles.Label(transform.position + new Vector3(0, 3, 0), "Wall Health: " + wallHealth, _style);
        }

        public void AssignSoldier(FriendlySoldier incomingSoldier)
        {
            soldier = incomingSoldier;
            isSoldierPresent = true;
            soldier.transform.SetParent(transform);
        }

        public bool IsIntact()
        {
            return isScaffoldingIntact && wallHealth == wallMaxHealth;
        }
    }
}