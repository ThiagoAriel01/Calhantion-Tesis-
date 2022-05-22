using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]

public class GameModeNetworkState : NetworkBehaviour
{

    protected SyncList<TeamDeathmatch.Team> _teams = new SyncList<TeamDeathmatch.Team>();
    public delegate void GMNSTeamD(TeamDeathmatch.Team[] teams);
    public GMNSTeamD onTeamsChanged;
    static protected GameModeNetworkState _instance;
    [SyncVar] protected int _currentSecond;
    [SyncVar] protected int _maxSeconds;

    SyncList<ControlPoint> _controlPoints = new SyncList<ControlPoint>();

    public int currentSecond
    {
        get
        {
            return _currentSecond;
        }
    }

    public int maxSeconds
    {
        get
        {
            return _maxSeconds;
        }
    }

    static public GameModeNetworkState instance
    {
        get
        {
            return _instance;
        }
    }

    public SyncList<ControlPoint> controlPoints
    {
        get
        {
            return _controlPoints;
        }
    }

    public bool HasTeams()
    {
        return teams != null && teams.Count > 0;
    }   

    [Server]
    public void AddTeams (TeamDeathmatch.Team[] teams)
    {
        _teams.AddRange(teams);
    }

    [Server]
    public void UpdateScore (int index, int newScore)
    {
        _teams[index] = new TeamDeathmatch.Team() { _name = _teams[index]._name, _score = newScore, _color = _teams[index]._color };
    }

    [Server]
    public void SetSecond(int newSecond, int maxSecond)
    {
        _currentSecond = newSecond;
        _maxSeconds = maxSecond;
    }

    private void Awake()
    {
        _instance = this;
    }

    [Server]
    public void Init()
    {
        _controlPoints.AddRange(FindObjectsOfType<ControlPoint>());
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    public SyncList<TeamDeathmatch.Team> teams
    {
        get
        {
            return _teams;
        }
    }

    void OnTeamsChanged (TeamDeathmatch.Team[] oldteams, TeamDeathmatch.Team[] newTeams)
    {
        onTeamsChanged?.Invoke(newTeams);
    }

    [Server]
    public void SetTeams(SyncList<TeamDeathmatch.Team> teams)
    {
        _teams = teams;
    }
}
