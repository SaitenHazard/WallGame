using UnityEngine;

public class WallSegment : MonoBehaviour
{
    public GameObject wallPiece;
    public GameObject scaffoldingPiece;
    public GameObject soldier;

    public int health = 100;
    public bool isScaffoldingIntact = true;
    public bool isSoldierPresent = false;

    private void Start()
    {
        UpdateSoldierState();
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
        UpdateSoldierState();
    }

    public void DamageScaffolding()
    {
        isScaffoldingIntact = false;
        scaffoldingPiece.SetActive(false);
        UpdateSoldierState();
    }

    public void SetSoldierPresent(bool present)
    {
        isSoldierPresent = present;
        UpdateSoldierState();
    }

    private void UpdateSoldierState()
    {
        if (isScaffoldingIntact && isSoldierPresent)
        {
            soldier.SetActive(true);
        }
        else
        {
            soldier.SetActive(false);
        }
    }
}