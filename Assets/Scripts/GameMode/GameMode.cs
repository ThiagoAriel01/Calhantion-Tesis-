using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class GameMode : ScriptableObject
{
    public delegate void GameModeDD();
    public GameModeDD onNewRound;
    public GameModeDD onEnd;
    [SerializeField] protected float _baseRespawnTime;
    [SerializeField] protected bool _teamBased = false;
    [SerializeField] protected bool _teamBasedSpawns = false;
    [SerializeField] protected GameModeNetworkState _netState;
    protected GameModeNetworkState _instancedNetState;
    protected GameModeFSM _fsm;
    protected List<PlayerState> _winners;
    protected PlayerState[] _losers;

    public List<PlayerState> winners
    {
        get
        {
            return _winners;
        }
    }
    public PlayerState[] losers
    {
        get
        {
            return _losers;
        }
    }

    virtual public float respawnTime
    {
        get
        {
            return _baseRespawnTime;
        }
    }

    public void NewRound()
    {
        _fsm.currentState.NewRound();
        onNewRound?.Invoke();
    }

    private void Awake()
    {

    }

    virtual public void StartGameMode()
    {

    }

    virtual public void UpdateGameMode(float deltaTime)
    {
        if (_fsm != null)
            _fsm.UpdateFSM(deltaTime);
    }

    virtual public bool CanRespawn(PlayerState state)
    {
        return _fsm.currentState.CanRespawn(state);
    }

    virtual public Transform ChooseSpawnLocation()
    {
        return _fsm.currentState.ChooseSpawnLocation();
    }

    virtual public Transform ChooseSpawnLocation(PlayerState state)
    {
        return _fsm.currentState.ChooseSpawnLocation(state);
    }

    virtual public void OnPlayerKilled(PlayerState victim, PlayerState attacker, ProtoPlayerMP.KillContext context)
    {
        _fsm.currentState.OnPlayerKilled(victim, attacker, context);
    }

    virtual public bool IsSpawningSpectators()
    {
        return _fsm.currentState.IsSpawningSpectators();
    }

    virtual public bool ShowsKillNotify()
    {
        return _fsm.currentState.ShowsKillNotify();
    }

    [Server]
    virtual public GameModeNetworkState SpawnNetworkState()
    {
        _instancedNetState = Instantiate(_netState,Vector3.zero,Quaternion.identity);
        NetworkServer.Spawn(_instancedNetState.gameObject);
        _instancedNetState.Init();
        return _instancedNetState;
    }

    virtual public void OnPlayerSpawn (NetworkConnection conn, PlayerState state)
    {
        _fsm.currentState.OnPlayerSpawn(conn, state, this);
    }

    virtual public bool CanDamagePlayer (PlayerState attacker, PlayerState victim)
    {
        return _fsm.currentState.CanDamagePlayer(attacker,victim);
    }

    virtual public void FinishGame()
    {
        PlayerState[] winners = DetermineWinners();
        List<PlayerState> losers = new List<PlayerState>();
        losers.AddRange(PlayerState.allPlayers);
        if (winners != null && winners.Length > 0)
        {
            foreach (var item in winners)
            {
                losers.Remove(item);
            }
        }
        foreach (var item in PlayerState.allPlayers)
        {
            ProtoPlayerMP mp = item.GetComponent<ProtoPlayerMP>();
            mp.totalInputFreeze = true;
            mp.plr.invunerable = true;
        }
        foreach (var item in losers)
        {
            MessageManager.instance.EndToEspecific(item.connectionToClient, 0, false);
        }
        if (winners != null && winners.Length > 0)
        {
            foreach (var item in winners)
            {
                if (item != null)
                    MessageManager.instance.EndToEspecific(item.connectionToClient, 0, true);
            }
        }
        _winners = new List<PlayerState>();
        if (winners != null && winners.Length > 0)
        {
            _winners.AddRange(winners);
        }
        _losers = losers.ToArray();
        onEnd?.Invoke();
    }

    virtual public PlayerState[] DetermineWinners()
    {
        PlayerState[] winners = new PlayerState[1];
        PlayerState top = null;
        int kills = 0;
        foreach (var item in PlayerState.allPlayers)
        {
            if (item.kills > kills)
            {
                kills = item.kills;
                top = item;
            }
        }
        winners[0] = top;
        return winners;
    }
}