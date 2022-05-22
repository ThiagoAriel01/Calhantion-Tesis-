using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class GameModeState
{
    protected float _t;
    protected float _respawnTime=5;
    protected string _id;

    public GameModeState (string pid)
    {
        _id = pid;
    }

    public string ID
    {
        get
        {
            return _id;
        }
    }

    virtual public void Enter(GameModeFSM fsm)
    {
        _t = 0.0f;
    }
    virtual public void Exit(GameModeFSM fsm)
    {
        _t = 0.0f;
    }

    virtual public void NewRound()
    {

    }

    virtual public bool CanRespawn(PlayerState state)
    {
        return !state.alive && state.deadTime >= _respawnTime;
    }

    virtual public Transform ChooseSpawnLocation()
    {
        return TesisNetworkManager.instancia.GetStartPosition();
    }

    virtual public void OnPlayerSpawn (NetworkConnection conn, PlayerState state, GameMode gm)
    {

    }

    virtual public Transform ChooseSpawnLocation(PlayerState state)
    {
        //if (_teamBasedSpawns)
        //{
        //    return NetworkManager.singleton.GetStartPositionName(("TEAM:" + state.teamIndex));
        //}
        //else
        //{
            return ChooseSpawnLocation();
        //}
    }

    virtual public void OnPlayerKilled(PlayerState victim, PlayerState attacker, ProtoPlayerMP.KillContext context)
    {
        if (victim != null)
        {
            victim.deaths += 1;
        }
        if (attacker != null)
        {
            attacker.kills += 1;
        }
        if (victim != null && attacker != null)
        {
            Debug.Log("skillused : " + context.skillused);
            SkillData d = LoadoutManager.instance.GetSkill(context.skillused);
            CharacterData dat = LoadoutManager.instance.GetCharacterData(attacker.GetComponent<ProtoPlayerMP>().modelName);
            CharacterData datvictim = LoadoutManager.instance.GetCharacterData(victim.GetComponent<ProtoPlayerMP>().modelName);
            if (d == null)
            {
                KillNotifiesManager.instance.SendNotifyToAll(
                "Icons/" + dat.name,
                attacker.GetComponent<PlayerNET>().nickName,
                ("Skills/" + context.skillused),
                victim.GetComponent<PlayerNET>().nickName,
                "Icons/" + datvictim.name
                );
                return;
            }
            else
            {
                KillNotifiesManager.instance.SendNotifyToAll(
                "Icons/" + dat.name,
                attacker.GetComponent<PlayerNET>().nickName,
                d != null ? ("Skills/" + d.Icon.name) : "",
                victim.GetComponent<PlayerNET>().nickName,
                "Icons/" + datvictim.name
                );
            }
        }
    }

    virtual public bool ShowsKillNotify()
    {
        return true;
    }

    virtual public bool CanDamagePlayer (PlayerState attacker, PlayerState victim)
    {
        //if (_teamBased)
        //{
        //    return attacker.teamIndex != victim.teamIndex;
        //}
        //else
        //{
            return true;
        //}
    }

    virtual public bool IsSpawningSpectators()
    {
        return false;
    }

    virtual public void UpdateState (GameModeFSM fsm, float deltaTime)
    {
        _t += deltaTime;
    }
}
