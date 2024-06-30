using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wall;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class ArmyController : MonoBehaviour
    {
        public static ArmyController instance;

        // Start is called before the first frame update
        [Header("_______________ Gameplay Relevant _______________")] [SerializeField]
        public int enemyCount = 100;

        [Space] [SerializeField] private float trebuchetCooldown = 5;
        
        [SerializeField]
        [Tooltip("RANDOM\nThe Columns are picket randomly\n\n" +
                 "SUCCESSION\nThe Columns are picket left to right\n\n" +
                 "RANDOMEQUAL\nThe Columns are picket randomly and all wall columns are hit equally often\n\n" +
                 "RANDOMEQUAL_ANIM\nThe Columns are picket randomly and all wall columns are hit equally often. Trebuchets that have finished their reload animation are preferred in selection.\n\n" +
                 "RANDOMEQUAL_SWITCH\nThe Columns are picket randomly and all wall columns are hit equally often. No Trebuchet fires twice in a row.")]
        private TargetingScheme targetingScheme = TargetingScheme.Random;

        [SerializeField]
        [Tooltip("0.0 - Shots arrive exactly <trebuchet cooldown> seconds apart.\n" +
                 "1.0 - Shots arrive with a random delay of up to 1 second")]
        [Range(0.0f, 1.0f)]
        private float trebuchetRandomness;

        [Space] [SerializeField] private float fireArrowsCooldown = 20;

        [SerializeField] [Range(0f, 1f)] private float fireArrowsDestruction = 0.8f;

        [Header("_______________ Look & Spawn Areas _______________")] [SerializeField] [Tooltip("(Yellow)")]
        private BowmenSettings bowmen;

          [SerializeField] [Tooltip("(Blue)")] private TrebuchetSettings trebuchetSettings;

        [SerializeField] [Tooltip("Green")] private FootsoldierSettings footsoldiers;

        [Header("_______________ Debug Area _______________")]
        public bool consoleOutput = true;

         [Space] public bool debugFireArrows;

         public bool debugTrebuchetLaunch;
         public bool debugKillTrebuchet;

        public List<bool> columnsHit;
        private Transform _bowmenParent;

        [SerializeField]
        private int lastWallIndexHit = -1;

        private Transform _footsoldierParent;
        private Transform _graveyardParent;
        private Transform _trebuchetParent;
        private List<int> _arrowTargets;
        private int _columns;

        private int _footsoldiersForefeit;

        private int _lastCount;
        private Vector2 _lastLowerLeft = Vector2.zero;

        private int _lastTrebuchet = -1;
        private Vector2 _lastUpperRight = Vector2.zero;

        private int _trebuchetCount = 5;

        private List<float> _trebuchetPositions; // vertical positions [0..1] relative to avaliable space

        private List<Trebuchet> _trebuchets;

        private WallManager _wallManager;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            _columns = WallManager.instance.wallColumns;
            /*if (_trebuchets.projectile.flightTime >= _trebuchetCooldown)
            {
                Debug.LogError("trebuchet projectile flight time should never be lower than trebuchet cooldown! flight time will now be set to " + (_trebuchetCooldown + 0.1f));
                _trebuchets.projectile.flightTime = _trebuchetCooldown + 0.1f;
            }*/
            CreateEmptyParents();
            SpawnArmy();
            SpawnTrebuchets();
            SpawnBowmen();
            Invoke(nameof(LaunchTrebuchet), trebuchetCooldown);
            Invoke(nameof(LaunchFireArrows), fireArrowsCooldown);
            _wallManager = FindObjectOfType<WallManager>();
            columnsHit = new List<bool>();
            _columns = WallManager.instance.wallColumns;
            for (var i = 0; i < _trebuchetCount; i++) columnsHit.Add(false);
        }

        private void Update()
        {
            if (debugFireArrows)
            {
                LaunchFireArrows();
                debugFireArrows = false;
            }

            if (debugTrebuchetLaunch)
            {
                LaunchTrebuchet();
                debugTrebuchetLaunch = false;
            }

            if (debugKillTrebuchet)
            {
                KillTrebuchet();
                debugKillTrebuchet = false;
            }
        }

        

        private bool SomethingChanged()
        {
            return _lastCount != _trebuchetCount || _lastLowerLeft != trebuchetSettings.spawnAreaLowerLeft ||
                   _lastUpperRight != trebuchetSettings.spawnAreaUpperRight;
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
        

        private void LaunchTrebuchet()
        {
            if (targetingScheme == TargetingScheme.HoldFire)
            {
                Invoke(nameof(LaunchTrebuchet), trebuchetCooldown);
                return;
            }

            int chosenWallIndex = -1;
            List<WallSegment> segments = WallManager.instance.GetWallSegments();

            if (targetingScheme == TargetingScheme.Succession)
            {
                chosenWallIndex = (lastWallIndexHit + 1) % segments.Count; 
            }
            else
            {   // Picking a random Wall segment with varying probabilities
                float sum = segments.Sum(x => x.probabilityModifier);
                if (targetingScheme == TargetingScheme.Random_NoWallTwice && lastWallIndexHit != -1) sum -= segments.ElementAt(lastWallIndexHit).probabilityModifier;
                List<float> normalizedProbabilities = segments.ConvertAll<float>(x => x.probabilityModifier / sum);
                if (targetingScheme == TargetingScheme.Random_NoWallTwice && lastWallIndexHit != -1) normalizedProbabilities[lastWallIndexHit] = 0;

                Debug.Log("Normalized Probabilities: " + string.Join(")  (", normalizedProbabilities));

                float choice = Random.Range(0.0f, 1.0f);

                float walkingSum = 0;

                for (int i = 0; i < normalizedProbabilities.Count; i++)
                {
                    walkingSum += normalizedProbabilities.ElementAt(i);
                    if (walkingSum >= choice)
                    {
                        chosenWallIndex = i;
                        break;
                    }
                }
            }
            Debug.Log("Chosen Wall Piece: " + chosenWallIndex);
            lastWallIndexHit = chosenWallIndex;

            // Picking correct trebuchet
            int trebuchetIndex = chosenWallIndex % WallManager.instance.wallColumns;

            _trebuchets[trebuchetIndex].SetSelection(segments[chosenWallIndex].transform.position, chosenWallIndex);
            _trebuchets[trebuchetIndex].Launch();

            // Restart cooldown
            Invoke(nameof(LaunchTrebuchet), trebuchetCooldown);
        }

        public void LaunchFireArrows()
        {
            if (targetingScheme == TargetingScheme.HoldFire)
            {
                Invoke(nameof(LaunchFireArrows), fireArrowsCooldown);
                return;
            }
            var parts = bowmen.count / _columns;
            var remainder = bowmen.count % _columns;
            var horizSpacing = (bowmen.spawnAreaUpperRight.x - bowmen.spawnAreaLowerLeft.x) / bowmen.count;
            for (var i = 0; i < _columns; i++)
            {
                
                if (i == _columns - 1) parts += remainder;
                if (Random.Range(0f, 1f) > fireArrowsDestruction) continue;
                for (var j = 0; j < parts; j++)
                {
                    var worldStart = transform.position
                                     //+ new Vector3(_bowmen.spawnAreaLowerLeft.x + i * horizSpacing, 0, _bowmen.spawnAreaUpperRight.y)
                                     + new Vector3(
                                         Random.Range(bowmen.spawnAreaLowerLeft.x, bowmen.spawnAreaUpperRight.x), 0,
                                         Random.Range(-0.5f, 0.5f));
                    var fireArrow = Instantiate(bowmen.fireArrowPrefab, worldStart
                        , Quaternion.identity, _bowmenParent);
                    fireArrow.SetDestination(_wallManager.GetWallSegmentPosition(i) +
                                             new Vector3(Random.Range(-0.6f, 0.6f), 0.7f, Random.Range(-0.3f, 0.3f)));
                    var flightTime = 2 + Random.Range(-0.5f, 0.5f);
                    fireArrow.SetFlightTime(flightTime);
                }

                StartCoroutine(InvokeAfterDelay(3, EventManager.RaiseOnScaffoldingHit, i));
            }

            Invoke(nameof(LaunchFireArrows), fireArrowsCooldown);
        }

        public void SetTargetingScheme(TargetingScheme scheme)
        {
            targetingScheme = scheme;
        }

        private static IEnumerator InvokeAfterDelay<T>(float delay, Action<T> method, T parameter)
        {
            yield return new WaitForSeconds(delay);
            method(parameter);
        }

        public Vector3 GetFootsoldierPosition()
        {
            if (_footsoldiersForefeit < _footsoldierParent.childCount)
                return _footsoldierParent.GetChild(_footsoldiersForefeit++).transform.position + new Vector3(0, 0.6f, 0);
            Debug.LogWarning(
                "More footsoldiers forefeit than left. returning dummy value. If this was close to winning the game you can ignore this warning.");
            return new Vector3(0, 0, 50);

        }

        public void BoltArrives()
        {
            Kill(1);
        }

        public void BombArrives()
        {
            Kill(5);
        }

        private void SpawnArmy()
        {
            float vertDistance = (footsoldiers.spawnAreaUpperRight.y - footsoldiers.spawnAreaLowerLeft.y) / enemyCount;
            for (var i = 0; i < enemyCount; i++)
            {
                var soldier =
                    Instantiate(footsoldiers.prefab,
                        transform.position + new Vector3(
                            Random.Range(footsoldiers.spawnAreaLowerLeft.x, footsoldiers.spawnAreaUpperRight.x),
                            0,
                            footsoldiers.spawnAreaLowerLeft.y + i*vertDistance),
                        Quaternion.identity,
                        _footsoldierParent);
                soldier.SetUp(footsoldiers.vibing, footsoldiers.ecstasy, footsoldiers.hatred);

                if (i % 10 == 0)
                {
                    GameObject fahne = Instantiate(footsoldiers.flagge, soldier.transform.position, Quaternion.Euler(0, 64, 0), soldier.transform);
                }
            }
        }

        private void SpawnTrebuchets()
        {
            AssignNewTrebuchetPositions();
            _trebuchets = new List<Trebuchet>();
            var horizSpacing = (trebuchetSettings.spawnAreaUpperRight.x - trebuchetSettings.spawnAreaLowerLeft.x) /
                               Mathf.Max(1, _trebuchetCount - 1);
            var vertSpace = trebuchetSettings.spawnAreaUpperRight.y - trebuchetSettings.spawnAreaLowerLeft.y;
            for (var i = 0; i < _trebuchetCount; i++)
            {
                var trebuchet = Instantiate(trebuchetSettings.prefab, transform.position
                                                                + new Vector3(trebuchetSettings.spawnAreaLowerLeft.x, 0,
                                                                    trebuchetSettings.spawnAreaLowerLeft.y)
                                                                + new Vector3(i * horizSpacing, 0,
                                                                    _trebuchetPositions[i] * vertSpace),
                    Quaternion.identity, _trebuchetParent);
                trebuchet.SetUp(trebuchetSettings.projectile, trebuchetSettings.reloadSpeed);
                _trebuchets.Add(trebuchet);
            }
        }

        private void SpawnBowmen()
        {
            for (var i = 0; i < bowmen.count; i++)
            {
                var bowman = Instantiate(bowmen.prefab,
                    transform.position +
                    new Vector3(Random.Range(bowmen.spawnAreaLowerLeft.x, bowmen.spawnAreaUpperRight.x), 0,
                        Random.Range(bowmen.spawnAreaLowerLeft.y, bowmen.spawnAreaUpperRight.y)), Quaternion.identity,
                    _bowmenParent);
            }
        }

        private void AssignNewTrebuchetPositions()
        {
            _trebuchetCount = WallManager.instance.wallColumns;
            _trebuchetPositions = new List<float>();
            for (var i = 0; i < _trebuchetCount; i++)
                _trebuchetPositions.Add(Random.Range(0.0f, 1.0f));
        }

        private void Kill(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var victim = _footsoldierParent.GetChild(0);
                victim.SetParent(_graveyardParent);
                victim.GetComponent<Footsoldier>().StartCoroutine(nameof(Footsoldier.Die));
                enemyCount--;
                _footsoldiersForefeit = Mathf.Max(_footsoldiersForefeit - 1, 0);
                if (enemyCount <= 0) SceneManager.LoadScene("Win");
            }
        }

        private void KillTrebuchet()
        {
            _trebuchets[_trebuchetCount - 1].Kill();
            _trebuchets[_trebuchetCount - 1].transform.SetParent(_graveyardParent);
            _trebuchets.RemoveAt(_trebuchetCount - 1);
            _trebuchetCount--;
            var newRowsHit = new List<bool>();
            for (var i = 0; i < _trebuchetCount; i++) newRowsHit.Add(columnsHit[i]);
            columnsHit = newRowsHit;
        }

        public void OnDrawGizmosSelected()
        {
            // Drawing Footsodier Spawn Area
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                transform.position +
                new Vector3(footsoldiers.spawnAreaLowerLeft.x, 0, footsoldiers.spawnAreaLowerLeft.y),
                transform.position + new Vector3(footsoldiers.spawnAreaLowerLeft.x, 0,
                    footsoldiers.spawnAreaUpperRight.y));
            Gizmos.DrawLine(
                transform.position +
                new Vector3(footsoldiers.spawnAreaLowerLeft.x, 0, footsoldiers.spawnAreaLowerLeft.y),
                transform.position + new Vector3(footsoldiers.spawnAreaUpperRight.x, 0,
                    footsoldiers.spawnAreaLowerLeft.y));
            Gizmos.DrawLine(
                transform.position + new Vector3(footsoldiers.spawnAreaUpperRight.x, 0,
                    footsoldiers.spawnAreaUpperRight.y),
                transform.position + new Vector3(footsoldiers.spawnAreaLowerLeft.x, 0,
                    footsoldiers.spawnAreaUpperRight.y));
            Gizmos.DrawLine(
                transform.position + new Vector3(footsoldiers.spawnAreaUpperRight.x, 0,
                    footsoldiers.spawnAreaUpperRight.y),
                transform.position + new Vector3(footsoldiers.spawnAreaUpperRight.x, 0,
                    footsoldiers.spawnAreaLowerLeft.y));
            Gizmos.DrawCube(
                transform.position +
                new Vector3(footsoldiers.spawnAreaLowerLeft.x, 0, footsoldiers.spawnAreaLowerLeft.y),
                new Vector3(1, 1, 1));
            Gizmos.DrawCube(
                transform.position + new Vector3(footsoldiers.spawnAreaUpperRight.x, 0,
                    footsoldiers.spawnAreaUpperRight.y), new Vector3(1, 1, 1));

            // Drawing Bowmen Spawn Area
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                transform.position + new Vector3(bowmen.spawnAreaLowerLeft.x, 0, bowmen.spawnAreaLowerLeft.y),
                transform.position + new Vector3(bowmen.spawnAreaLowerLeft.x, 0, bowmen.spawnAreaUpperRight.y));
            Gizmos.DrawLine(
                transform.position + new Vector3(bowmen.spawnAreaLowerLeft.x, 0, bowmen.spawnAreaLowerLeft.y),
                transform.position + new Vector3(bowmen.spawnAreaUpperRight.x, 0, bowmen.spawnAreaLowerLeft.y));
            Gizmos.DrawLine(
                transform.position + new Vector3(bowmen.spawnAreaUpperRight.x, 0, bowmen.spawnAreaUpperRight.y),
                transform.position + new Vector3(bowmen.spawnAreaLowerLeft.x, 0, bowmen.spawnAreaUpperRight.y));
            Gizmos.DrawLine(
                transform.position + new Vector3(bowmen.spawnAreaUpperRight.x, 0, bowmen.spawnAreaUpperRight.y),
                transform.position + new Vector3(bowmen.spawnAreaUpperRight.x, 0, bowmen.spawnAreaLowerLeft.y));
            Gizmos.DrawCube(
                transform.position + new Vector3(bowmen.spawnAreaLowerLeft.x, 0, bowmen.spawnAreaLowerLeft.y),
                new Vector3(1, 1, 1));
            Gizmos.DrawCube(
                transform.position + new Vector3(bowmen.spawnAreaUpperRight.x, 0, bowmen.spawnAreaUpperRight.y),
                new Vector3(1, 1, 1));

            // Drawing Trebuchet Spawn Area
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                transform.position + new Vector3(trebuchetSettings.spawnAreaLowerLeft.x, 0, trebuchetSettings.spawnAreaLowerLeft.y),
                transform.position +
                new Vector3(trebuchetSettings.spawnAreaLowerLeft.x, 0, trebuchetSettings.spawnAreaUpperRight.y));
            Gizmos.DrawLine(
                transform.position + new Vector3(trebuchetSettings.spawnAreaLowerLeft.x, 0, trebuchetSettings.spawnAreaLowerLeft.y),
                transform.position +
                new Vector3(trebuchetSettings.spawnAreaUpperRight.x, 0, trebuchetSettings.spawnAreaLowerLeft.y));

            Gizmos.DrawLine(
                transform.position +
                new Vector3(trebuchetSettings.spawnAreaUpperRight.x, 0, trebuchetSettings.spawnAreaUpperRight.y),
                transform.position +
                new Vector3(trebuchetSettings.spawnAreaLowerLeft.x, 0, trebuchetSettings.spawnAreaUpperRight.y));
            Gizmos.DrawLine(
                transform.position +
                new Vector3(trebuchetSettings.spawnAreaUpperRight.x, 0, trebuchetSettings.spawnAreaUpperRight.y),
                transform.position +
                new Vector3(trebuchetSettings.spawnAreaUpperRight.x, 0, trebuchetSettings.spawnAreaLowerLeft.y));
            Gizmos.DrawCube(
                transform.position + new Vector3(trebuchetSettings.spawnAreaLowerLeft.x, 0, trebuchetSettings.spawnAreaLowerLeft.y),
                new Vector3(1, 1, 1));
            Gizmos.DrawCube(
                transform.position +
                new Vector3(trebuchetSettings.spawnAreaUpperRight.x, 0, trebuchetSettings.spawnAreaUpperRight.y),
                new Vector3(1, 1, 1));

            // Drawing Trebuchet Cubes
            var updatePositions = SomethingChanged();
            if (updatePositions)
            {
                _lastCount = _trebuchetCount;
                _lastLowerLeft = trebuchetSettings.spawnAreaLowerLeft;
                _lastUpperRight = trebuchetSettings.spawnAreaUpperRight;
                AssignNewTrebuchetPositions();
            }

            var horizSpacing = (trebuchetSettings.spawnAreaUpperRight.x - trebuchetSettings.spawnAreaLowerLeft.x) /
                               Mathf.Max(1, _trebuchetCount - 1);
            var vertSpace = trebuchetSettings.spawnAreaUpperRight.y - trebuchetSettings.spawnAreaLowerLeft.y;

            //if (!Application.IsPlaying(this))
            /*
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
            }*/
        }
    }

    [Serializable]
    internal class EnemyTroop
    {
        public Vector2 spawnAreaLowerLeft;
        public Vector2 spawnAreaUpperRight;
    }

    [Serializable]
    internal class BowmenSettings : EnemyTroop
    {
        public GameObject prefab;
        public TargetProjectile fireArrowPrefab;
        public int count = 20;
    }

    [Serializable]
    internal class TrebuchetSettings : EnemyTroop
    {
        public Trebuchet prefab;
        public float reloadSpeed;
        public ProjectileSettings projectile;
    }

    [Serializable]
    public class ProjectileSettings
    {
        public TargetProjectile prefab;
        public float flightTime;
        public float parabolaHeight;
    }

    [Serializable]
    internal class FootsoldierSettings : EnemyTroop
    {
        public Footsoldier prefab;

        public GameObject flagge;

        //public Color flagColor = Color.white;

        [Tooltip("Defines the Oscillation height (Or footstep height) of the foot soldiers.")]
        public float vibing;

        [Tooltip("Defines the speed at which the foot soldiers oscillate")]
        public float ecstasy;

        [Tooltip("Defines how fast the footsoldiers approach the Wall.")]
        public float hatred;
    }

    
    public enum TargetingScheme
    {
        Random,
        /* The target wall gets picket randomly */

        Succession,
        /* The target wall gets picket left to right */

        Random_NoWallTwice,
        /* The target wall gets picket randomly, but no wall piece is targetet twice in a row. */

        HoldFire
    }
}