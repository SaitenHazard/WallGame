using Enemies;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LeviatedSpringDefense : CooldownUpgrade
{
    [Header("__________ Balancing __________")]

    public int enemyDamage = 5;
    public float projectileFlightTime = 2.0f;

    [Space]
    [Header("__________ References __________")]

    public Transform projectileSpawnPoint;
    public TargetProjectile trebuchetRound;

    public override void Engage()
    {
        TargetProjectile projectile = Instantiate(trebuchetRound, projectileSpawnPoint.position, Quaternion.identity);
        projectile.SetDestination(ArmyController.instance.GetFootsoldierPosition());
        ArmyController.instance.Invoke(nameof(ArmyController.BombArrives), projectileFlightTime);
        projectile.SetFlightTime(projectileFlightTime);
    }
}
