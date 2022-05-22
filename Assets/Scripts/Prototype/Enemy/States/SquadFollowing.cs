using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadFollowing : EnemyState
{
    public SquadFollowing(string p_id) : base(p_id)
    {
    }

    public override bool OnExit(Enemy e)
    {
        return true;
    }

    public override bool OnUpdate(Enemy e, float delta)
    {
        e.SquadFollowUpdate(delta);
        return true;
    }
}
