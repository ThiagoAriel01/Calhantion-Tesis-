using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FootstepsSounds : NetworkBehaviour
{
    [SerializeField] protected FootstepTrigger[] _triggers;
    [SerializeField] protected AudioCue _cue;

    public override void OnStartAuthority()
    {
        foreach (FootstepTrigger item in _triggers)
        {
            item.onFootstep += OnFootstep;
        }
    }

    public void OnFootstep()
    {
        _cue.PlaySound(transform.position, netIdentity);
    }
}
