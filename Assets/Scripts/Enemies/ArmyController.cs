using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class ArmyController : MonoBehaviour
    {
        // Start is called before the first frame update


        [Header("_______________ Trebuchet Settings _______________")]
        public Trebuchet trebuchetPrefab;
        public Vector2 trebuchetSpawnAreaLowerLeft;
        public Vector2 trebuchetSpawnAreaUpperRight;
        [Tooltip("Numbers of trebuchets spawned at startup")]
        public int trebuchetCount = 5;

        private List<float> trebuchetPositions; // vertical positions [0..1] relative to avaliable space

        [Tooltip("Fligth Time of the trebuchet rounds in seconds")]
        public float projectileFlightTime;

        private int lastCount = 0;
        private Vector2 lastLowerLeft = Vector2.zero;
        private Vector2 lastUpperRight = Vector2.zero;
        private bool SomethingChanged() {
            return lastCount != trebuchetCount || lastLowerLeft != trebuchetSpawnAreaLowerLeft || lastUpperRight != trebuchetSpawnAreaUpperRight;
        }

   
        [Header("_______________ Footsoldier Settings _______________")]
        public Footsoldier soldierPrefab;
        public Vector2 spawnAreaLowerLeft;
        public Vector2 spawnAreaUpperRight;
        public int enemyCount = 100;

        [Tooltip("How far up and down the footsoldiers oscillate")]
        public float vibing = 0.05f;

        [Tooltip("How fast the footsoldiers oscillate")]
        public float ecstasy = 3;

        [Tooltip("How fast the footsoldiers approach the wall")]
        public float hatred = 0.3f;

        void Start()
        {
            SpawnArmy();
            SpawnTrebuchets();
        }

        public bool DEBUG_KillOne = false;

        private void Update()
        {
            if (DEBUG_KillOne)
            {
                Kill(1);
                DEBUG_KillOne=false;
            }
        }

        public int footsoldiersForefeit = 0;

        public Vector3 GetFootsoldierPosition()
        {
            return transform.GetChild(footsoldiersForefeit++).transform.position;
        }
        private void SpawnArmy()
        {
            for (int i = 0; i < enemyCount; i++)
            {
                Footsoldier soldier = Instantiate(soldierPrefab, transform.position + new Vector3(Random.Range(spawnAreaLowerLeft.x, spawnAreaUpperRight.x), 0, Random.Range(spawnAreaLowerLeft.y, spawnAreaUpperRight.y)), Quaternion.identity, transform.GetChild(0));
                soldier.SetUp(vibing, ecstasy, hatred);
            }
        }

        public void SpawnTrebuchets()
        {
            AssignNewTrebuchetPositions();
            float horizSpacing = (trebuchetSpawnAreaUpperRight.x - trebuchetSpawnAreaLowerLeft.x) / Mathf.Max(1, trebuchetCount - 1);
            float vertSpace = trebuchetSpawnAreaUpperRight.y - trebuchetSpawnAreaLowerLeft.y;
            for (int i = 0; i < trebuchetCount; i++)
            {
                Trebuchet trebuchet = Instantiate(trebuchetPrefab, transform.position
                                                                     + new Vector3(trebuchetSpawnAreaLowerLeft.x, 0, trebuchetSpawnAreaLowerLeft.y)
                                                                     + new Vector3(i * horizSpacing, 0, trebuchetPositions[i] * vertSpace), Quaternion.identity, transform.GetChild(1));
            }
        }

        private void AssignNewTrebuchetPositions()
        {
            trebuchetPositions = new List<float>();
            for (int i = 0; i < trebuchetCount; i++)
                trebuchetPositions.Add(Random.Range(0.0f, 1.0f));
        }

        public void Kill(int count)
        {
            for (int i = 0; i < count; i++)
            {
                StartCoroutine(transform.GetChild(0).GetComponent<Footsoldier>().Die());
                enemyCount--;
                footsoldiersForefeit = Mathf.Clamp(footsoldiersForefeit - 1, 0, 5000);
            }
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + new Vector3(spawnAreaLowerLeft.x, 0, spawnAreaLowerLeft.y), transform.position + new Vector3(spawnAreaLowerLeft.x, 0, spawnAreaUpperRight.y));
            Gizmos.DrawLine(transform.position + new Vector3(spawnAreaLowerLeft.x, 0, spawnAreaLowerLeft.y), transform.position + new Vector3(spawnAreaUpperRight.x, 0, spawnAreaLowerLeft.y));

            Gizmos.DrawLine(transform.position + new Vector3(spawnAreaUpperRight.x, 0, spawnAreaUpperRight.y), transform.position + new Vector3(spawnAreaLowerLeft.x, 0, spawnAreaUpperRight.y));
            Gizmos.DrawLine(transform.position + new Vector3(spawnAreaUpperRight.x, 0, spawnAreaUpperRight.y), transform.position + new Vector3(spawnAreaUpperRight.x, 0, spawnAreaLowerLeft.y));

            // Drawing TrebuchetPositions
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + new Vector3(trebuchetSpawnAreaLowerLeft.x, 0, trebuchetSpawnAreaLowerLeft.y), transform.position + new Vector3(trebuchetSpawnAreaLowerLeft.x, 0, trebuchetSpawnAreaUpperRight.y));
            Gizmos.DrawLine(transform.position + new Vector3(trebuchetSpawnAreaLowerLeft.x, 0, trebuchetSpawnAreaLowerLeft.y), transform.position + new Vector3(trebuchetSpawnAreaUpperRight.x, 0, trebuchetSpawnAreaLowerLeft.y));

            Gizmos.DrawLine(transform.position + new Vector3(trebuchetSpawnAreaUpperRight.x, 0, trebuchetSpawnAreaUpperRight.y), transform.position + new Vector3(trebuchetSpawnAreaLowerLeft.x, 0, trebuchetSpawnAreaUpperRight.y));
            Gizmos.DrawLine(transform.position + new Vector3(trebuchetSpawnAreaUpperRight.x, 0, trebuchetSpawnAreaUpperRight.y), transform.position + new Vector3(trebuchetSpawnAreaUpperRight.x, 0, trebuchetSpawnAreaLowerLeft.y));

            bool updatePositions = SomethingChanged();
            if (updatePositions)
            {
                lastCount = trebuchetCount;
                lastLowerLeft = trebuchetSpawnAreaLowerLeft;
                lastUpperRight = trebuchetSpawnAreaUpperRight;
                AssignNewTrebuchetPositions();
            }
            float horizSpacing = (trebuchetSpawnAreaUpperRight.x - trebuchetSpawnAreaLowerLeft.x) / Mathf.Max(1, trebuchetCount-1);
            float vertSpace = trebuchetSpawnAreaUpperRight.y - trebuchetSpawnAreaLowerLeft.y;
            for (int i = 0; i < trebuchetCount; i++)
            {
                Gizmos.DrawCube(transform.position
                    + new Vector3(trebuchetSpawnAreaLowerLeft.x, 0, trebuchetSpawnAreaLowerLeft.y)
                    + new Vector3(i * horizSpacing, 2.5f, trebuchetPositions[i] * vertSpace), new Vector3(1.5f, 5f, 2.6f));
            }
        }
    }
    

    public enum Formation
    {
        Scattered, InRows, InLegions
    }
}