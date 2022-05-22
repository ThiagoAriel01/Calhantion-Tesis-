using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class TesisNetworkManager : NetworkManager, INetworkHandler
{
    [SerializeField] protected LobbyPlayerState _lobbyPlayer;
    [SerializeField] protected NetworkIdentity _specPlayer;
    public delegate void TesisNetLobbyD();
    public delegate void TesisNetSpawnD(NetworkConnection conn, PlayerState state);
    public TesisNetSpawnD onPlayerSpawn;
    public TesisNetLobbyD onLobbyConnect;
    public TesisNetLobbyD onLobbyDisconnect;
    public TesisNetLobbyD onClientChangedScene;
    protected bool _inLobby;
    static protected TesisNetworkManager _instancia;
    protected Dictionary<int,ConnectionModel> connectionmodels = new Dictionary<int,ConnectionModel>();

    [System.Serializable]
    protected class ConnectionModel
    {
        [SerializeField] public NetworkConnection conn;
        [SerializeField] public string model;
    }

    static public TesisNetworkManager instancia
    {
        get
        {
            return _instancia;
        }
    }

    public struct SpawnMessage : NetworkMessage
    {
        public bool spawns;
        public string characterModel;
    }

    public struct ServerInfo : NetworkMessage
    {
        public bool inLobby;
        //public string map;
    }

    public override void OnStartServer()
    {
        _inLobby = true;
        base.OnStartServer();
        //string scene = MenuManager.instance.selectedMap;
        //ServerChangeScene(scene);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if (!isNetworkActive)
            return;
        _inLobby = false;
        GameModeManager.instance.StartServer();
    }

    public override void Awake()
    {
        if (_instancia != null)
        {
            Destroy(gameObject);
            return;
        }
        _instancia = this;
        LobbyPlayerState.onAllPlayersReady = OnAllPlayersReady;
        base.Awake();
        (this as INetworkHandler).RegisterHandlers();
        CreateHandlers();
    }

    void INetworkHandler.RegisterHandlers()
    {
        NetworkServer.RegisterHandler<SpawnMessage>(OnSpawnPlayer);
        NetworkClient.RegisterHandler<ServerInfo>(OnReceiveInfo);
    }

    void OnAllPlayersReady()
    {
        _inLobby = false;
        string scene = MenuManager.instance.selectedMap;
        ServerChangeScene(scene);
    }

    static public void CreateHandlers()
    {
        ProtoPlayerMP.onPlayerDied = null;
        NetworkServer.ClearHandlers();
        INetworkHandler[] handlers = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<INetworkHandler>().ToArray();
        for (int i = 0; i < handlers.Length; i++)
        {
            handlers[i].RegisterHandlers();
        }
        INetworkHandler[] skillhandlers = LoadoutManager.instance.skills.OfType<INetworkHandler>().ToArray();
        for (int i = 0; i < skillhandlers.Length; i++)
        {
            skillhandlers[i].RegisterHandlers();
        }
    }

    void OnReceiveInfo (ServerInfo info)
    {
        if (!info.inLobby && !NetworkClient.ready)
            NetworkClient.Ready();
        if (info.inLobby)
        {
            onLobbyConnect?.Invoke();
        }
    }

    void OnSpawnPlayer (NetworkConnection conn, SpawnMessage message)
    {
        //connectionmodels.Add(conn.connectionId, new ConnectionModel() { conn = conn, model = message.characterModel });
        Transform startPos = GameModeManager.instance.currentGameMode.ChooseSpawnLocation();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        //player.GetComponent<PlayerScript>().ChangeModel();
        NetworkServer.AddPlayerForConnection(conn, player);
        player.GetComponent<ProtoPlayerMP>().ChangeModel(message.characterModel);
        onPlayerSpawn?.Invoke(conn, player.GetComponent<PlayerState>());
    }

    [Server]
    public void TrySpawnAllPlayers()
    {
        foreach (KeyValuePair<int, NetworkConnectionToClient> conn in NetworkServer.connections)
        {
            OnSpawnPlayer(conn.Value, new SpawnMessage() { characterModel = connectionmodels[conn.Key].model });
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        TesisNetworkManager.CreateHandlers();
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        onClientChangedScene?.Invoke();
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        // always become ready.
        if (!NetworkClient.ready) NetworkClient.Ready();

        SpawnMessage msg = new SpawnMessage { characterModel = LoadoutManager.instance.GetCharacter() };
        msg.spawns = true;
        // Only call AddPlayer for normal scene changes, not additive load/unload
        NetworkClient.Send(msg);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        if (!_inLobby)
        {
            if (!NetworkClient.ready) NetworkClient.Ready();
            //GameModeManager.instance.RetrieveGameMode();
        }
    }

    void SpawnLobbyPlayer(NetworkConnection conn)
    {
        LobbyPlayerState s = Instantiate(_lobbyPlayer);
        s.gameObject.name = conn.connectionId + " Lobby Player"; 
        NetworkServer.AddPlayerForConnection(conn, s.gameObject);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.LogWarning("connected : " + conn.connectionId);
        if (!NetworkClient.ready) NetworkClient.Ready();
        ServerInfo f = new ServerInfo { inLobby = _inLobby };
        conn.Send(f);
        if (_inLobby)
        {
            SpawnLobbyPlayer(conn);
        }
        //base.OnServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.LogWarning("disconnected : " + conn.connectionId);

        //if (connectionmodels.ContainsKey(conn.connectionId))
       //     connectionmodels.Remove(conn.connectionId);

    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //base.OnServerAddPlayer(conn);
    }
}