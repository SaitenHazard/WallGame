using UnityEngine;

namespace Enemies
{
    public class ArmyController : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject soldierPrefab;

        public int enemyCount = 100;

        public Vector2 spawnMin;
        public Vector2 spawnMax;

        [Header("Footsoldier Settings")]

        /*public Formation formation = Formation.Scattered;

    public int ROWS_SoldiersPerRow = 50;

    public Vector3 LEGIONS_columns_rows_legions = new Vector3(5, 8, 5);*/

        [Tooltip("How far up and down the footsoldiers oscillate")]
        public float vibing = 0.05f;

        [Tooltip("How fast the footsoldiers oscillate")]
        public float ecstasy = 3;

        [Tooltip("How fast the footsoldiers approach the wall")]
        public float hatred = 0.3f;

        void Start()
        {
            SpawnArmy();
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
                GameObject soldier = Instantiate(soldierPrefab, transform.position + new Vector3(Random.Range(spawnMin.x, spawnMax.x), 0, Random.Range(spawnMin.y, spawnMax.y)), Quaternion.identity, transform);
                soldier.GetComponent<Footsoldier>().SetUp(vibing, ecstasy, hatred);
            }
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
            Gizmos.DrawLine(transform.position + new Vector3(spawnMin.x, 0, spawnMin.y), transform.position + new Vector3(spawnMin.x, 0, spawnMax.y));
            Gizmos.DrawLine(transform.position + new Vector3(spawnMin.x, 0, spawnMin.y), transform.position + new Vector3(spawnMax.x, 0, spawnMin.y));

            Gizmos.DrawLine(transform.position + new Vector3(spawnMax.x, 0, spawnMax.y), transform.position + new Vector3(spawnMin.x, 0, spawnMax.y));
            Gizmos.DrawLine(transform.position + new Vector3(spawnMax.x, 0, spawnMax.y), transform.position + new Vector3(spawnMax.x, 0, spawnMin.y));
        }
    }

    public enum Formation
    {
        Scattered, InRows, InLegions
    }
}