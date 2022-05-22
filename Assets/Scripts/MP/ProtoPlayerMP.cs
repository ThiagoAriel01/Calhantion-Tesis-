using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ProtoPlayerMP : NetworkBehaviour
{
    protected PlayerScript _plr;
    protected PlayerSkills _skills;
    protected PlayerAnimator _anim;
    protected NetworkAnimator _netAnim;
    protected HealthComponent _health;
    public delegate void PlayerKillNETDelegate(PlayerState p_plr, PlayerState attacker, KillContext context);
    public delegate void PlayerDmgNETDelegate(Character character, GameObject attacker, SHitInfo info);
    public delegate void PlayerNETD(ProtoPlayerMP mp);
    static public PlayerKillNETDelegate onPlayerDied;
    public PlayerDmgNETDelegate onTakeDamage;
    public PlayerNETD onDeath;
    public PlayerNETD onRespawn;
    protected PlayerState _state;
    protected GameObject _lastattacker;
    static protected float _lastAttackerTime = 10;
    protected float _lastT = 0.0f;
    protected bool _killed;
    [SyncVar] protected string _modelName;

    public string modelName
    {
        get
        {
            return _modelName;
        }
    }

    public struct KillContext
    {
        public string skillused;
        public bool killed;
        public bool countDeath;
    }

    public delegate void PlayerBuffD(BuffData b);
    public PlayerBuffD onBuffAdded;
    public PlayerBuffD onBuffRemoved;
    static protected ProtoPlayerMP _local;

    [SyncVar(hook = nameof(OnChangeModel))] protected GameObject _model;
    [SyncVar(hook = nameof(OnChangeSpeed))] protected float _moveSpeed;
    [SyncVar(hook = nameof(OnChangeJump))] protected float _jumpForce;
    [SyncVar(hook = nameof(OnChangeSpeedAir))] protected float _moveSpeedAir;
    [SyncVar(hook = nameof(OnChangeJumpAir))] protected float _jumpForceAir;
    [SyncVar(hook = nameof(OnChangeTotalFreeze))] protected bool _totalinputfreeze;

    public PlayerScript plr
    {
        get
        {
            return _plr;
        }
    }

    static public ProtoPlayerMP local
    {
        get
        {
            return _local;
        }
    }
    public bool totalInputFreeze
    {
        get
        {
            return _totalinputfreeze;
        }
        set
        {
            _totalinputfreeze = value;
        }
    }
    public float moveSpeed
    {
        get
        {
            return _moveSpeed;
        }
        set
        {
            _moveSpeed = value;
        }
    }

    public float jumpForce
    {
        get
        {
            return _jumpForce;
        }
        set
        {
            _jumpForce = value;
        }
    }
    public float moveSpeedAir
    {
        get
        {
            return _moveSpeedAir;
        }
        set
        {
            _moveSpeedAir = value;
        }
    }

    public float jumpForceAir
    {
        get
        {
            return _jumpForceAir;
        }
        set
        {
            _jumpForceAir = value;
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        _local = this;
    }

    [Client]
    public void ClientAddBuff(string buff)
    {
        CmdAddBuff(buff);
    }

    [Client]
    public void ClientRemoveBuff(string buff)
    {
        CmdRemoveBuff(buff);
    }

    [Command]
    void CmdAddBuff(string buff)
    {
        BuffData b = BuffUtility.instance.GetBuffByName(buff);
        if (b != null)
            _plr.AddBuff(b);
    }

    [Command]
    void CmdRemoveBuff(string buff)
    {
        BuffData existent = _plr.GetBuff(buff);
        if (existent == null)
            return;
        _plr.EndBuff(existent);
    }

    [Client]
    public void Suicide()
    {
        CmdSuicide();
    }

    [Command]
    void CmdSuicide()
    {
        _plr.GetComponent<IHitable>().TakeDamage( new SHitInfo()
        {
             dmg=999999f,
             victim = _plr.gameObject,
             skill = "suicide"
        });
    }


    private void Update()
    {
        if (!isServer)
            return;

        if (_lastattacker != null)
        {
            _lastT -= Time.deltaTime;
            if (_lastT <= 0.0f)
                _lastattacker = null;
        }
        _plr.UpdateBuffs();
        if (GameModeManager.instance.currentGameMode.CanRespawn(_state))
        {
            RespawnNow();
        }
    }

    [Server]
    public void RespawnNow()
    {
        _state.alive = true;
        totalInputFreeze = false;
        _plr.Health.Revive();
        if (_anim == null)
            _anim = GetComponentInChildren<PlayerAnimator>();
        Transform t = GameModeManager.instance.currentGameMode.ChooseSpawnLocation(_state);
        TargetRespawn(this.connectionToClient, t.transform.position, t.transform.forward);
        _lastT = 0.0f;
        _lastattacker = null;
    }

    [TargetRpc]
    public void TargetSetVelocity(Vector3 vel)
    {
        _plr.velocity = new Vector3(vel.x,0.0f,vel.z);
        _plr.SetCurGravity(vel.y);
    }

    private void Awake()
    {
        _state = GetComponent<PlayerState>();
        _netAnim = GetComponent<NetworkAnimator>();
        _plr = GetComponent<PlayerScript>();
        _skills = GetComponent<PlayerSkills>();
        //_anim = GetComponentInChildren<PlayerAnimator>();
        _health = GetComponent<HealthComponent>();
        _plr.enabled = false;
        _skills.enabled = false;
        //_anim.enabled = false;
    }

    void OnChangeSpeed(float prevv, float newv)
    {
        _plr.moveSpeed = newv;
    }

    void OnChangeJump(float prevv, float newv)
    {
        _plr.jumpForce = _jumpForce;
    }

    void OnChangeSpeedAir(float prevv, float newv)
    {
        _plr.airSpeed = newv;
    }

    void OnChangeJumpAir(float prevv, float newv)
    {
        _plr.airJumpForce = newv;
    }

    void OnChangeTotalFreeze(bool prevv, bool newv)
    {
        _plr.inputFreeze = newv;
        _skills.silenced = newv;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        //Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        //rb.isKinematic = true;
        //rb.interpolation = RigidbodyInterpolation.Interpolate;
        //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        if (_model!=null)
            ApplyModel(_model);
    }

    public override void OnStartServer()
    {
        _moveSpeed = _plr.moveSpeed;
        _jumpForce = _plr.jumpForce;
        _moveSpeedAir = _plr.airSpeed;
        _jumpForceAir = _plr.airJumpForce;
        _plr.onTakeDamage += OnTakeDamage;
        _plr.onKilled += OnKilled;
        _plr.onRespawn += OnRespawn;
        _plr.onBuffAdded += (x) => OnBuffAdded(connectionToClient, x.name);
        _plr.onBuffRemoved += (x) => OnBuffRemoved(connectionToClient, x.name);
    }

    [TargetRpc]
    void OnBuffAdded(NetworkConnection conn, string b)
    {
        BuffData buff = BuffUtility.instance.GetBuffByName(b);
        if (buff == null)
            return;
        onBuffAdded?.Invoke(buff);
    }

    [TargetRpc]
    void OnBuffRemoved(NetworkConnection conn, string b)
    {
        BuffData buff = BuffUtility.instance.GetBuffByNameInstance(b);
        if (buff == null)
            return;
        onBuffRemoved?.Invoke(buff);
    }

    [Server]
    public void ChangeModel(string newModel)
    {
        Debug.Log(newModel);

        PlayerAnimator prefab = Resources.Load<PlayerAnimator>("Player/" + newModel);
        PlayerAnimator model = Instantiate(prefab, transform.position, transform.rotation);
        NetworkServer.Spawn(model.gameObject);
        model.GetComponent<NetworkIdentity>().AssignClientAuthority(GetComponent<NetworkIdentity>().connectionToClient);
        _model = model.gameObject;
        _modelName = newModel;
        ApplyModel(_model);
        RpcChangeModel(newModel, model.gameObject);
        //RpcChangeModel(newModel);
    }

    public void ShowPlayerModel(bool val)
    {
        CmdShowPlayerModel(val);
    }

    [Command]
    void CmdShowPlayerModel(bool val)
    {
        RpcShowPlayerModel(val);
    }

    [ClientRpc]
    void RpcShowPlayerModel(bool val)
    {
        if (_anim == null)
            _anim = GetComponentInChildren<PlayerAnimator>();
        _anim.ShowMeshes(val);
    }

    void OnChangeModel(GameObject prev, GameObject newp)
    {
        ApplyModel(newp);
    }

    [ClientRpc]
    void RpcChangeModel(string newp,GameObject obj)
    {
        ApplyModel(obj);
    }

    void ApplyModel (GameObject obj)
    {
       // Debug.Log(newp);

        if (obj == null)
            return;
        PlayerAnimator model = obj.GetComponent<PlayerAnimator>();
        model.transform.SetParent(transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        GetComponent<PlayerAnimatorMP>().Initialize();
        if (isLocalPlayer)
        {
            _plr.uAnimator = model.GetComponent<Animator>();
            model.Initiar();
            model.enabled = true;
        }
        //_netAnim = GetComponent<NetworkAnimator>();
        //_netAnim.ResetAnim();
    }

    [Server]
    void OnTakeDamage (Character character, GameObject attacker, SHitInfo info)
    {
        _lastattacker = attacker;
        _lastT = _lastAttackerTime;
        if (attacker != null)
        {
            NetworkConnection conn = attacker.GetComponent<NetworkIdentity>().connectionToClient;
            TargetOnTakeDamage(conn, attacker, info);
        }
        TargetOnTakeDamage(connectionToClient, attacker, info);
    }

    [Server]
    void OnKilled(Character character, GameObject attacker, SHitInfo info)
    {
        /*_killed = true;
        KillContext kcontext = new KillContext()
        {
            skillused = info.skill,
            killed = true
        };
        onPlayerDied?.Invoke(character.GetComponent<PlayerState>(), attacker.GetComponent<PlayerState>(), kcontext);*/
    }

    [TargetRpc]
    void TargetOnTakeDamage(NetworkConnection conn, GameObject attacker,SHitInfo info)
    {
        if (isLocalPlayer)
        {
            if (_anim == null)
                _anim = GetComponentInChildren<PlayerAnimator>();
            _anim.PlayAnimation("hurt");
        }
        if (!isServer)
        {
            Character.onCharacterDamaged?.Invoke(_plr, attacker, info);
        }
        onTakeDamage?.Invoke(_plr, attacker, info);
    }

    [Server]
    void OnRespawn(PlayerScript plr, SHitInfo info)
    {
        PlayerState state = GetComponent<PlayerState>();
        _plr.ClearBuffs();
        state.alive = false;
        totalInputFreeze = true;
        ResetMaterial();

        if (info.attacker == null)
        {
            KillContext k = new KillContext();
            k.killed = _lastattacker != null;
            k.skillused = info.skill;
            k.countDeath = _lastattacker != null;
            onPlayerDied?.Invoke(GetComponent<PlayerState>(), _lastattacker != null ? _lastattacker.GetComponent<PlayerState>() : null, k);
        }
        else
        {
            KillContext kcontext = new KillContext()
            {
                skillused = info.skill,
                killed = true
            };
            PlayerState atk = info.attacker.GetComponent<PlayerState>();
            atk.OnKillConfirm(info.skill);
            onPlayerDied?.Invoke(plr.GetComponent<PlayerState>(), atk, kcontext);
        }
        TargetOnDeath(plr.GetComponent<NetworkIdentity>().connectionToClient);
    }

    [TargetRpc]
    void TargetOnDeath(NetworkConnection conn)
    {
        onDeath?.Invoke(this);
        if (_anim == null)
            _anim = GetComponentInChildren<PlayerAnimator>();
        _anim.PlayAnimation("die");
    }

    [TargetRpc]
    void TargetRespawn (NetworkConnection target,Vector3 newPos, Vector3 newforward)
    {
        _plr.Mana = _plr.ManaMax;
        _plr.velocity = Vector3.zero;
        newforward.y = .0f;
        _plr.transform.rotation = Quaternion.LookRotation(newforward);
        _plr.Teleport(newPos);
                if (_anim == null)
            _anim = GetComponentInChildren<PlayerAnimator>();
        _anim.PlayAnimation("reset");
        GetComponent<PlayerSkills>().ZeroAllCooldowns();
        onRespawn?.Invoke(this);
    }

    [Server]
    public void ChangeMaterial(string mat)
    {
        //ChangeMaterialLocal(mat);
        RpcChangeMaterial(mat);
    }

    [Client]
    public void ClientChangeMaterial (string mat)
    {
        CmdChangeMaterial(mat);
    }

    [Command]
    void CmdChangeMaterial (string mat)
    {
        ChangeMaterial(mat);
    }

    [Server]
    public void ResetMaterial()
    {
        RpcResetMaterial();
    }

    [Client]
    public void ClientResetMaterial()
    {
        CmdResetMaterial();
    }

    [Command]
    void CmdResetMaterial()
    {
        ResetMaterial();
    }

    [ClientRpc]
    void RpcResetMaterial()
    {
        if (_anim == null)
            _anim = GetComponentInChildren<PlayerAnimator>();
        _anim.ResetMaterial();
    }

    [ClientRpc]
    void RpcChangeMaterial(string mat)
    {
        ChangeMaterialLocal(mat);
    }

    void ChangeMaterialLocal (string mat)
    {
        Material mate = Resources.Load<Material>("Materials/" + mat);
        if (mate == null)
            return;
        if (_anim == null)
            _anim = GetComponentInChildren<PlayerAnimator>();
        _anim.ChangeMaterial(mate);
    }

    public override void OnStartAuthority()
    {
        _plr = GetComponent<PlayerScript>();
        _skills = GetComponent<PlayerSkills>();
        PlayerFactory.CreateClientSide(_plr);
        _plr.camara = Camera.main;
        _plr.enabled = true;
        _skills.enabled = true;
    }

    [Server]
    public void SetBool (string key, bool val)
    {
        TargetSetBool(key, val);
    }

    [TargetRpc]
    void TargetSetBool(string key, bool val)
    {
        if (_anim==null)
            _anim = GetComponentInChildren<PlayerAnimator>();
        _anim.animachion.SetBool(key, val);
    }
}