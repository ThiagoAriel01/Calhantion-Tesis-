using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleGhost : GhostState
{
    public IdleGhost(Ghost g,float moveSpeed) : base(g,moveSpeed)
    {

    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        g.transform.position = Vector3.Lerp(g.transform.position, g.idlePosition, _moveSpeed * deltaTime);
        g.transform.rotation = Quaternion.Lerp(g.transform.rotation,g.owner.transform.rotation, _moveSpeed * deltaTime);
        if (g.target != null)
        {
            onRequestChange?.Invoke("combat");
        }
    }
}