using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingWaypointState : EnemyState
{

    public FollowingWaypointState(string p_id) : base (p_id)
    {
    }

    public override bool OnExit(Enemy e)
    {
        e.StopWaypoints();
        return true;
    }

    public override bool OnUpdate(Enemy e, float delta)
    {
        e.WaypointUpdate();
        return true;
    }
}
