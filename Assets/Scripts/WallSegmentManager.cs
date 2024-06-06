using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class WallSegmentManager : MonoBehaviour
{
    public static WallSegmentManager instance;

    private readonly List<WallSegment> wallSegments = new();
    private readonly List<Scafolding> scafoldings = new();

    [SerializeField] CrossbowmenSpawnPoint spawnPointRow2Left;
    [SerializeField] CrossbowmenSpawnPoint spawnPointRow2Right;

    public float GetVelocity()
    {
        return float.PositiveInfinity;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(SetListWallSegmentObjects());
    }

    private IEnumerator SetListWallSegmentObjects()
    {
        yield return new WaitForSeconds(1);

        foreach (Transform child in transform)
        {
            wallSegments.Add(child.GetComponent<WallSegment>());
            SetClosestSpawnBowMan(child);

            scafoldings.Add(child.GetChild(1).GetComponent<Scafolding>());
        }
        
    }

    public bool InfrontOfDamagedScafolding(Transform _transform, bool spawnedFromleft)
    {
        //float distance = float.PositiveInfinity;

        foreach (Scafolding scafolding in scafoldings)
        {

            if (scafolding.GetHealth() == 0)
            {
                if (spawnedFromleft == false)
                {
                    if (_transform.position.x < scafolding.transform.position.x)
                    {
                        continue;
                    }
                }

                if (spawnedFromleft == true)
                {
                    if (_transform.position.x > scafolding.transform.position.x)
                    {
                        continue;
                    }
                }

                float distanceCandidate = Mathf.Abs(scafolding.transform.position.x - _transform.position.x);

                if (distanceCandidate > 0.95f && distanceCandidate < 1.05f)
                {
                    Debug.Log(distanceCandidate);
                    return true;
                }
            }
        }

        return false;
    }

    private void SetClosestSpawnBowMan(Transform wallSegmentTransform)
    {
        CrossbowmenSpawnPoint spawnPoint = null;
        UnityEngine.Vector3 segmentPosition = wallSegmentTransform.position;
        
        int row = wallSegmentTransform.GetComponent<WallSegment>().GetRow();

        if (row == 2)
        {
            UnityEngine.Vector3 spawnLeftPosition = spawnPointRow2Left.transform.position;
            UnityEngine.Vector3 spawnRightPosition = spawnPointRow2Right.transform.position;

            float distanceFromSpawnLeft = UnityEngine.Vector3.Distance(segmentPosition, spawnLeftPosition);
            float distanceFromSpawnRight = UnityEngine.Vector3.Distance(segmentPosition, spawnRightPosition);

            if (distanceFromSpawnLeft < distanceFromSpawnRight)
                spawnPoint = spawnPointRow2Left;
            else
                spawnPoint = spawnPointRow2Right;
        }

        wallSegmentTransform.GetChild(0).GetComponent<Crossbowman_towsif>().SetSpawnPoint(spawnPoint);
        //Debug.Log(spawnPoint);
        //Debug.Log(wallSegmentTransform.name);
    }
}
