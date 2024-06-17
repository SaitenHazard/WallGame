
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Wall;

namespace Enemies
{
    public class ArmyController : MonoBehaviour
    {
        public static ArmyController instance;

        // Start is called before the first frame update
        [Header("_______________ Gameplay Relevant _______________")]
        [SerializeField]
        public int enemyCount = 100;
        [Space]
        [SerializeField]
        private float _trebuchetCooldown = 5;
        [SerializeField]
        [Tooltip("RANDOM\nThe Columns are picket randomly\n\n" +
                 "SUCCESSION\nThe Columns are picket left to right\n\n" +
                 "RANDOMEQUAL\nThe Columns are picket randomly and all wall columns are hit equally often\n\n" +
                 "RANDOMEQUAL_ANIM\nThe Columns are picket randomly and all wall columns are hit equally often. Trebuchets that have finished their reload animation are preferred in selection.\n\n" +
                 "RANDOMEQUAL_SWITCH\nThe Columns are picket randomly and all wall columns are hit equally often. No Trebuchet fires twice in a row.")]
        private TargetingScheme _targetingScheme = TargetingScheme.RANDOM;
        [SerializeField]
        [Tooltip("0.0 - Shots arrive exactly <trebuchet cooldown> seconds apart.\n" + 
            "1.0 - Shots arrive with a random delay of up to 1 second")]
        [Range(0.0f,1.0f)]
        private float _trebuchetRandomness = 0.0f;
        [Space]
        [Header("As of now, fire arrows are purely cosmetic")]
        [SerializeField]
        private float fireArrowsCooldown = 20;
        [SerializeField]
        [Range(0f, 1f)]
        private float fireArrowsDestruction = 0.8f;

        [Header("_______________ Look & Spawn Areas _______________")]
        [SerializeField]
        [Tooltip("(Yellow)")]
        private BowmenSettings _bowmen;

        [SerializeField]
        [Tooltip("(Blue)")]
        private TrebuchetSettings _trebuchets;

        [SerializeField]
        [Tooltip("Green")]
        private FootsoldierSettings _footsoldiers;

        [Header("_______________ Debug Area _______________")]
        public bool consoleOutput = true;
        [Space]
        public bool DEBUG_FireArrows = false;
        public bool DEBUG_TrebuchetLaunch = false;
        public bool DEBUG_KillTrebuchet = false;

        public List<bool> columnsHit;

        private int trebuchetCount = 5;

        private WallManager wallManager;
        private int columns;
        private List<int> arrowTargets;

        private List<float> trebuchetPositions; // vertical positions [0..1] relative to avaliable space

        private int lastCount = 0;
        private Vector2 lastLowerLeft = Vector2.zero;
        private Vector2 lastUpperRight = Vector2.zero;
        private bool SomethingChanged() {
            return lastCount != trebuchetCount || lastLowerLeft != _trebuchets.spawnAreaLowerLeft || lastUpperRight != _trebuchets.spawnAreaUpperRight;
        }

        private Transform _footsoldierParent = null;
        private Transform _graveyardParent = null;
        private Transform _trebuchetParent = null;
        private Transform _bowmenParent = null;

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            columns = WallManager.instance.wallColumns;
            /*if (_trebuchets.projectile.flightTime >= _trebuchetCooldown)
            {
                Debug.LogError("trebuchet projectile flight time should never be lower than trebuchet cooldown! flight time will now be set to " + (_trebuchetCooldown + 0.1f));
                _trebuchets.projectile.flightTime = _trebuchetCooldown + 0.1f;
            }*/
            CreateEmptyParents();
            SpawnArmy();
            SpawnTrebuchets();
            SpawnBowmen();
            Invoke("LaunchTrebuchet", _trebuchetCooldown);
            Invoke("LaunchFireArrows", fireArrowsCooldown);
            wallManager = FindObjectOfType<WallManager>();
            columnsHit = new List<bool>();
            for (int i = 0; i < trebuchetCount; i++)
            {
                columnsHit.Add(false);
            }
        }

        private void CreateEmptyParents()
        {
            _footsoldierParent = new GameObject("Footsoldier Parent").transform;
            _footsoldierParent.parent = transform;
            _trebuchetParent = new GameObject("Trebuchet Parent").transform;
            _trebuchetParent.parent = transform;
            _bowmenParent = new GameObject("Bowmen Parent").transform;
            _bowmenParent.parent = transform;

            _graveyardParent = new GameObject("Graveyard").transform;
            _graveyardParent.parent = transform;
        }

        private List<Trebuchet> trebuchets;

        private int lastTrebuchet = -1;


