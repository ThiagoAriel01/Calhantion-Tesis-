using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ManaPickup : PickUpable
{
    [SerializeField] protected float heals;

    [Server]
    override protected void OnPickedUp(PlayerScript plr)
    {
        base.OnPickedUp(plr);
        curPlr = plr;
        t = 0.0f;
        TargetHealMana(plr.GetComponent<NetworkIdentity>().connectionToClient, plr.gameObject, heals);
    }

    [TargetRpc]
    public void TargetHealMana (NetworkConnection conn,GameObject obj, float amount)
    {
        PlayerScript plr = obj.GetComponent<PlayerScript>();
        plr.Mana += heals;
    }

    override protected bool NeedsPickup(PlayerScript plr)
    {
        return true;
    }
}
