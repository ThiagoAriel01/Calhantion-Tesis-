using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyPlayerState : NetworkBehaviour
{
    public delegate void LPSD(LobbyPlayerState s);
    public delegate void LPSV();
    static public LPSD onPlayerAdded;
    static public LPSD onPlayerRemoved;
    static public LPSV onAllPlayersReady;
    [SyncVar] protected string _nickName;
    [SyncVar] protected bool _ready;
    static protected LobbyPlayerState _localState;
    static protected List<LobbyPlayerState> _allStates = new List<LobbyPlayerState>();

    static public List<LobbyPlayerState> allStates
    {
        get
        {
            return _allStates;
        }
    }

    public string nickname
    {
        get
        {
            return _nickName;
        }
    }

    public bool ready
    {
        get
        {
            return _ready;
        }
    }

    static public LobbyPlayerState localState
    {
        get
        {
            return _localState;
        }
    }

    private void Awake()
    {
        _allStates.Add(this);
        onPlayerAdded?.Invoke(this);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        _localState = this;
        InitNickName(PlayerPrefs.GetString("PlayerNickName", "Player"));
    }

    static void CheckAllReady()
    {
        foreach (var item in _allStates)
        {
            if (!item.ready)
                return;
        }
        onAllPlayersReady?.Invoke();
    }

    private void OnDestroy()
    {
        _allStates.Remove(this);
        onPlayerRemoved?.Invoke(this);
    }

    [Client]
    public void InitNickName(string nick)
    {
        CmdNickSend(nick);
    }

    [Command]
    void CmdNickSend (string s)
    {
        _nickName = s;
    }

    [Client]
    public void SetReady()
    {
        CmdReadySend(true);
    }

    [Command]
    void CmdReadySend(bool v)
    {
        _ready = v;
        CheckAllReady();
    }

}
