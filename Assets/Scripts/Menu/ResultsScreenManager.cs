using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsScreenManager : MonoSingleton<ResultsScreenManager>
{
    protected GameModeManager.GameEndedMsg _msg;
    protected bool _received;

    public GameModeManager.GameEndedMsg msg
    {
        get
        {
            return _msg;
        }
    }

    public bool received
    {
        get
        {
            return _received;
        }
        set
        {
            _received = value;
        }
    }

    override protected bool Awake()
    {
        if (!base.Awake())
            return false;
        GameModeManager.instance.onClientGameEnded += (x) => { _msg = x; _received = true; };
        return true;
    }
}