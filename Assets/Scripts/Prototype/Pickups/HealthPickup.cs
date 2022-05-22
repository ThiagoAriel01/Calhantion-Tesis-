using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : PickUpable
{
    [SerializeField] protected float heals;

    override protected void OnPickedUp(PlayerScript plr)
    {
        base.OnPickedUp(plr);
        plr.Health.Heal(heals);
    }

    override protected bool NeedsPickup(PlayerScript plr)
    {
        return plr.Health.Health < plr.Health.HealthMax;
    }
}