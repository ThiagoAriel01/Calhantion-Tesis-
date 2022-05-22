using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TimedObjectDestructor : NetworkBehaviour
{
    [SerializeField] protected float _duration = 6f;

    public override void OnStartServer()
    {
        Invoke("DestroyNow",_duration);
    }

    [Server]
    void DestroyNow()
    {
        NetworkServer.UnSpawn(gameObject);
        Destroy(gameObject);
    }

}
