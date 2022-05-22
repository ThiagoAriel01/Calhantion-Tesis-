using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemy : Enemy
{
    [SerializeField] protected Attack basicMelee;
    [SerializeField] protected float range = 5f;
    [SerializeField] protected float proyectileCooldown = 10f;
    protected float pCooldown = 0;
    [SerializeField] protected float proyectileRangeMin = 15f;
    [SerializeField] protected ProyectileAttack basicProyectile;

    public override void CombatUpdate(float delta)
    {
        base.CombatUpdate(delta);
        if (currentAttack == null && currentProyectile==null && target!=null)
        {
            if ((target.transform.position - transform.position).magnitude <= range)
            {
                PerformAttack(basicMelee, target.gameObject);
            }
            else if(pCooldown<=0 && (target.transform.position - transform.position).magnitude >= proyectileRangeMin && !inSquad
                 && targetInLineOfSight)
            {
                PerformProyectileAttack(basicProyectile,target.transform);
                if (squadData!=null)
                {
                    foreach (var item in squadData.enemiesInSquad)
                    {
                        if (item != this)
                            item?.PerformProyectileAttack(basicProyectile, target.transform);
                    }
                }
                pCooldown = proyectileCooldown;
            }
        }
    }
    override protected void CooldownsUpdate()
    {
        HandleCooldown(ref pCooldown);
    }
}