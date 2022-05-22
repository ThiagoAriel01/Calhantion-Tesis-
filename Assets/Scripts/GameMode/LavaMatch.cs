using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/GameModes/Lava Match")]
public class LavaMatch : GameMode
{
    public int _maxRounds=5;
    [HideInInspector] public float _startinglevel;
    protected LavaFloor _lava;
    protected int _etapa;
    protected bool reachedEnd;
    [HideInInspector] public float t;
    protected float endt;
    [HideInInspector] public float remaining;
    protected bool started;

    public int etapa
    {
        get
        {
            return _etapa;
        }
        set
        {
            _etapa = value;
        }
    }

    public LavaFloor lava
    {
        get
        {
            return _lava;
        }
    }

    public override void StartGameMode()
    {
        base.StartGameMode();

        _lava = GameObject.FindObjectOfType<LavaFloor>();
        _startinglevel = _lava.transform.position.y;
        t = 0;
        LavaMatchStartingState starting = new LavaMatchStartingState("starting", this);
        LavaMatchFlowingState flowing = new LavaMatchFlowingState("flowing", this);
        LavaMatchTopState top = new LavaMatchTopState("top", this);
        LavaMatchEndingState ending = new LavaMatchEndingState("ending", this);
        GameModeState[] states = new GameModeState[]
        {
            starting,
            flowing,
            top,
            ending
        };
        _fsm = new GameModeFSM(states, starting);
        _fsm.ChangeState(starting);
    }

    public override void UpdateGameMode(float deltaTime)
    {
        base.UpdateGameMode(deltaTime);
        if (_lava == null)
            return;
        /*float spd = Mathf.Clamp(_lava._lavaSpeed + _etapa * _lava._lavaSpeedPerEtapa,0f,_lava._lavaMaxSpeed);
        float duration = ((_lava._lavaMaxlevel - _startinglevel) / spd);
        t += deltaTime / duration;
        if (t>=0 && !started)
        {
            started = true;
        }
        Vector3 v = _lava.transform.position;
        v.y = Mathf.Lerp(_startinglevel, _lava._lavaMaxlevel, t);
        _lava.transform.position = v;
        if (t > 1 && !reachedEnd)
        {
            endt = 0.0f;
            reachedEnd = true;
            remaining = _lava._lavaEndDelay;
        }
        if (reachedEnd)
        {
            endt -= deltaTime;
            remaining -= deltaTime;
            if (endt <= 0.0f)
            {
                MessageManager.instance.SendMessageToAll(0, ((int)remaining).ToString(), Color.yellow, 1f);
                endt = 0.1f;
            }
        }*/
        /*
        if (t > 1 + _lava._lavaEndDelay / duration)
        {
            t = -_lava._lavaDelay / duration;
            Vector3 bb = _lava.transform.position;
            bb.y = _startinglevel;
            _lava.transform.position = bb;
            reachedEnd = false;
            remaining = 0.0f;
            started = false;
            endt = 0.0f;
            _etapa++;
            NewRound();
            ProtoPlayerMP[] mps = GameObject.FindObjectsOfType<ProtoPlayerMP>();
            foreach (ProtoPlayerMP item in mps)
            {
                item.RespawnNow();
            }
        }*/
    }

    public override Transform ChooseSpawnLocation(PlayerState state)
    {
        return base.ChooseSpawnLocation(state);
    }

    public override Transform ChooseSpawnLocation()
    {
        return base.ChooseSpawnLocation();
    }
}
