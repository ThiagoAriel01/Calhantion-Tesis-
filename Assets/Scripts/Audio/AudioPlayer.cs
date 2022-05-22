using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AudioPlayer : NetworkBehaviour
{
    [SerializeField] protected AudioCue _cue;
    [SerializeField] protected bool useOwner = false;

    public override void OnStartServer()
    {
        NetworkIdentity o = null;
        if (useOwner)
            o = GetComponentInParent<NetworkIdentity>();
        _cue.PlaySound(transform.position, o);
    }
}