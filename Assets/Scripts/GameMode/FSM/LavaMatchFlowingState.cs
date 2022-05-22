using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaMatchFlowingState : LavaMatchState
{
    public LavaMatchFlowingState(string pid, LavaMatch plava) : base(pid, plava)
    {

    }

    public override void Enter(GameModeFSM fsm)
    {
        base.Enter(fsm);
        MessageManager.instance.SendMessageToAll(0, "Lava starts flowing...", Color.yellow, 5f);
        _lava.t = 0.0f;
    }

    public override void UpdateState(GameModeFSM fsm, float deltaTime)
    {
        base.UpdateState(fsm, deltaTime);
        float spd = Mathf.Clamp(_lava.lava._lavaSpeed + _lava.etapa * _lava.lava._lavaSpeedPerEtapa, 0f, _lava.lava._lavaMaxSpeed);
        float duration = ((_lava.lava._lavaMaxlevel - _lava._startinglevel) / spd);
        _lava.t += deltaTime / duration;
        Vector3 v = _lava.lava.transform.position;
        v.y = Mathf.Lerp(_lava._startinglevel, _lava.lava._lavaMaxlevel, _lava.t);
        _lava.lava.transform.position = v;
        if (_lava.t >= 1f)
        {
            fsm.ChangeState("top");
            return;
        }
    }
}
