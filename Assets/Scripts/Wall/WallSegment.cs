using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Wall
{
    public class WallSegment : MonoBehaviour
    {
        public GameObject wallPiece;
        public GameObject scaffoldingPiece;
        public FriendlySoldier soldier;

        public int wallMaxHealth = 3;
        public int wallHealth;
        public int scaffoldingMaxHealth = 2;
        public int scaffoldingHealth;
        public bool isScaffoldingIntact = true;
        public bool soldierRequested;
        public bool isSoldierPresent;
        public Mesh normalWall;
        public Mesh chippedWall;
        public Mesh damagedWall;
        public Mesh destroyedWall;
        public int level; //To be used to request soldier at the correct level
        public bool chosenOne;
        public Material translucent;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        private readonly GUIStyle _style = new();
        private Material _wallMaterial;


        public readonly WallSegmentNotCriticalEvent OnWallNotSegmentCritical = new();

        public readonly WallSegmentCriticalEvent OnWallSegmentCritical = new();


        private void Start()
        {
            _meshFilter = GetComponentInChildren<MeshFilter>();
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _wallMaterial = _meshRenderer.material;
            normalWall = _meshFilter.mesh;
            scaffoldingHealth = scaffoldingMaxHealth;
            wallHealth = wallMaxHealth;
            // UpdateSoldierState();
        }

        //public void Update()
        //{
        //    /* TODO
        //     * 
        //     * This needs to be reworked. We cannot invoke an event each frame only to set a value each frame that's probably already set.
        //     * If it's for debugging, please remove.
        //     * Also the way the these events are created is not right, please use the EventManager.
        //     */
        //    switch (wallHealth)
        //    {
        //        case 0 when !_ciritcalInvooked:
        //            Debug.Log("Hello");
        //            _ciritcalInvooked = true;
        //            OnWallSegmentCritical.Invoke(this);
        //            break;
        //        case > 0 when _ciritcalInvooked:
        //            Debug.Log("It's me");
        //            _ciritcalInvooked = false;
        //            OnWallNotSegmentCritical.Invoke(this);
        //            break;
        //    }
        //}

        /// /////////////////


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
            _meshFilter.mesh = state switch
            {
                0 => destroyedWall,
                1 => damagedWall,
                2 => chippedWall,
                3 => normalWall,
                _ => _meshFilter.mesh
            };
        }

        public void DamageScaffolding()
        {
            if (!scaffoldingPiece) return;
            print("DAmaged me" + scaffoldingHealth);
            scaffoldingHealth -= 1;
            if (scaffoldingHealth <= 0) scaffoldingPiece.SetActive(false);
            if (isSoldierPresent)
            {
                soldier.DieByArrows();
                isSoldierPresent = false;
            }
        }

        public bool RepairScaffolding()
        {
            if (!scaffoldingPiece)
            {
                FloatingTextManager.instance.DoFloatingText("Not Scaffolding!");
                return false;
            }
            if (scaffoldingHealth == scaffoldingMaxHealth)
            {
                FloatingTextManager.instance.DoFloatingText("Not Damaged!");
                return false;
            }
            scaffoldingHealth = Mathf.Min(scaffoldingMaxHealth, scaffoldingHealth + 1);
            scaffoldingPiece.SetActive(true);
            RequestSoldier();
            StartCoroutine(nameof(JuicyRepair));
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
            if (wallHealth == wallMaxHealth)
            {
                FloatingTextManager.instance.DoFloatingText("Not Damaged!");
                return false;
            }
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