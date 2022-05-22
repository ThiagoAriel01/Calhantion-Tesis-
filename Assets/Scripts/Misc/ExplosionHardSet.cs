using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ExplosionHardSet : NetworkBehaviour
{
    [SerializeField] protected float _damage;
    [SerializeField] protected float _duration;
    [SerializeField] protected float range;
    [SerializeField] protected float radius;
    [SerializeField] protected float durationExplosion;
    [SerializeField] protected float rate = 0.0f;
    [SerializeField] protected string _skill;

    [Server]
    public void Init(GameObject owner)
    {
        Explosion e = GetComponent<Explosion>();
        if (e == null)
            return;
        e.Init(_damage, _damage, radius / 2, owner, rate, _duration, _skill);
        e.enabled = true;
    }
}
