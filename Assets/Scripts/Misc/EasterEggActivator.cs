using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EasterEggActivator : NetworkBehaviour
{
    [SerializeField] protected float _chance;
    [SerializeField] protected GameObject _objActivate;
    [SerializeField] protected GameObject _objDeactivate;
    [SyncVar(hook = nameof(OnActivateChanged))]protected bool _actived;

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (Random.Range(0f, 100f) <= _chance)
            _actived = true;
    }

    void OnActivateChanged (bool oldv, bool newv)
    {
        _objActivate.SetActive(newv);
        _objDeactivate.SetActive(!newv);
    }
}