        private void LaunchTrebuchet()
        {
            int wallPieceIndex = -1;
            int trebuchetIndex = -1;

            if (!columnsHit.Contains(false))
            {
                columnsHit = new List<bool>();
                for (int i = 0; i < trebuchetCount; i++)
                {
                    columnsHit.Add(false);
                }
            }
            string output = "TrebuchetChoice:\n";
            List<int> infrage = new List<int>();
            switch (_targetingScheme)
            {
                case TargetingScheme.RANDOM:
                    trebuchetIndex = Random.Range(0, trebuchets.Count);
                    break;
                case TargetingScheme.SUCCESSION:
                    trebuchetIndex = ++lastTrebuchet % trebuchetCount;
                    break;
                case TargetingScheme.RANDOMEQUAL:
                    trebuchetIndex = Random.Range(0, trebuchetCount);

                    if (columnsHit[trebuchetIndex])
                        trebuchetIndex = columnsHit.FindIndex(t => !t);
                    
                    break;
                case TargetingScheme.RANDOMEQUAL_SWITCH:
                    
                    for (int i = 0; i < trebuchetCount; i++)
                    {
                        if (!columnsHit[i] && i != lastTrebuchet) infrage.Add(i);
                    }
                    output += ("Infrage kommen: " + System.String.Join(" ", infrage) + "\n");

                    trebuchetIndex = infrage.Count == 1 ? infrage[0] : infrage[Random.Range(0, infrage.Count)];

                    break;

                case TargetingScheme.RANDOMEQUAL_ANIM:
                    for (int i = 0; i < trebuchetCount; i++)
                    {
                        if (!columnsHit[i]) infrage.Add(i);
                    }
                    output += ("Infrage kommen: " + System.String.Join(" ", infrage) + "\n");
                    List<int> preferred = new List<int>();
                    for (int i = 0; i < infrage.Count; i++)
                    {
                        if (trebuchets[infrage[i]].ready) preferred.Add(infrage[i]);
                    }
                    output += ("Preferrable sind: " + System.String.Join(" ", preferred) + "\n");
                    if (preferred.Count == 0)
                    {
                        // No possible Trebuchet is ready, we're choosing a non-ready one.
                        trebuchetIndex = infrage.Count == 1? infrage[0] : infrage[Random.Range(0, infrage.Count)];
                    } else 
                    {
                        trebuchetIndex = preferred.Count == 1? preferred[0] : preferred[Random.Range(0, preferred.Count)];
                    }
                    output += ("Gewählt wurde: " + trebuchetIndex);
                    
                    break;
            }

            columnsHit[trebuchetIndex] = true;

            if (consoleOutput) Debug.Log(output);

            if (!trebuchets[trebuchetIndex].ready)
            {
                if (consoleOutput) Debug.LogWarning("The selected trebuchet had not yet finished his reload animation. Consider increasing reload speed.");
            }
            lastTrebuchet = trebuchetIndex;

            int selectedRow = Random.Range(0, WallManager.instance.wallRows);

            wallPieceIndex = selectedRow * WallManager.instance.wallColumns + trebuchetIndex;

            //Debug.Log("Launching at (" + trebuchetIndex + ", " + selectedRow + ") index " + wallPieceIndex);

            trebuchets[trebuchetIndex].SetSelection(wallManager.GetWallSegmentPosition(wallPieceIndex), wallPieceIndex);
            if (_trebuchetRandomness != 0)
                trebuchets[trebuchetIndex].SetFlightTime(_trebuchets.projectile.flightTime + Random.Range(0, _trebuchetRandomness));
            trebuchets[trebuchetIndex].Launch();
            
            Invoke("LaunchTrebuchet", _trebuchetCooldown);
        }

