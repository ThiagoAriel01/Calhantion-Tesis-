using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDMStartingState : TeamDeathMatchState
{
    [SerializeField] protected float _waitTime=10f;

    public TDMStartingState (string pid, float waitTime=10f) : base(pid)
    {
        _waitTime = waitTime;
    }

    public override void UpdateState(GameModeFSM fsm, float deltaTime)
    {
        base.UpdateState(fsm, deltaTime);
        if (_t>= _waitTime)
        {
            fsm.ChangeState("base");
        }
    }

    public override bool IsSpawningSpectators()
    {
        return true;
    }
}