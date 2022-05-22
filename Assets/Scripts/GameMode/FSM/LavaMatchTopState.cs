using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaMatchTopState : LavaMatchState
{
    private float _rate;
    private float _timer;

    public LavaMatchTopState(string pid, LavaMatch plava) : base(pid, plava)
    {

    }

    public override void Enter(GameModeFSM fsm)
    {
        base.Enter(fsm);
        _timer = _lava.lava._lavaEndDelay;
    }

    public override void UpdateState(GameModeFSM fsm, float deltaTime)
    {
        base.UpdateState(fsm, deltaTime);
        _timer -= deltaTime;
        _rate -= deltaTime;
        if (_rate <= 0.0f)
        {
            string str = "New Round in ";
            if (_lava.etapa >= _lava._maxRounds)
                str = "Game Ends in ";
            str += ((int)_timer).ToString();
            if (_timer <= 10)
            {
                AudioCue cue = Resources.Load<AudioCue>("AudioCue/" + "TimerCounter");
                if (cue != null)
                    cue.PlaySound();
            }
            MessageManager.instance.SendMessageToAll(0, str, Color.yellow, 1f);
            _rate = 1f;
        }
        if (_timer <= 0)
        {
            if (_lava.etapa >= _lava._maxRounds)
                fsm.ChangeState("ending");
            else
                fsm.ChangeState("starting");
            return;
        }
    }

}
