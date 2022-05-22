using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected string _id;
    protected float time;

    public delegate void StateD(string s);
    public StateD onRequestChange;

    public string ID
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }

    virtual public void Enter()
    {

    }

    virtual public void Update(float deltaTime)
    {
        time += deltaTime;
    }

    virtual public void Exit()
    {

    }
}
