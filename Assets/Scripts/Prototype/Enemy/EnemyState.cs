using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected string id;

    public EnemyState(string p_id)
    {
        id = p_id;
    }

    public string ID
    {
        get
        {
            return id;
        }
    }

    virtual public bool OnEnter(Enemy e)
    {
        return true;
    }
    virtual public bool OnUpdate(Enemy e,float delta)
    {
        return true;
    }
    virtual public bool OnExit(Enemy e)
    {
        return true;
    }

}
