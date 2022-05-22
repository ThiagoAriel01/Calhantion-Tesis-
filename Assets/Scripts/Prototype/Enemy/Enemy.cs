using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : Character
{
    [SerializeField] protected float sightRadius=35f;
    [SerializeField] protected float walkSpeed;
    [SerializeField] protected float jogSpeed;
    [SerializeField] protected float runSpeed;
    [SerializeField] protected Squad squadData;
    [SerializeField] protected float squadUpdateRate = 3f;
    [SerializeField] protected bool usesLineOfSight;
    [SerializeField] protected AIWaypoints currentWaypoints;
    [SerializeField] protected LayerMask layers;
    protected CharacterController _controller;
    protected Waypoint waypoint;
    protected EnemyAnimator _enemyAnimador;
    protected EnemyStateMachine _machine;
    protected bool _reachedWaypoint;
    protected bool _goingToDestination;
    protected float _waypointTimer;
    protected bool targetInLineOfSight;
    public bool inSquad;
    protected Vector3 delta;
    protected ProyectileAttack currentProyectile;
    protected Vector3 destination;
    protected Quaternion deltaRot;
    protected Entity target;

    protected NavMeshAgent _agent;
    protected SightComponent sight;

    protected WalkType curWalkType;

    public enum WalkType { Walk, Jog, Run }

    public NavMeshAgent agent
    {
        get
        {
            return _agent;
        }
    }

    virtual public float speed
    {
        get
        {
            return _agent.speed;
        }
        set
        {
            _agent.speed = value;
        }
    }

    public bool reachedWaypoint
    {
        get
        {
            return _reachedWaypoint;
        }
    }

    public float waypointTimer
    {
        get
        {
            return _waypointTimer;
        }
    }
    public EnemyAnimator enemyAnimador
    {
        get
        {
            return _enemyAnimador;
        }
    }

    public CharacterController controller
    {
        get { return _controller; }
    }

    public SightComponent Sight
    {
        get
        {
            return sight;
        }
    }

    public Squad currentSquad
    {
        get
        {
            return squadData;
        }
        set
        {
            squadData = value;
        }
    }


    override protected void Awake()
    {
        base.Awake();
        _uAnimator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        if (squadData != null)
        {
            squadData = Squad.InstantiateSquad(this,squadData);
            InvokeRepeating("UpdateSquad", squadUpdateRate + Random.Range(-1f,3f), squadUpdateRate);
        }
        InvokeRepeating("UpdateSight",squadUpdateRate,squadUpdateRate);
        sight = SightComponent.Create(this, sightRadius);
        sight.onTargetAcquired += OnTargetAcquired;
        _controller = GetComponent<CharacterController>();
        _enemyAnimador = GetComponentInChildren<EnemyAnimator>();
        _machine = new EnemyStateMachine(this);

    }

    virtual protected void UpdateSight()
    {

        if (!usesLineOfSight)
        {
            targetInLineOfSight = true;
            return;
        }

        if (target == null || inSquad)
            return;

        Transform p = GetPoint("sight");
        Vector3 dir = (target.transform.position + Vector3.up) - p.transform.position;
        targetInLineOfSight = false;
        RaycastHit hit;
        if (Physics.Raycast(p.transform.position, dir, out hit, sightRadius, layers))
        {
            if (hit.collider.gameObject == target.gameObject)
                targetInLineOfSight = true;
        }
    }

    virtual protected void HandleCooldown(ref float cooldown)
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
            cooldown = 0;
    }

    protected override void OnTakeDamage(Character character, GameObject target, SHitInfo hit)
    {
        base.OnTakeDamage(character, target, hit);
    }

    virtual protected void UpdateSquad()
    {
        squadData?.OnUpdate(this,squadUpdateRate);
    }

    virtual protected void OnTargetAcquired (SightComponent s,Entity ptarget)
    {
        if (ptarget==null)
            return;

        //AlertAllies(ptarget);
        SetTarget(ptarget);
    }

    virtual public void PerformProyectileAttack (ProyectileAttack pa, Transform target)
    {
        if (pa == null || target == null)
            return;
        _goingToDestination = false;
        PlayAnimation(pa.trigger,pa.fireRate);
        currentProyectile = pa;
        try
        {
            StopCoroutine("ProyectileSequence");
            StartCoroutine(ProyectileSequence(pa, target));
        }
        catch
        {

        }
    }

    IEnumerator ProyectileSequence(ProyectileAttack pa, Transform target)
    {
        float t = 0;
        Vector3 dir = target.transform.position - transform.position;
        float d = 0;
        transform.rotation = Quaternion.LookRotation(dir);
        Transform p = GetPoint("proyectilePoint");
        _enemyAnimador.animador.SetBool("throwing", true);
        WalkType wt = curWalkType;
        speed = 0;
        Entity e = target.GetComponent<Entity>();
        while (t < 1)
        {
            Vector3 v = e.Velocity * 0.75f;
            d = (target.transform.position - p.transform.position).magnitude;
            if (v.y < 0)
                v.y = 0;
            if (e == null)
                dir = target.transform.position - p.transform.position;
            else
                dir = (target.transform.position + v * (d/pa.proyectileSpeed)) - p.transform.position;
            speed = 0;
            t += Time.deltaTime / pa.delay;
            //transform.rotation = Quaternion.LookRotation(dir);
            yield return null;
        }
        Vector3 spreadedDir = dir.normalized;
        spreadedDir.x += Random.Range(-pa.spread, pa.spread);
        spreadedDir.y += Random.Range(-pa.spread, pa.spread);
        spreadedDir.z += Random.Range(-pa.spread, pa.spread);
        Proyectile pro = Instantiate(currentProyectile.proyectile, p.transform.position, Quaternion.LookRotation(spreadedDir));
        Explosion ex = pro.GetComponent<Explosion>();
        if (ex != null)
        {
            ex.Init(pa.damage, pa.damage, pa.explosionScale, gameObject, 0, 10f);
        }
        pro.Init(gameObject, spreadedDir, pa.proyectileSpeed, 10f, pa.damage, 1f, pa.explosionScale, pa.explosion);
        _enemyAnimador.animador.SetBool("throwing", false);
        yield return new WaitForSeconds(pa.fireRate);
        SetWalkType(wt);
        currentProyectile = null;
    }

    virtual public void SquadFollowUpdate(float t)
    {
        if (currentAttack != null || currentProyectile != null)
            return;
        Vector3 sampled = squadData.SampleFormationPosition(this);
        if ((sampled - destination).magnitude > stoppingDistance * 2 + 1f)
        {
            SetDestination(sampled);
        }
    }

    virtual protected void AlertAllies(Entity ptarget)
    {
        if (ptarget == null || target!=null)
            return;

        List<Entity> es = sight.GetAllEntitiesInside();
        foreach (Entity e in es)
        {
            Enemy enemy = e as Enemy;
            if (IsFriendly(e) && enemy != null && enemy.target==null)
            {
                enemy.SetTarget(ptarget);
                enemy.AlertAllies(ptarget);
            }
        }
    }

    virtual public void StopWaypoints()
    {
        currentWaypoints = null;
        waypoint = null;
        _reachedWaypoint = false;
    }

    virtual protected void SetTarget(Entity e)
    {
        target = e;
        _machine.ChangeState("combat");
    }

    virtual protected void Start()
    {
        _machine.ChangeState("idle");
        if (currentWaypoints != null)
        {
            TraverseWaypoints(currentWaypoints);
        }
    }

    virtual public void CombatUpdate (float delta)
    {
        if (!inSquad && (target.transform.position - destination).magnitude > stoppingDistance*2 + 1f)
        {
            SetDestination(target.transform.position);
        }
    }

    override protected IEnumerator AnimationSequence(string trigger, float duration)
    {
        _uAnimator.SetTrigger(trigger);
        inAnimation = true;
        WalkType dt = curWalkType;
        speed = 0;
        yield return new WaitForSeconds(duration);
        SetWalkType(dt);
        inAnimation = false;
    }

    virtual public void AddDelta(Vector3 move, Quaternion rot, Vector3 vel)
    {
        delta += move;
        deltaRot *= rot;
    }

    virtual public void TraverseWaypoints (AIWaypoints waypoints)
    {
        waypoint = waypoints.TraverseWaypoints(this);
        currentWaypoints = waypoints;
        _machine.ChangeState("waypoint");
    }

    protected override void Update()
    {
        base.Update();
        CooldownsUpdate();
        _machine.OnUpdate(Time.deltaTime);
        if (inSquad)
            SquadFollowUpdate(Time.deltaTime);
        AnimatorUpdate();
        if (_goingToDestination)
            MoveUpdate(destination);
        ProcessMove();
    }

    virtual protected void CooldownsUpdate()
    {

    }

    virtual public void OnSquadMerged(Squad merged)
    {
        squadData = merged;
        if (merged.Leader != this)
        {
            inSquad = true;
            if (WalkType.Jog > curWalkType)
            {
                SetWalkType(WalkType.Jog);
            }
        }
        else
        {
            inSquad = false;
        }
    }

    virtual protected void ProcessMove()
    {
        _controller.Move(delta + _agent.velocity * Time.deltaTime + Vector3.up * -20 * Time.deltaTime);
        //transform.rotation *= deltaRot;
        delta = Vector3.zero;
        deltaRot = Quaternion.identity;
    }

    virtual public void AnimatorUpdate()
    {
        if (speed>0)
            _enemyAnimador.animador.SetFloat("forward", (_agent.desiredVelocity.magnitude / speed) * ((int)curWalkType +1),0.1f,Time.deltaTime);
    }

    virtual public void SetWalkType (WalkType walkType)
    {
        curWalkType = walkType;
        switch (curWalkType)
        {
            case WalkType.Walk:
                speed = walkSpeed;
                break;
            case WalkType.Jog:
                speed = jogSpeed;
                break;
            case WalkType.Run:
                speed = runSpeed;
                break;
            default:
                break;
        }
    }

    virtual public float stoppingDistance
    {
        get
        {
            return _agent.stoppingDistance;
        }
    }

    virtual public void WaypointUpdate()
    {
        if (_reachedWaypoint)
            _waypointTimer += Time.deltaTime;
        currentWaypoints?.OnUpdate(this,Time.deltaTime);
        waypoint?.OnUpdate(this, Time.deltaTime);
    }

    virtual public void SetWaypoint (Waypoint way)
    {
        waypoint = way;
        way.OnStart(this);
    }

    virtual public void SetDestination(Vector3 pos)
    {
        if (_agent!=null)
                _agent.SetDestination(pos);
        _goingToDestination = true;
        destination = pos;
    }

    virtual public void OnReachWaypoint (Waypoint w)
    {
        _reachedWaypoint = true;
        _waypointTimer = 0;
    }

    virtual public void SetDestination(Waypoint w)
    {
        _reachedWaypoint = false;
        _waypointTimer = 0;
        SetDestination(w.transform.position);
    }

    virtual public void MoveUpdate(Vector3 target)
    {

    }

}
