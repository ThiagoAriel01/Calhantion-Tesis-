using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GhostState : State
{
    protected float _moveSpeed;
    protected Ghost g;

    public GhostState (Ghost g,float moveSpeed)
    {
        this.g = g;
        _moveSpeed = moveSpeed;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }
}