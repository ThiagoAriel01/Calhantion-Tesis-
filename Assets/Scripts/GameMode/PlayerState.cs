using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerState : NetworkBehaviour
{
    [SyncVar] protected int _kills;
    [SyncVar] protected int _deaths;
    [SyncVar(hook = nameof(OnDamageChanged))] protected float _damage;
    [SyncVar(hook = nameof(OnTeamChanged))] protected int _teamIndex=-1;
    [SyncVar] protected bool _alive = true;
    [SyncVar] protected GameObject _currentGhost;
    protected float _deadTime;
    public delegate void PlayerStateD(PlayerState state);
    public delegate void PlayerStateteamD(int newTeam);
    static private List<PlayerState> _states = new List<PlayerState>();
    static public PlayerStateD onPlayerAdded;
    public PlayerStateD onDamagedChanged;
    static public PlayerStateD onPlayerRemoved;
    public PlayerStateteamD onTeamChanged;
    public PlayerStateteamD onServerTeamChanged;
    protected PlayerSkills _skills;

    static public List<PlayerState> allPlayers
    {
        get
        {
            return _states;
        }
    }

    public void ChangeTeam(int newteam)
    {
        CmdChangeTeam(newteam);
    }

    void OnTeamChanged(int oldv, int newv)
    {
        onTeamChanged?.Invoke(newv);
    }

    [Command]
    void CmdChangeTeam(int newTeam)
    {
        _teamIndex = newTeam;
        onServerTeamChanged?.Invoke(newTeam);
    }

    [Client]
    void OnDamageChanged(float oldv, float newv)
    {
        onDamagedChanged?.Invoke(this);
    }


    [Server]
    public void OnKillConfirm(string sk)
    {
        TargetKillConfirm(sk);
    }

    [TargetRpc]
    void TargetKillConfirm(string sk)
    {
        if (!isLocalPlayer)
            return;
        _skills.ResetCooldowns(sk);
    }

    public int kills
    {
        get => _kills;
        set => _kills = value;
    }
    public int deaths
    {
        get => _deaths;
        set => _deaths = value;
    }

    public GameObject currentGhost
    {
        get => _currentGhost;
        set => _currentGhost = value;
    }

    public int teamIndex
    {
        get => _teamIndex;
        set => _teamIndex = value;
    }
    public bool alive
    {
        get => _alive;
        set => _alive = value;
    }
    public float deadTime
    {
        get => _deadTime;
        set => _deadTime = value;
    }

    public float damage
    {
        get => _damage;
        set => _damage = value;
    }

    private void Awake()
    {
        _skills = GetComponent<PlayerSkills>();
        _states.Add(this);
        onPlayerAdded?.Invoke(this);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        GetComponent<Character>().onAttacked += OnAttacked;
    }

    void OnAttacked(GameObject victim, Character attacker, SHitInfo info)
    {
        _damage += info.dmg;
    }

    private void OnDestroy()
    {
        _states.Remove(this);
        onPlayerRemoved?.Invoke(this);
    }

    private void Update()
    {
        if (!isServer)
            return;

        if (!_alive)
            _deadTime += Time.deltaTime;
        else
            _deadTime = 0.0f;

    }

}