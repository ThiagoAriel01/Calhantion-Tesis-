using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class AIWaypoints : MonoBehaviour
{
    [SerializeField] protected float refresh;
    List<Waypoint> points = new List<Waypoint>();

    void Awake()
    {
        Refresh();
    }

    private void OnValidate()
    {
        Refresh();
    }

    void Refresh()
    {
        points.Clear();
        points.AddRange(transform.GetComponentsInChildren<Waypoint>());
    }

    private void OnDrawGizmos()
    {
        foreach (Waypoint w in points)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(w.transform.position,0.5f);
            if (w.isValid)
                Gizmos.DrawLine(w.transform.position, w.connectsWith.transform.position);
        }
    }

    virtual public Waypoint ClosestWaypoint(Vector3 pos)
    {
        float dist = 999f;
        Waypoint closest = points[0];
        for (int i = 0; i < points.Count; i++)
        {
            float curDist = (points[i].transform.position - pos).magnitude;
            if (curDist < dist)
            {
                dist = curDist;
                closest = points[i];
            }
        }
        return closest;
    }

    virtual public void OnUpdate (Enemy enemy, float delta)
    {

    }

    virtual public Waypoint TraverseWaypoints(Enemy enemy)
    {
        Waypoint w = ClosestWaypoint(enemy.transform.position);
        w.OnStart(enemy);
        return w;
    }
}
