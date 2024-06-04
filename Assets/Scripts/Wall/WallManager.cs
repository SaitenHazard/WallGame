using UnityEngine;
using System.Collections.Generic;

public class WallManager : MonoBehaviour, IInteractable
{
    public static WallManager Instance { get; private set; }

    public List<WallSegment> wallSegments;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeWallSegments();
    }

    private void InitializeWallSegments()
    {
        // Clear the existing list
        wallSegments.Clear();

        // Iterate through all children and add their WallSegment component to the list
        foreach (Transform child in transform)
        {
            WallSegment segment = child.GetComponent<WallSegment>();
            if (segment != null)
            {
                wallSegments.Add(segment);
            }
        }
    }
    
    
    public WallSegment GetClosestWallSegment(Vector3 position)
    {
        WallSegment closestSegment = null;
        float closestDistance = Mathf.Infinity;

        foreach (WallSegment segment in wallSegments)
        {
            float distance = Vector3.Distance(position, segment.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSegment = segment;
            }
        }

        return closestSegment;
    }
    
    public void Interact()
    {
        GetClosestWallSegment();
    }

    public void RepairWallSegment(int index)
    {
        if (index >= 0 && index < wallSegments.Count)
        {
            wallSegments[index].RepairWall();
        }
    }

    public void DamageWallSegment(int index)
    {
        if (index >= 0 && index < wallSegments.Count)
        {
            wallSegments[index].DamageWall();
        }
    }

    public void RepairScaffoldingSegment(int index)
    {
        if (index >= 0 && index < wallSegments.Count)
        {
            wallSegments[index].RepairScaffolding();
        }
    }

    public void DamageScaffoldingSegment(int index)
    {
        if (index >= 0 && index < wallSegments.Count)
        {
            wallSegments[index].DamageScaffolding();
        }
    }

    public void SetSoldierPresent(int index, bool present)
    {
        if (index >= 0 && index < wallSegments.Count)
        {
            wallSegments[index].SetSoldierPresent(present);
        }
    }

 
}