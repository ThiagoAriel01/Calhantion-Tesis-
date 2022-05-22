using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealthComponent : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))] [SerializeField] protected float health = 100f;
    [SerializeField] protected float healthMax = 100f;

    public delegate void TakeDamageDelegate(HealthComponent health, float dmg, SHitInfo info);
    public delegate void HealthChangeDelegate(HealthComponent health,float oldh, float newh);
    public TakeDamageDelegate onTakeDamage;
    public TakeDamageDelegate onDie;
    public HealthChangeDelegate onHealthChange;
    public HealthChangeDelegate onHealthChangeClient;

    protected bool isDead;
    [SyncVar] protected bool _invunerable;

    [SyncVar] protected float _nextAbsorb = 1f;

    public float Health { get { return health; } set { float oldHealth = health; health = Mathf.Clamp(value,0,healthMax); onHealthChange?.Invoke(this, oldHealth,health); } }
    public float HealthMax { get { return healthMax; } }

    public bool invunerable
    {
        get
        {
            return _invunerable;
        }
        set
        {
            if (isClient)
            {
                CmdSetInvunv(value);
            }
            else
            {
                _invunerable = value;
            }
        }
    }

    public float nextAbsorb
    {
        get
        {
            return _nextAbsorb;
        }
        set
        {
            if (isClient)
            {
                CmdSetAbsorb(value);
            }
            else
            {
                _nextAbsorb = value;
            }
        }
    }

    [Command]
    void CmdSetAbsorb (float val)
    {
        _nextAbsorb = val;
    }

    void OnHealthChanged (float oldp, float newp)
    {
        onHealthChange?.Invoke(this, oldp,newp);
        onHealthChangeClient?.Invoke(this, oldp, newp);
    }

    [Command]
    void CmdSetInvunv(bool val)
    {
        _invunerable = val;
    }

    [Command]
    public void CmdHeal (float amount)
    {
        Heal(amount);
    }

    [Server]
    public void Revive()
    {
        Health = healthMax;
        isDead = false;
        _invunerable = true;
        Invoke("EndInvuv",0.1f);
    }

    void EndInvuv()
    {
        _invunerable = false;
    }

    [Server]
    public void Heal (float amount)
    {
        Health += amount;
    }


    [Server]
    public float TakeDamage(float dmg)
    {
        return TakeDamage(dmg, false, new SHitInfo());
    }

    [Server]
    public float TakeDamage(float dmg, SHitInfo info)
    {
        return TakeDamage(dmg, false,info);
    }

    [Server]
    public float TakeDamage(float dmg, bool ignoreInvun)
    {
        return TakeDamage(dmg, ignoreInvun, new SHitInfo());
    }

    [Server]
    public float TakeDamage(float dmg, bool ignoreInvun, SHitInfo info)
    {
        dmg *= _nextAbsorb;
        info.dmg *= _nextAbsorb;
        bool ignore = !ignoreInvun && invunerable;
        if (isDead || ignore)
            return 0;

        Health -= dmg;
        if (Health <= 0)
        {
            Health = 0.0f;
            isDead = true;
            onDie?.Invoke(this, dmg, info);
        }
        else
        {
            onTakeDamage?.Invoke(this, dmg, info);
        }
        _nextAbsorb = 1f;
        return dmg;
    }
}
