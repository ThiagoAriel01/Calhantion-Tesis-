using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
{
    static private T _instance;

    virtual protected bool Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return false;
        }
        _instance = this as T;
        return true;
    }


    static public T instance
    {
        get
        {
            return _instance;
        }
    }
}
