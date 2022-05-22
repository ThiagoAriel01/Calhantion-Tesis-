using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ControlPoint : NetworkBehaviour
{
    [SerializeField] protected char _label;
    [SerializeField] protected int _scoreAdd;
    [SerializeField] protected float _scoreRate;
    [SerializeField] protected float _captureDuration = 15f;
    [SerializeField] protected float _captureDecay = 15f;
    [SerializeField] protected float _captureNoneDecay = 30f;
    protected List<PlayerState> _playersInside = new List<PlayerState>();
    protected CapsuleCollider _capsule;
    [SyncVar] protected float _capturingT;
    protected float _scoreT;
    [SyncVar] protected TeamDeathmatch.Team _capturingBy;
    [SyncVar] protected TeamDeathmatch.Team _domTeam;

    [SyncVar(hook = nameof(OnClientCaptured))] protected TeamDeathmatch.Team _teamCaptured = null;

    [SerializeField] protected Renderer[] _teamColoredrenderers;
    public char label
    {
        get
        {
            return _label;
        }
    }

    public float capturingT
    {
        get
        {
            return _capturingT;
        }
    }

    public TeamDeathmatch.Team capturingBy
    {
        get
        {
            return _capturingBy;
        }
    }

    public TeamDeathmatch.Team teamCaptured
    {
        get
        {
            return _teamCaptured;
        }
    }

    public TeamDeathmatch.Team domTeam
    {
        get
        {
            return _domTeam;
        }
    }

    private void Awake()
    {
        _capsule = GetComponent<CapsuleCollider>();
        RefreshColor();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        RefreshColor();
    }

    void RefreshColor()
    {
        foreach (var item in _teamColoredrenderers)
        {
            item.material.SetColor("_Color", _teamCaptured == null ? Color.white : _teamCaptured._color);
        }
    }

    void OnClientCaptured (TeamDeathmatch.Team oldTeam, TeamDeathmatch.Team newTeam)
    {
        RefreshColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;
        PlayerState ps = other.GetComponent<PlayerState>();
        if (ps == null)
            return;
        OnPlayerEnter(ps);
    }

    protected TeamDeathmatch.Team[] GetTeams()
    {
        try
        {
            TeamDeathmatch tdm = (GameModeManager.instance.currentGameMode as TeamDeathmatch);
            return tdm.GetTeams();
        }
        catch
        {

        }
        return null;
    }

    protected int[] GetCapturingPlayers(TeamDeathmatch.Team[] p_teams)
    {
        int[] playersPerTeam = new int[p_teams.Length];
        foreach (var item in _playersInside)
        {
            if (item.alive && item.teamIndex>=0)
            {
                playersPerTeam[item.teamIndex]++;
            }
        }
        return playersPerTeam;
    }

    protected int GetDominationTeam(int[] playersPerTeam)
    {
        int max = 0;
        int teamsGreaterOne = 0;
        int maxTeamIndex = -1;
        for (int i = 0; i < playersPerTeam.Length; i++)
        {
            if (playersPerTeam[i] > 0)
                teamsGreaterOne++;
            if (playersPerTeam[i] > max)
            {
                max = playersPerTeam[i];
                maxTeamIndex = i;
            }
        }
        if (teamsGreaterOne >= 2 || teamsGreaterOne<=0)
        {
            return -1;
        }
        return maxTeamIndex;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isServer)
            return;
        PlayerState ps = other.GetComponent<PlayerState>();
        if (ps == null)
            return;
        OnPlayerExit(ps);
    }

    private void FixedUpdate()
    {
        if (!isServer)
            return;

        if (_teamCaptured != null)
        {
            _scoreT += Time.fixedDeltaTime / _scoreRate;
            if (_scoreT >= 1f)
            {
                TeamDeathmatch tdm = GameModeManager.instance.currentGameMode as TeamDeathmatch;
                if (tdm != null)
                {
                    tdm.AddTeamScore(_teamCaptured._name, _scoreAdd);
                }
                _scoreT = .0f;
            }
        }

        _playersInside.RemoveAll(x => !x.alive || x == null);

        int[] capturingPlayers = GetCapturingPlayers(GetTeams());
        int domteamindex = GetDominationTeam(capturingPlayers);
        TeamDeathmatch.Team domteam = domteamindex>=0 ? GetTeams()[domteamindex] : null;
        _domTeam = domteam;
        if (domteam == null)
        {
            bool allzero = true;
            for (int i = 0; i < capturingPlayers.Length; i++)
            {
                if (capturingPlayers[i] > 0)
                {
                    allzero = false;
                    break;
                }
            }
            if (allzero && _capturingBy != null)
            {
                _capturingT -= Time.fixedDeltaTime / (_captureNoneDecay);
                if (_capturingT <= 0)
                {
                    _capturingT = .0f;
                    _capturingBy = null;
                }
            }
            return;
        }
        int amountInTeam = capturingPlayers[domteamindex];
        if (_capturingBy == null)
        {
            if (_capturingT <= 0 && _teamCaptured != domteam)
            {
                _capturingT = .0f;
                _capturingBy = domteam;
            }
        }
        else
        {
            if (_capturingBy == domteam)
            {
                _capturingT += Time.fixedDeltaTime / (_captureDuration / amountInTeam);
                if (_capturingT >= 1f)
                {
                    _capturingBy = null;
                    _teamCaptured = domteam;
                    _capturingT = 0.0f;
                }
            }
            else if (_capturingBy != null)
            {
                _capturingT -= Time.fixedDeltaTime / (_captureDecay / amountInTeam);
            }
            if (_capturingT <= 0)
            {
                _capturingT = .0f;
                _capturingBy = null;
            }
        }
    }

    [Server]
    void OnPlayerEnter(PlayerState ps)
    {
        if (_playersInside.Contains(ps))
            return;
        _playersInside.Add(ps);
    }

    [Server]
    void OnPlayerExit (PlayerState ps)
    {
        if (!_playersInside.Contains(ps))
            return;
        _playersInside.Remove(ps);
    }
}
