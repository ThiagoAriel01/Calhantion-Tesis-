using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ghost : NetworkBehaviour, IHitable
{

    public delegate void GhostD(Ghost g);
    public GhostD onDestroy;

    [SerializeField] protected string _skillID;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _returnSpeed;
    [SerializeField] protected float _turnSpeed;
    [SyncVar] protected float _health;
    [SerializeField] protected float _healthMax;
    [SerializeField] protected float _healthConsumeRate;
    [SerializeField] protected float _defenseRatio;
    [SerializeField] protected float _range;
    [SerializeField] protected float _atkRange;
    [SerializeField] protected float _atkConsume = 3f;

    [SerializeField] protected float _attackRate;
    [SerializeField] protected float _attackHitPoint;
    [SerializeField] protected Explosion _explosion;
    [SerializeField] protected float _dmg;
    [SerializeField] protected float _scale;
    [SerializeField] protected float _explosionRate;
    [SerializeField] protected float _explosionDuration;
    [SerializeField] protected Vector3 _idleOffset;
    [SerializeField] protected Transform _hitPoint;

    protected float _attackT;
    protected bool _isFree;
    protected bool _attacking;
    protected GameObject _owner;
    protected PlayerState _ownerState;
    protected GameObject _target;
    protected Animator _anim;
    protected StateMachine _sm;

    public float remainingTime
    {
        get
        {
            float t = (_health / _healthConsumeRate);
            return t;
        }
    }

    public GameObject owner
    {
        get
        {
            return _owner;
        }
    }

    public Vector3 idlePosition
    {
        get
        {
            Vector3 v = _owner.transform.TransformPoint(_idleOffset);
            return v;
        }
    }

    public float atkRange
    {
        get => _atkRange;
    }
    public bool attacking
    {
        get => _attacking;
    }

    public GameObject target
    {
        get => _target;
    }

    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        //_sm.AddState(new AttackingGhost(_moveSpeed, _atkRange,_attackRate,_attackHitPoint), "attacking");
    }

    public void Kill()
    {
        if (isServer)
            _health = 0;
        else
            CmdKill();
    }

    [Command]
    void CmdKill()
    {
        _health = 0;
    }

    public void Init(GameObject owner)
    {
        _sm = new StateMachine();
        _sm.AddState(new IdleGhost(this, _returnSpeed), "idle");
        _sm.AddState(new CombatGhost(this, _moveSpeed), "combat");
        _sm.ChangeState("idle");
        _health = _healthMax;
        _owner = owner;
        _ownerState = _owner.GetComponent<PlayerState>();
        //ProyectileFactory.RequestSpawnExplosion(explosion, owner, atPos, transform.forward, dmg, force, scale, explosionRate, explosionDuration);
    }

    float IHitable.TakeDamage(SHitInfo hit)
    {
        if (hit.attacker == _owner)
            return 0;
        float dmg = hit.dmg * _defenseRatio;
        _health -= dmg;
        return dmg;
    }

    private void OnDestroy()
    {
        if (isServer)
        {
            onDestroy?.Invoke(this);
            NetworkServer.UnSpawn(gameObject);
        }
        if (_ownerState!=null)
        _ownerState.currentGhost = null;
    }

    public void Attack()
    {
        if (_attacking)
            return;

        _anim.SetTrigger("attack");
        RpcAttack();
        _attacking = true;
        CancelInvoke();
        Invoke("HitAttack", _attackHitPoint);
        Invoke("EndAttack", _attackRate);
    }

    [ClientRpc]
    void RpcAttack()
    {
        _anim.SetTrigger("attack");
    }

    void HitAttack()
    {
        ProyectileFactory.RequestSpawnExplosion(_explosion, owner, _hitPoint.transform.position, transform.forward, _dmg, _dmg, _scale, _skillID);
    }

    void EndAttack()
    {
        _attacking = false;
        _health -= _atkConsume;
    }
       
    public override void OnStartServer()
    {
        base.OnStartServer();
        _health = _healthMax;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    private void Update()
    {
        if (!isServer)
            return;
        if (_owner == null)
        {
            Destroy(gameObject);
            return;
        }
        _sm.Update(Time.deltaTime);
        Vector3 dir = transform.position - _owner.transform.position;
        if (dir.magnitude > _range)
        {
            float diff = (dir.magnitude - _range) + 0.01f;
            transform.position -= dir.normalized * diff;
        }
        _health -= Time.deltaTime * _healthConsumeRate;
        if (_health <= 0)
        {
            Destroy(gameObject);
            enabled = false;
        }
    }

    public void MoveDelta(Vector3 delta)
    {
        transform.position += delta;
    }

    private void FixedUpdate()
    {
        if (!isServer)
            return;
        PlayerState target = SearchForTarget();
        if (target != null)
        {
            _target = target.gameObject;
        }
        else
        {
            _target = null;
        }
    }

    PlayerState SearchForTarget() 
    {
        float dist = _range;
        PlayerState closest = null;
        foreach (PlayerState plr in PlayerState.allPlayers)
        {
            if (plr.gameObject != _owner.gameObject && GameModeManager.instance.currentGameMode.CanDamagePlayer(_ownerState,plr))
            {
                float curDist = (plr.transform.position - _owner.transform.position).magnitude;
                if (curDist < dist)
                {
                    dist = curDist;
                    closest = plr;
                }
            }
        }
        return closest;
    }
}
