using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TriggerHurt : NetworkBehaviour
{
    [SerializeField] protected string _killer;
    [SerializeField] protected float _damage;

    public override void OnStartClient()
    {
        base.OnStartClient();
        enabled = false;
    }

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        HealthComponent h = other.GetComponent<HealthComponent>();
        if (h == null)
            return;

        SHitInfo info = new SHitInfo()
        {
            skill = _killer
        };

       h.TakeDamage(_damage,true, info);
    }
}