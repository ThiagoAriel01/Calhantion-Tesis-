using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeFSM
{
    protected GameModeState _currentState;
    protected GameModeState[] _states;

    public GameModeFSM (GameModeState[] p_states, GameModeState p_starting)
    {
        _states = p_states;
    }

    public GameModeState currentState
    {
        get
        {
            return _currentState;
        }
    }

    public bool ChangeState(string id)
    {
        GameModeState state = FindState(id);
        if (state == null)
            return false;
        return ChangeState(state);
    }

    public GameModeState FindState (string id)
    {
        foreach (GameModeState state in _states)
        {
            if (state.ID == id)
                return state;
        }
        return null;
    }

    public bool ChangeState(GameModeState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit(this);
        }
        _currentState = newState;
        _currentState.Enter(this);
        return true;
    }

    public void UpdateFSM (float deltaTime)
    {
        _currentState.UpdateState(this,deltaTime);
    }

}
