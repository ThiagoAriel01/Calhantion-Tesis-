using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState : EnemyState
{
    public CombatState(string p_id) : base(p_id)
    {
    }

    public override bool OnEnter(Enemy e)
    {
        e.SetWalkType(Enemy.WalkType.Run);
        e.PlayAnimation("equipTrigger",0.75f);
        e.enemyAnimador.animador.SetBool("equip", true);
        return true;
    }

    public override bool OnExit(Enemy e)
    {
        e.SetWalkType(Enemy.WalkType.Jog);
        e.PlayAnimation("unequipTrigger", 0.75f);
        e.enemyAnimador.animador.SetBool("equip", false);
        return true;
    }

    public override bool OnUpdate(Enemy e, float delta)
    {
        e.CombatUpdate(delta);
        return true;
    }
}
