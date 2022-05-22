using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public float stayTime;
    public Enemy.WalkType walkType;
    public Waypoint connectsWith;

    public bool isValid
    {
        get
        {
            return connectsWith != null;
        }
    }

    public float distanceToConnection
    {
        get
        {
            return (transform.position - connectsWith.transform.position).magnitude;
        }
    }

    virtual public void OnStart(Enemy enemy)
    {
        enemy.SetWalkType(walkType);
        enemy.SetDestination(this);
    }

    virtual public void OnUpdate(Enemy enemy, float delta)
    {
        float d = (enemy.transform.position - transform.position).magnitude;
        if (d <= enemy.stoppingDistance + 1f)
        {
            if (!enemy.reachedWaypoint)
                enemy.OnReachWaypoint(this);
            if (enemy.reachedWaypoint && enemy.waypointTimer >= stayTime)
                OnEnd(enemy);
        }
    }

    virtual public void OnEnd(Enemy enemy)
    {
        enemy.SetWaypoint(connectsWith);
    }
}