        private void LaunchFireArrows()
        {
            int parts = _bowmen.count / columns;
            int remainder = _bowmen.count % columns;
            bool addRemainder = false;
            float horizSpacing = (_bowmen.spawnAreaUpperRight.x - _bowmen.spawnAreaLowerLeft.x) / _bowmen.count;
            for (var i = 0; i < columns; i++)
            {
                print(i);
                if (addRemainder) parts += remainder;
                if (UnityEngine.Random.Range(0f, 1f) > fireArrowsDestruction) continue;
                for (var j = 0; j < parts; j++)
                {
                    Vector3 worldStart = transform.position
                                                + new Vector3(_bowmen.spawnAreaLowerLeft.x + i * horizSpacing, 0, _bowmen.spawnAreaUpperRight.y)
                                                + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0, UnityEngine.Random.Range(-0.5f, 0.5f));
                    TargetProjectile fireArrow = Instantiate(_bowmen.fireArrowPrefab, worldStart
                        , Quaternion.identity, _bowmenParent);
                    fireArrow.SetDestination(wallManager.GetWallSegmentPosition(i));
                    var flightTime = 3 + UnityEngine.Random.Range(-0.5f, 0.5f);
                    fireArrow.SetFlightTime(flightTime);
                }
                StartCoroutine(InvokeAfterDelay(3, EventManager.RaiseOnScaffoldingHit, i));
            }
            Invoke("LaunchFireArrows", fireArrowsCooldown);
        }

        private IEnumerator InvokeAfterDelay<T>(float delay, System.Action<T> method, T parameter)
        {
            yield return new WaitForSeconds(delay);
            method(parameter);
        }

        private void Update()
        {
            if (DEBUG_FireArrows)
            {
                LaunchFireArrows();
                DEBUG_FireArrows = false;
            }
            if (DEBUG_TrebuchetLaunch)
            {
                LaunchTrebuchet();
                DEBUG_TrebuchetLaunch = false;
            }
            if (DEBUG_KillTrebuchet)
            {
                KillTrebuchet();
                DEBUG_KillTrebuchet = false;
            }
        }

        private int footsoldiersForefeit = 0;

        public Vector3 GetFootsoldierPosition()
        {
            return _footsoldierParent.GetChild(footsoldiersForefeit++).transform.position + new Vector3(0,0.6f,0);
        }

        public void BoltArrives()
        {
            Kill(1);
        }

        private void SpawnArmy()
        {
            for (int i = 0; i < enemyCount; i++)
            {
                Footsoldier soldier = 
                    Instantiate(_footsoldiers.prefab, 
                    transform.position + new Vector3(Random.Range(_footsoldiers.spawnAreaLowerLeft.x, _footsoldiers.spawnAreaUpperRight.x), 
                                                    0, 
                                                    Random.Range(_footsoldiers.spawnAreaLowerLeft.y, _footsoldiers.spawnAreaUpperRight.y)), 
                    Quaternion.identity, 
                    _footsoldierParent);
                soldier.SetUp(_footsoldiers.vibing, _footsoldiers.ecstasy, _footsoldiers.hatred);
            }
        }

        public void SpawnTrebuchets()
        {
            AssignNewTrebuchetPositions();
            trebuchets = new List<Trebuchet>();
            float horizSpacing = (_trebuchets.spawnAreaUpperRight.x - _trebuchets.spawnAreaLowerLeft.x) / Mathf.Max(1, trebuchetCount - 1);
            float vertSpace = _trebuchets.spawnAreaUpperRight.y - _trebuchets.spawnAreaLowerLeft.y;
            for (int i = 0; i < trebuchetCount; i++)
            {
                Trebuchet trebuchet = Instantiate(_trebuchets.prefab, transform.position
                                                                     + new Vector3(_trebuchets.spawnAreaLowerLeft.x, 0, _trebuchets.spawnAreaLowerLeft.y)
                                                                     + new Vector3(i * horizSpacing, 0, trebuchetPositions[i] * vertSpace), Quaternion.identity, _trebuchetParent);
                trebuchet.SetUp(_trebuchets.projectile, _trebuchets.reloadSpeed);
                trebuchets.Add(trebuchet);
            }
        }

        public void SpawnBowmen()
        {
            for (int i = 0; i < _bowmen.count; i++)
            {
                GameObject bowman = Instantiate(_bowmen.prefab, transform.position + new Vector3(Random.Range(_bowmen.spawnAreaLowerLeft.x, _bowmen.spawnAreaUpperRight.x), 0, Random.Range(_bowmen.spawnAreaLowerLeft.y, _bowmen.spawnAreaUpperRight.y)), Quaternion.identity, _bowmenParent);
            }
        }

        private void AssignNewTrebuchetPositions()
        {
            trebuchetCount = WallManager.instance.wallColumns;
            trebuchetPositions = new List<float>();
            for (int i = 0; i < trebuchetCount; i++)
                trebuchetPositions.Add(Random.Range(0.0f, 1.0f));
        }

        public void Kill(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Transform victim = _footsoldierParent.GetChild(0);
                victim.SetParent(_graveyardParent);
                victim.GetComponent<Footsoldier>().StartCoroutine("Die");
                enemyCount--;
                footsoldiersForefeit = Mathf.Max(footsoldiersForefeit - 1, 0);
            }
        }

        public void KillTrebuchet()
        {
            trebuchets[trebuchetCount-1].Kill();
            trebuchets[trebuchetCount-1].transform.SetParent(_graveyardParent);
            trebuchets.RemoveAt(trebuchetCount - 1);
            trebuchetCount--;
            List<bool> newRowsHit = new List<bool>();
            for (int i = 0; i < trebuchetCount; i++)
            {
                newRowsHit.Add(columnsHit[i]);
            }
            columnsHit = newRowsHit;
        }

        public void OnDrawGizmosSelected()
        {
            // Drawing Footsodier Spawn Area
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + new Vector3(_footsoldiers.spawnAreaLowerLeft.x, 0, _footsoldiers.spawnAreaLowerLeft.y), transform.position + new Vector3(_footsoldiers.spawnAreaLowerLeft.x, 0, _footsoldiers.spawnAreaUpperRight.y));
            Gizmos.DrawLine(transform.position + new Vector3(_footsoldiers.spawnAreaLowerLeft.x, 0, _footsoldiers.spawnAreaLowerLeft.y), transform.position + new Vector3(_footsoldiers.spawnAreaUpperRight.x, 0, _footsoldiers.spawnAreaLowerLeft.y));
            Gizmos.DrawLine(transform.position + new Vector3(_footsoldiers.spawnAreaUpperRight.x, 0, _footsoldiers.spawnAreaUpperRight.y), transform.position + new Vector3(_footsoldiers.spawnAreaLowerLeft.x, 0, _footsoldiers.spawnAreaUpperRight.y));
            Gizmos.DrawLine(transform.position + new Vector3(_footsoldiers.spawnAreaUpperRight.x, 0, _footsoldiers.spawnAreaUpperRight.y), transform.position + new Vector3(_footsoldiers.spawnAreaUpperRight.x, 0, _footsoldiers.spawnAreaLowerLeft.y));
            Gizmos.DrawCube(transform.position + new Vector3(_footsoldiers.spawnAreaLowerLeft.x, 0, _footsoldiers.spawnAreaLowerLeft.y), new Vector3(1, 1, 1));
            Gizmos.DrawCube(transform.position + new Vector3(_footsoldiers.spawnAreaUpperRight.x, 0, _footsoldiers.spawnAreaUpperRight.y), new Vector3(1, 1, 1));

            // Drawing Bowmen Spawn Area
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + new Vector3(_bowmen.spawnAreaLowerLeft.x, 0, _bowmen.spawnAreaLowerLeft.y), transform.position + new Vector3(_bowmen.spawnAreaLowerLeft.x, 0, _bowmen.spawnAreaUpperRight.y));
            Gizmos.DrawLine(transform.position + new Vector3(_bowmen.spawnAreaLowerLeft.x, 0, _bowmen.spawnAreaLowerLeft.y), transform.position + new Vector3(_bowmen.spawnAreaUpperRight.x, 0, _bowmen.spawnAreaLowerLeft.y));
            Gizmos.DrawLine(transform.position + new Vector3(_bowmen.spawnAreaUpperRight.x, 0, _bowmen.spawnAreaUpperRight.y), transform.position + new Vector3(_bowmen.spawnAreaLowerLeft.x, 0, _bowmen.spawnAreaUpperRight.y));
            Gizmos.DrawLine(transform.position + new Vector3(_bowmen.spawnAreaUpperRight.x, 0, _bowmen.spawnAreaUpperRight.y), transform.position + new Vector3(_bowmen.spawnAreaUpperRight.x, 0, _bowmen.spawnAreaLowerLeft.y));
            Gizmos.DrawCube(transform.position + new Vector3(_bowmen.spawnAreaLowerLeft.x, 0, _bowmen.spawnAreaLowerLeft.y), new Vector3(1, 1, 1));
            Gizmos.DrawCube(transform.position + new Vector3(_bowmen.spawnAreaUpperRight.x, 0, _bowmen.spawnAreaUpperRight.y), new Vector3(1, 1, 1));

            // Drawing Trebuchet Spawn Area
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + new Vector3(_trebuchets.spawnAreaLowerLeft.x, 0, _trebuchets.spawnAreaLowerLeft.y), transform.position + new Vector3(_trebuchets.spawnAreaLowerLeft.x, 0, _trebuchets.spawnAreaUpperRight.y));
            Gizmos.DrawLine(transform.position + new Vector3(_trebuchets.spawnAreaLowerLeft.x, 0, _trebuchets.spawnAreaLowerLeft.y), transform.position + new Vector3(_trebuchets.spawnAreaUpperRight.x, 0, _trebuchets.spawnAreaLowerLeft.y));

            Gizmos.DrawLine(transform.position + new Vector3(_trebuchets.spawnAreaUpperRight.x, 0, _trebuchets.spawnAreaUpperRight.y), transform.position + new Vector3(_trebuchets.spawnAreaLowerLeft.x, 0, _trebuchets.spawnAreaUpperRight.y));
            Gizmos.DrawLine(transform.position + new Vector3(_trebuchets.spawnAreaUpperRight.x, 0, _trebuchets.spawnAreaUpperRight.y), transform.position + new Vector3(_trebuchets.spawnAreaUpperRight.x, 0, _trebuchets.spawnAreaLowerLeft.y));
            Gizmos.DrawCube(transform.position + new Vector3(_trebuchets.spawnAreaLowerLeft.x, 0, _trebuchets.spawnAreaLowerLeft.y), new Vector3(1, 1, 1));
            Gizmos.DrawCube(transform.position + new Vector3(_trebuchets.spawnAreaUpperRight.x, 0, _trebuchets.spawnAreaUpperRight.y), new Vector3(1, 1, 1));

            // Drawing Trebuchet Cubes
            bool updatePositions = SomethingChanged();
            if (updatePositions)
            {
                lastCount = trebuchetCount;
                lastLowerLeft = _trebuchets.spawnAreaLowerLeft;
                lastUpperRight = _trebuchets.spawnAreaUpperRight;
                AssignNewTrebuchetPositions();
            }
            float horizSpacing = (_trebuchets.spawnAreaUpperRight.x - _trebuchets.spawnAreaLowerLeft.x) / Mathf.Max(1, trebuchetCount-1);
            float vertSpace = _trebuchets.spawnAreaUpperRight.y - _trebuchets.spawnAreaLowerLeft.y;

            //if (!Application.IsPlaying(this))

            for (int i = 0; i < trebuchetCount; i++)
            {
                Gizmos.color = Color.blue;
                Vector3 trebuchetPos = transform.position
                    + new Vector3(_trebuchets.spawnAreaLowerLeft.x, 0, _trebuchets.spawnAreaLowerLeft.y)
                    + new Vector3(i * horizSpacing, 2.5f, trebuchetPositions[i] * vertSpace);

                Gizmos.DrawCube(trebuchetPos, new Vector3(1.5f, 5f, 2.6f));

                Vector3 parabolaPeak = trebuchetPos / 2 + new Vector3(0, _trebuchets.projectile.parabolaHeight + 6, 0);

                Gizmos.color = new Color(0, 0, 0.5f, 0.5f);
                Gizmos.DrawLine(trebuchetPos, parabolaPeak);
                Gizmos.DrawLine(parabolaPeak, new Vector3((i - trebuchetCount/2)*1.5f, 0, 0));
            }
        }
    }

    public enum Formation
    {
        Scattered, InRows, InLegions
    }

    [System.Serializable]
    class EnemyTroop
    {
        public Vector2 spawnAreaLowerLeft;
        public Vector2 spawnAreaUpperRight;
    }

    [System.Serializable]
    class BowmenSettings : EnemyTroop
    {
        public GameObject prefab;
        public TargetProjectile fireArrowPrefab;
        public int count = 20;
    }

    [System.Serializable]
    class TrebuchetSettings : EnemyTroop
    {
        public Trebuchet prefab;
        public float reloadSpeed;
        public ProjectileSettings projectile;
    }

    [System.Serializable]
    public class ProjectileSettings
    {
        public TargetProjectile prefab;
        public float flightTime;
        public float parabolaHeight;
    }

    [System.Serializable]
    class FootsoldierSettings : EnemyTroop
    {
        public Footsoldier prefab;
        [Tooltip("Defines the Oscillation height (Or footstep height) of the foot soldiers.")]
        public float vibing;

        [Tooltip("Defines the speed at which the foot soldiers oscillate")]
        public float ecstasy;

        [Tooltip("Defines how fast the footsoldiers approach the Wall.")]
        public float hatred;
    }


    enum TargetingScheme
    {
        RANDOM,          
        /* The Columns get picket randomly */
        
        SUCCESSION,      
        /* The Columns get picket left to right */

        RANDOMEQUAL,     
        /* The Columns get picket randomly, but no trebuchet fires twice in a row and all wall columns are hit equally often */

        RANDOMEQUAL_ANIM, 
        /* The Columns are picket randomly and all wall columns are hit equally often. Trebuchets that have finished 
         * their reload animation are preferred in selection. */

        RANDOMEQUAL_SWITCH 
        /* The Columns are picket randomly and all wall columns are hit equally often. No Trebuchet fires twice in a row. */
    }
}