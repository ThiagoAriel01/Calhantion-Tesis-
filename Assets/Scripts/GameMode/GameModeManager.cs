using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameModeManager : MonoSingleton<GameModeManager>, INetworkHandler
{
    [SerializeField] protected int _defaultGameMode;
    protected int _selectedGameMode;
    protected GameMode _instancedGameMode;
    [SerializeField] protected GameMode[] _gamemodes;
    public delegate void GameModeD(int i);
    public delegate void GameModeVoidD();
    public delegate void GameModeMsgD(GameEndedMsg msg);
    public GameModeD onReceivedGameMode;
    public GameModeVoidD onGameModeStart;
    public GameModeMsgD onClientGameEnded;
    protected bool _synced;
    protected bool _gameEndedSucessfully;

    public struct GameEndedMsg : NetworkMessage
    {
        public SPlayerStats[] players;
        public SPlayerStats localPlayer;
        public bool victory;
    }

    [System.Serializable]
    public struct SPlayerStats
    {
        [SerializeField] public string name;
        [SerializeField] public int kills;
        [SerializeField] public int deaths;
        [SerializeField] public float damage;
        [SerializeField] public string teamName;
        [SerializeField] public string model;
        [SerializeField] public Color teamColor;
    }

    public struct GameModeMessage : NetworkMessage
    {
        public int gamemodeIndex;
    }

    public void SelectGameMode (int i)
    {
        _selectedGameMode = i;
    }

    public GameMode selectedGameMode
    {
        get
        {
            return _gamemodes[_selectedGameMode];
        }
    }

    public GameMode currentGameMode
    {
        get => _instancedGameMode;
    }
    public GameMode[] gamemodes
    {
        get => _gamemodes;
    }
    public bool sync
    {
        get => _synced;
    }

    protected override bool Awake()
    {
        if (!base.Awake())
            return false;
        (this as INetworkHandler).RegisterHandlers();
        //NetworkServer.RegisterHandler<GameModeMessage>(SendGameMode);
        //NetworkClient.RegisterHandler<GameModeMessage>(ReceiveGameMode);
        return true;
    }

    void INetworkHandler.RegisterHandlers()
    {
        NetworkClient.RegisterHandler<GameEndedMsg>(OnGameEndedMsg);
    }

    [Client]
    void OnGameEndedMsg(GameEndedMsg msg)
    {
        onClientGameEnded?.Invoke(msg);
    }

    [Server]
    public void StartServer()
    {
        _gameEndedSucessfully = false;
        _synced = true;
        _instancedGameMode = Instantiate(_gamemodes[_selectedGameMode]);
        _instancedGameMode.onEnd += OnEnd;
        _instancedGameMode.StartGameMode();
        _instancedGameMode.SpawnNetworkState();
        ProtoPlayerMP.onPlayerDied += (x,y,z) => { _instancedGameMode.OnPlayerKilled(x, y, z); };
        TesisNetworkManager.instancia.onPlayerSpawn += OnPlayerSpawn;
        onGameModeStart?.Invoke();
    }

    [Server]
    void OnPlayerSpawn(NetworkConnection conn, PlayerState state)
    {
        if (currentGameMode == null)
            return;
        currentGameMode.OnPlayerSpawn(conn, state);
    }

    [Server]
    void OnEnd()
    {
        _gameEndedSucessfully = true;
        Invoke("CloseServer",5f);
    }

    [Server]
    void CloseServer()
    {
        CancelInvoke();
        _gameEndedSucessfully = true;
        List<PlayerState> states = PlayerState.allPlayers;
        SPlayerStats[] stats = new SPlayerStats[states.Count];
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i] = new SPlayerStats
            {
                name = states[i].GetComponent<PlayerNET>().nickName,
                kills = states[i].kills,
                deaths = states[i].deaths,
                damage = states[i].damage,
                model = states[i].GetComponent<ProtoPlayerMP>().modelName
            };
            if (GameModeNetworkState.instance.HasTeams() && GameModeNetworkState.instance.teams != null && GameModeNetworkState.instance.teams.Count > states[i].teamIndex) 
            {
                stats[i].teamName = GameModeNetworkState.instance.teams[states[i].teamIndex]._name;
                stats[i].teamColor = GameModeNetworkState.instance.teams[states[i].teamIndex]._color;
            }
        }
        foreach (PlayerState state in states)
        {
            SPlayerStats localPlayer = new SPlayerStats
            {
                name = state.GetComponent<PlayerNET>().nickName,
                kills = state.kills,
                deaths = state.deaths,
                damage = state.damage,
                model = state.GetComponent<ProtoPlayerMP>().modelName
            };
            if (GameModeNetworkState.instance.HasTeams() &&  GameModeNetworkState.instance.teams != null && GameModeNetworkState.instance.teams.Count > state.teamIndex)
            {
                localPlayer.teamName = GameModeNetworkState.instance.teams[state.teamIndex]._name;
                localPlayer.teamColor = GameModeNetworkState.instance.teams[state.teamIndex]._color;
            }
            GameEndedMsg msg = new GameEndedMsg()
            {
                players = stats,
                localPlayer = localPlayer,
                victory = _instancedGameMode.winners.Contains(state)
            };
            state.connectionToClient.Send(msg);
        }
        Invoke("CloseNow",1f);
    }

    public void Clear()
    {
        CancelInvoke();
        _instancedGameMode = null;
        _synced = false;
    }

    void CloseNow()
    {
        CancelInvoke();
        _instancedGameMode = null;
        _synced = false;
        List<PlayerState> states = PlayerState.allPlayers;
        for (int i = 0; i < states.Count; i++)
        {
            states[i].connectionToClient.Disconnect();
        }
        TesisNetworkManager.instancia.StopHost();
        TesisNetworkManager.instancia.StopServer();
        TesisNetworkManager.instancia.StopClient();
        NetworkServer.DisconnectAll();
        NetworkServer.Shutdown();
        TesisNetworkManager.CreateHandlers();
        _selectedGameMode = 0;
    }

    private void Update()
    {
        if (_instancedGameMode != null)
            _instancedGameMode.UpdateGameMode(Time.deltaTime);
    }



    /*
    [Server]
    void SendGameMode(NetworkConnection conn, GameModeMessage msg)
    {
        msg.gamemodeIndex = _currentGameMode;
        conn.Send(msg);
    }

    [Client]
    void ReceiveGameMode (GameModeMessage msg)
    {
        _currentGameMode = msg.gamemodeIndex;
        _synced = true;
        onReceivedGameMode?.Invoke(_currentGameMode);
    }

    [Client]
    public void RetrieveGameMode()
    {
        GameModeMessage g = new GameModeMessage() { gamemodeIndex = -1};
        NetworkClient.Send(g);
    }
    */
}