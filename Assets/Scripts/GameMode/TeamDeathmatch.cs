using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "Data/GameModes/Team Deathmatch")]
public abstract class TeamDeathmatch : GameMode
{
    [SerializeField] protected bool _quadTeam = false;
    [SerializeField] protected int _timeLimit = 600;
    [SerializeField] protected int _maxTeamScore = 2500;
    protected int _currentSecond=0;
    protected float _currentT;
    protected bool ended;
    protected Team[] _teams;

    [System.Serializable]
    public class Team
    {
        [SerializeField] public string _name;
        [SerializeField] public Color _color;
        [SerializeField] public int _score;
    }

    [Server]
    public void AddTeamScore(int teamIndex, int amount)
    {
        _teams[teamIndex]._score += amount;
        _instancedNetState.UpdateScore(teamIndex, _teams[teamIndex]._score);
        if (_teams[teamIndex]._score >= _maxTeamScore && !ended)
            FinishGame();
    }

    [Server]
    public void AddTeamScore(string teamName, int amount)
    {
        int teamIndex = GetIndexByName(teamName);
        if (teamIndex < 0)
            return;
        AddTeamScore(teamIndex, amount);
    }

    public override PlayerState[] DetermineWinners()
    {
        int winnerTeamIndex = GetWinnerTeam();
        List<PlayerState> winners = new List<PlayerState>();
        if (winnerTeamIndex < 0)
            return null;
        foreach (var item in PlayerState.allPlayers)
        {
            if (item.teamIndex == winnerTeamIndex)
            {
                winners.Add(item);
            }
        }
        if (winners.Count>0)
            return winners.ToArray();
        return new PlayerState[0];
    }

    protected virtual int GetWinnerTeam()
    {
        int minScore = 0;
        int minTeam = -1;
        for (int i = 0; i < _teams.Length; i++)
        {
            if (_teams[i]._score > minScore)
            {
                minScore = _teams[i]._score;
                minTeam = i;
            }
        }
        return minTeam;
    }

    int GetIndexByName(string teamname)
    {
        for (int i = 0; i < _teams.Length; i++)
        {
            if (_teams[i]._name == teamname)
                return i;
        }
        return -1;
    }

    [Server]
    public Team[] GetTeams()
    {
        return _teams;
    }

    public int GetPlayersInTeam(int teamindex)
    {
        int count = 0;
        foreach (var item in PlayerState.allPlayers)
        {
            if (item.teamIndex == teamindex)
                count++;
        }
        return count;
    }

    public override void UpdateGameMode(float deltaTime)
    {
        base.UpdateGameMode(deltaTime);
        _currentT += deltaTime;
        _currentSecond = (int)_currentT;
        _instancedNetState.SetSecond(_currentSecond,_timeLimit);
        if (_currentSecond >= _timeLimit && !ended)
        {
            FinishGame();
        }
    }

    public override void FinishGame()
    {
        base.FinishGame();
        ended = true;
    }

    public override void StartGameMode()
    {
        base.StartGameMode();
        if (!_quadTeam)
        {
            _teams = new Team[2];
            _teams[0] = new Team
            {
                _name = "RED",
                _color = Color.red,
                _score = 0
            };
            _teams[1] = new Team
            {
                _name = "BLUE",
                _color = Color.blue,
                _score = 0
            };
        }
        else
        {
            _teams = new Team[4];
            _teams[0] = new Team
            {
                _name = "RED",
                _color = Color.red,
                _score = 0
            };
            _teams[1] = new Team
            {
                _name = "BLUE",
                _color = Color.blue,
                _score = 0
            };
            _teams[2] = new Team
            {
                _name = "YELLOW",
                _color = Color.yellow,
                _score = 0
            };
            _teams[3] = new Team
            {
                _name = "GREEN",
                _color = Color.green,
                _score = 0
            };
        }
    }

    public override GameModeNetworkState SpawnNetworkState()
    {
        GameModeNetworkState netstate = base.SpawnNetworkState();
        netstate.AddTeams(_teams);
        return netstate;
    }
}
