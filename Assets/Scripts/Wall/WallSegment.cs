using UnityEngine;
using UnityEngine.Serialization;

namespace Wall
{
    public class WallSegment : MonoBehaviour
    {
        public GameObject wallPiece;
        public GameObject scaffoldingPiece;
        public FriendlySoldier soldier;

        public int health = 100;
        public bool isScaffoldingIntact = true;
        public bool soldierRequested = false;
        public bool isSoldierPresent;

        private void Start()
        {
            // UpdateSoldierState();
        }

        public void RepairWall()
        {
            health = 100;
            // Additional logic for repairing the wall piece
        }

        public void DamageWall()
        {
            health = Mathf.Max(0, health - 10); // Example damage logic
            // Additional logic for damaging the wall piece
        }

        public void RepairScaffolding()
        {
            isScaffoldingIntact = true;
            scaffoldingPiece.SetActive(true);
            WallManager.instance.RequestSoldier(this);
            // UpdateSoldierState();
        }

        public void DamageScaffolding()
        {
            isScaffoldingIntact = false;
            scaffoldingPiece.SetActive(false);
            if (isSoldierPresent) soldier.Die();
            Invoke("RepairScaffolding", 1);
            // UpdateSoldierState();
        }

        // private void UpdateSoldierState()
        // {
        //     if (isScaffoldingIntact && _isSoldierPresent)
        //     {
        //         soldier.SetActive(true);
        //     }
        //     else
        //     {
        //         soldier.SetActive(false);
        //     }
        // }

        public void AssignSoldier(FriendlySoldier incomingSoldier)
        {
            soldier = incomingSoldier;
            isSoldierPresent = true;
            soldier.transform.SetParent(transform);
        }
    }
}