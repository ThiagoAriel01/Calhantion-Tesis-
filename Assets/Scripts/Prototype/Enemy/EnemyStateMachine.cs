using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{

    protected List<EnemyState> states = new List<EnemyState>();

    protected EnemyState currentState;

    protected Enemy _enemy;

    public EnemyStateMachine(Enemy penemy)
    {
        _enemy = penemy;
        states.Add(new EnemyState("idle"));
        states.Add(new FollowingWaypointState("waypoint"));
        states.Add(new CombatState("combat"));
        //states.Add(new SquadFollowing("squadFollow"));
    }

    public void OnUpdate (float delta)
    {
        currentState?.OnUpdate(_enemy,delta);
    }

    public bool ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return false;
        currentState?.OnExit(_enemy);
        currentState = newState;
        currentState?.OnEnter(_enemy);
        return true;
    }
    public bool ChangeState(string newState)
    {
        EnemyState st = GetStateByName(newState);
        return ChangeState(st);
    }

    EnemyState GetStateByName (string str)
    {
        foreach (EnemyState state in states)
        {
            if (state.ID == str)
                return state;
        }
        return null;
    }

}
