using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownPickup : PickUpable
{
    [SerializeField] protected float reduces;

    override protected void OnPickedUp(PlayerScript plr)
    {
        curPlr = plr;
        t = 0.0f;
        plr.GetComponent<PlayerSkills>()?.ReduceAllCooldowns(reduces);
    }

    override protected bool NeedsPickup(PlayerScript plr)
    {
        return true;
    }
}
