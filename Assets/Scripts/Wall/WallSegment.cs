using System;
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

        public int health = 2;
        public bool isScaffoldingIntact = true;
        public bool soldierRequested = false;
        public bool isSoldierPresent;
        public Mesh normalWall;
        public Mesh damagedWall;
        public Mesh destroyedWall;
        public int level; //To be used to request soldier at the correct level
        public bool chosenOne;

        private MeshFilter _meshFilter;

        private void Start()
        {
            _meshFilter = GetComponentInChildren<MeshFilter>();
            normalWall = _meshFilter.mesh;
            // UpdateSoldierState();
        }

        private void ChangeWallState()
        {
            _meshFilter.mesh = health switch
            {
                0 => destroyedWall,
                1 => damagedWall,
                2 => normalWall,
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
            health = Mathf.Max(0, health - 1);
            ChangeWallState();
            if (isSoldierPresent)
            {
                soldier.Die();
                isSoldierPresent = false;
            }
        }

        public bool RepairWall()
        {
            if (health == 2) return false;
            health = Mathf.Min(2, health + 1);
            ChangeWallState();
            RequestSoldier();
            return true;
        }

        private void RequestSoldier()
        {
            if (health == 2 && isScaffoldingIntact && !soldierRequested) WallManager.instance.RequestSoldier(this);
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
            return isScaffoldingIntact && health == 2;
        }
    }
}
