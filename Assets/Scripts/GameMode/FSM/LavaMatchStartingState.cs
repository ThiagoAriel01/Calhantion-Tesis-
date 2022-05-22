using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaMatchStartingState : LavaMatchState
{

    public LavaMatchStartingState(string pid, LavaMatch plava) : base(pid, plava)
    {

    }

    public override void Enter(GameModeFSM fsm)
    {
        base.Enter(fsm);
        _lava.t = 0;
        Vector3 bb = _lava.lava.transform.position;
        bb.y = _lava._startinglevel;
        _lava.lava.transform.position = bb;
        _lava.NewRound();
        if (_lava.etapa > 0)
        {
            string str = "Ronda " + (_lava.etapa + 1);
            if (_lava.etapa + 1 >= _lava._maxRounds)
                str = "LAST ROUND!";
            MessageManager.instance.SendMessageToAll(0, str, Color.yellow, 5f);
            ProtoPlayerMP[] mps = GameObject.FindObjectsOfType<ProtoPlayerMP>();
            foreach (ProtoPlayerMP item in mps)
            {
                item.RespawnNow();
            }
        }
        _lava.etapa++;
    }

    public override void UpdateState(GameModeFSM fsm, float deltaTime)
    {
        base.UpdateState(fsm, deltaTime);
        if (_t >= _lava.lava._lavaDelay)
        {
            fsm.ChangeState("flowing");
            return;
        }
    }

}
