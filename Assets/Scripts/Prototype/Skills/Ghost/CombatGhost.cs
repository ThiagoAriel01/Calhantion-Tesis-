using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatGhost : GhostState
{

    public CombatGhost(Ghost g, float moveSpeed) : base(g, moveSpeed)
    {

    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (g.attacking)
            return;
        if (g.target == null)
        {
            onRequestChange?.Invoke("idle");
        }
        else
        {
            Vector3 v = g.target.transform.position - g.transform.position;
            g.transform.rotation = Quaternion.LookRotation(v);
            float dist = v.magnitude;
            v.Normalize();
            v *= _moveSpeed;
            g.transform.position += v * deltaTime;
            if (dist <= g.atkRange)
            {
                g.Attack();
            }
        }
    }
}