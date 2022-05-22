using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaMatchEndingState : LavaMatchState
{
    public LavaMatchEndingState(string pid, LavaMatch plava) : base(pid, plava)
    {

    }

    public override void Enter(GameModeFSM fsm)
    {
        base.Enter(fsm);
        _lava.FinishGame();
    }

}