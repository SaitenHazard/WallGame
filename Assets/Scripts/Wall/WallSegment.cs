using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Wall
{
    public class WallSegment : MonoBehaviour
    {
        [Header("__________ Upgrades __________")]
        [Tooltip(">1 --> increased chance to be hit\n=1 --> normal chance to be hit\n<1 --> decreased chance to be hit")]
        public float probabilityModifier = 1.0f;

        [Header("__________ Wall __________")]
        public int wallMaxHealth = 3;
        [Space]
        public int wallHealth;
        [Space]
        public GameObject wallPiece;
        [SerializeField]
        private MeshFilter _wallMeshFilter;
        public Mesh normalWall;
        public Mesh chippedWall;
        public Mesh damagedWall;
        public Mesh destroyedWall;

        [Header("__________ Scaffolding __________")]
        public int scaffoldingMaxHealth = 3;
        [Space]
        public int scaffoldingHealth;
        public bool isScaffoldingIntact = true;
        [Space]
        public GameObject scaffoldingPiece;
        [SerializeField]
        private MeshFilter _scafMeshFilter;
        public Mesh intactScaffolding;
        public Mesh chippedScaffolding;
        public Mesh damagedScaffolding;
        public Mesh brokenScaffolding;

        [Header("__________ Soldier Boy __________")]
        public FriendlySoldier soldier;
        public bool soldierRequested;
        public bool isSoldierPresent;

        public int level; //To be used to request soldier at the correct level
        public bool chosenOne;
        public Material translucent;

        
        private MeshRenderer _meshRenderer;

        private readonly GUIStyle _style = new();
        private Material _wallMaterial;

        /////////////////for Debug Only

        private bool _ciritcalInvooked;
        public readonly WallSegmentNotCriticalEvent OnWallNotSegmentCritical = new();

        public readonly WallSegmentCriticalEvent OnWallSegmentCritical = new();


        private void Start()
        {
            _wallMeshFilter = GetComponentInChildren<MeshFilter>();
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _wallMaterial = _meshRenderer.material;
            normalWall = _wallMeshFilter.mesh;
            scaffoldingHealth = scaffoldingMaxHealth;
            wallHealth = wallMaxHealth;
            // UpdateSoldierState();
        }

        public void Update()
        {
            /* TODO
             * 
             * This needs to be reworked. We cannot invoke an event each frame only to set a value each frame that's probably already set.
             * If it's for debugging, please remove.
             * Also the way the these events are created is not right, please use the EventManager.
             */
            switch (wallHealth)
            {
                case 0 when !_ciritcalInvooked:
                    Debug.Log("Hello");
                    _ciritcalInvooked = true;
                    OnWallSegmentCritical.Invoke(this);
                    break;
                case > 0 when _ciritcalInvooked:
                    Debug.Log("It's me");
                    _ciritcalInvooked = false;
                    OnWallNotSegmentCritical.Invoke(this);
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            _style.fontSize = 32;
            if (chosenOne)
                Handles.Label(transform.position + new Vector3(0, 3, 0), "Wall Health: " + wallHealth, _style);
        }


        public bool WallDamaged()
        {
            return wallHealth < wallMaxHealth;
        }

        public bool ScaffoldingDamaged()
        {
            return isScaffoldingIntact;
        }

        private IEnumerator JuicyRepair()
        {
            for (float x = 0; x < 1; x += Time.deltaTime * 4)
            {
                transform.localScale = Vector3.one * (1 + (1 - Mathf.Cos(x * 3.14f * 2)) / 2.5f);
                yield return null;
            }
        }

        public bool SetPreview(bool enabled)
        {
            if (enabled && wallHealth < wallMaxHealth)
            {
                ChangeWallState(wallHealth + 1);
                _meshRenderer.material = translucent;
                return true;
            }

            ChangeWallState(wallHealth);
            _meshRenderer.material = _wallMaterial;
            return false;
        }

        private void ChangeWallState(int state)
        {
            _wallMeshFilter.mesh = state switch
            {
                0 => destroyedWall,
                1 => damagedWall,
                2 => chippedWall,
                3 => normalWall,
                _ => _wallMeshFilter.mesh
            };
        }

        private void ChangeScaffoldingState(int state)
        {
            _scafMeshFilter.mesh = state switch
            {
                0 => brokenScaffolding,
                1 => damagedScaffolding,
                2 => chippedScaffolding,
                3 => intactScaffolding,
                _ => _scafMeshFilter.mesh
            };
        }

        public void DamageScaffolding()
        {
            if (!scaffoldingPiece) return;
            //print("DAmaged me" + scaffoldingHealth);
            scaffoldingHealth = Mathf.Max(0, scaffoldingHealth-1);
            if (scaffoldingHealth == 0) scaffoldingPiece.GetComponent<BoxCollider>().enabled = false;
            if (isSoldierPresent)
            {
                soldier.DieByArrows();
                isSoldierPresent = false;
            }
            ChangeScaffoldingState(scaffoldingHealth);
        }

        public bool RepairScaffolding()
        {
            if (!scaffoldingPiece) return false;
            if (scaffoldingHealth == scaffoldingMaxHealth) return false;
            scaffoldingHealth = Mathf.Min(scaffoldingMaxHealth, scaffoldingHealth + 1);
            if (scaffoldingHealth == 0) scaffoldingPiece.GetComponent<BoxCollider>().enabled = false;
            RequestSoldier();
            StartCoroutine(nameof(JuicyRepair));
            ChangeScaffoldingState(scaffoldingHealth);
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

            if (wallHealth == 0) OnWallSegmentCritical.Invoke(this);
        }

        public bool RepairWall()
        {
            if (wallHealth == wallMaxHealth) return false;
            wallHealth = Mathf.Min(wallMaxHealth, wallHealth + 1);
            ChangeWallState(wallHealth);
            RequestSoldier();
            OnWallNotSegmentCritical.Invoke(this);
            StartCoroutine(nameof(JuicyRepair));
            return true;
        }

        private void RequestSoldier()
        {
            if (wallHealth == wallMaxHealth && scaffoldingHealth == scaffoldingMaxHealth && !soldierRequested)
                WallManager.instance.RequestSoldier(this);
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