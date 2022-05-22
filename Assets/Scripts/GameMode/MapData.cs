using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Map")]
public class MapData : ScriptableObject
{
    [SerializeField] protected string _levelName;
    [SerializeField] protected string _displayName;
    [SerializeField] protected GameMode[] _compatibleGamemodes;

    public string levelName
    {
        get
        {
            return _levelName;
        }
    }
    public string displayName
    {
        get
        {
            return _displayName;
        }
    }

    public bool IsCompatibleWith(GameMode mode)
    {
        for (int i = 0; i < _compatibleGamemodes.Length; i++)
        {
            if (_compatibleGamemodes[i] == mode)
                return true;
        }
        return false;
    }

}
