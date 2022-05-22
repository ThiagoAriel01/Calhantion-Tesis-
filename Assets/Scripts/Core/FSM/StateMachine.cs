using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    protected State _currentState;
    protected List<State> _states = new List<State>();

    public State GetState (string id)
    {
        foreach (State state in _states)
        {
            if (state.ID == id)
                return state;
        }
        return null;
    }

    virtual public void AddState (State state, string asid)
    {
        State existent = _states.Find(x => x.ID == asid);
        if (existent == null)
        {
            state.ID = asid;
            _states.Add(state);
        }
    }

    public void ChangeState(string stateID)
    {
        State s = GetState(stateID);
        if (s!=null)
            ChangeState(s);
    }

    virtual public void ChangeState (State newState)
    {
        if (_currentState != null)
        {
            _currentState.onRequestChange -= OnRequestChange;
            _currentState.Exit();
        }
        _currentState = newState;
        _currentState.onRequestChange += OnRequestChange;
        _currentState.Enter();
    }

    virtual public void OnRequestChange(string newState)
    {
        ChangeState(newState);
    }

    virtual public void Update(float deltaTime)
    {
        _currentState?.Update(deltaTime);
    }
}