using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerNET : NetworkBehaviour 
{
    internal static readonly List<PlayerNET> playersList = new List<PlayerNET>();
    public delegate void PlayerNETDelegate(PlayerNET p_plr);
    static public PlayerNETDelegate onPlayerAdded;
    static public PlayerNETDelegate onPlayerRemove;
    static public PlayerNETDelegate onPlayerRefresh;
    public PlayerNETDelegate onChangedNickName;
    static private PlayerNET _localInstance = null;

    [SyncVar(hook = nameof(OnChangedNickName))]
    protected string _nickName;

    [SyncVar(hook = nameof(OnChangedScore))]
    protected int _score;

    public int score
    {
        get => _score;
    }

    static public PlayerNET localInstance
    {
        get => _localInstance;
    }

    public string nickName
    {
        get
        {
            return _nickName;
        }
        set
        {
            _nickName = value;
        }
    }

    void OnChangedScore(int oldValue, int newValue)
    {
        _score = newValue;
    }

    [Server]
    public void AddScore()
    {
        _score++;
    }

    void OnChangedNickName (string old, string p_new)
    {
        _nickName = p_new;
        onChangedNickName?.Invoke(this);
    }

    static public PlayerNET GetPlayer (int index)
    {
        return playersList[index];
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetNickName(PlayerPrefs.GetString("PlayerNickName", ""));
        _localInstance = this;
    }

    [Command]
    void CmdSetNickName (string p_nick)
    {
        _nickName = p_nick;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        onPlayerAdded?.Invoke(this);
        onPlayerRefresh?.Invoke(this);
        playersList.Add(this);
    }

    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistent storage</para>
    /// </summary>
    public override void OnStopServer()
    {
        onPlayerRemove?.Invoke(this);
        onPlayerRefresh?.Invoke(this);
        playersList.Remove(this);
    }
}
