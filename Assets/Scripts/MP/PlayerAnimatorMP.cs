using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAnimatorMP : NetworkBehaviour
{
    protected PlayerAnimator _target;

    public void Initialize()
    {
        _target = GetComponentInChildren<PlayerAnimator>(true);
        if (_target!=null && isLocalPlayer)
            _target.onPlayAnimation = OnPlayAnimation;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

    void OnPlayAnimation (string p_trigger)
    {
        if (isLocalPlayer)
            Cmd_PlayAnimation(p_trigger);
    }

    [Command]
    protected void Cmd_PlayAnimation(string p_trigger)
    {
        Rpc_PlayAnimation(p_trigger);
    }

    [ClientRpc] 
    protected void Rpc_PlayAnimation (string p_trigger)
    {
        if (!isLocalPlayer)
        {
            if (_target == null)
                _target = GetComponentInChildren<PlayerAnimator>(true);
            _target.PlayAnimation(p_trigger);
        }
    }
}
