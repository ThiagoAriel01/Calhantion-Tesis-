using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class CloneScript : NetworkBehaviour, IHitable
{
    [SerializeField] protected TextMeshPro _nicknametext;
    [SerializeField] protected TextMeshPro _hptext;
    [SerializeField] protected SpriteRenderer _hpbar;
    protected float _defaultHPSize;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _accel;
    [SerializeField] protected float _gravity = 10f;
    [SerializeField] protected float _maxFallSpeed = 30f;
    [SerializeField] protected float _stickForce = 5f;
    [SerializeField] protected float _airControl;
    [SerializeField] protected float _airSpeed;
    [SerializeField] protected float _airAccel;
    [SerializeField] protected float duration = 6f;
    [SerializeField] protected GameObject _smoke;
    protected CharacterController _cc;
    protected float _currentGravity;
    protected Vector3 _velocity;
    protected Vector3 _move;
    protected Vector3 _input;
    protected Animator animador;
    [SyncVar] protected bool isgrounded;
    [SyncVar] protected Vector3 _vel;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _defaultHPSize = _hpbar.transform.localScale.x;
    }

    float IHitable.TakeDamage(SHitInfo hit)
    {
        if (hit.dmg > 0)
        {
            Disapear();
        }
        return hit.dmg;
    }

    private void Update()
    {
        UpdateAnim();
        if (!isServer)
            return;
        InputUpdate();
        Move();
        GravityUpdate();
        ApplyMove();
    }

    public void SetOwner(NetworkIdentity owner)
    {
        RpcRefreshClone(owner);
        Invoke("Disapear", duration);
    }

    void Disapear()
    {
        NetworkServer.Spawn(Instantiate(_smoke, transform.position, transform.rotation).gameObject);
        NetworkServer.UnSpawn(gameObject);
        Destroy(gameObject);
    }

    [ClientRpc]
    void RpcRefreshClone (NetworkIdentity owner)
    {
        Refresh(owner);
    }

    void Refresh (NetworkIdentity owner)
    {
        PlayerState state = owner.GetComponent<PlayerState>();
        if (state == null)
            return;
        _nicknametext.text = state.GetComponent<PlayerNET>().nickName;
        float hl = (state.GetComponent<HealthComponent>().Health);
        float hmax = (state.GetComponent<HealthComponent>().HealthMax);
        _hptext.text = ((int)hl).ToString();
        _hpbar.transform.localScale = new Vector3(_defaultHPSize * hl / hmax, _hpbar.transform.localScale.y, 1f);

        if (GameModeNetworkState.instance.HasTeams())
        {
            TeamDeathmatch.Team tm = GameModeNetworkState.instance.teams[state.teamIndex];
            _nicknametext.color = tm._color;
            _hpbar.color = tm._color;
        }

        ProtoPlayerMP mp = state.GetComponent<ProtoPlayerMP>();
        PlayerScript pscript = state.GetComponent<PlayerScript>();

        _velocity = pscript.velocity;
        _currentGravity = pscript.currentGravity;

        Animator existent = mp.GetComponentInChildren<Animator>();

        animador = Instantiate(Resources.Load<Animator>("Player/" + mp.modelName), transform);

        AnimatorStateInfo curinfo = existent.GetCurrentAnimatorStateInfo(0);
        animador.Play(curinfo.shortNameHash, 0, curinfo.normalizedTime);
        animador.transform.localPosition = Vector3.zero;
        animador.transform.localRotation = Quaternion.identity;
        Destroy(animador.GetComponent<NetworkIdentity>());
    }

    void GravityUpdate()
    {

        if (_cc.isGrounded)
            _currentGravity = -_stickForce;
        else
            _currentGravity -= _gravity * Time.deltaTime;

        if (_currentGravity <= -_maxFallSpeed)
            _currentGravity = -_maxFallSpeed;
    }

    void ApplyMove()
    {
        _cc.Move(_velocity * Time.deltaTime + _currentGravity * Vector3.up * Time.deltaTime);
    }

    void Move()
    {
        if (_cc.isGrounded)
            GroundMove();
        else
        {
            AirMove();
        }
    }

    void GroundMove()
    {
        _velocity = Vector3.Lerp(_velocity, _move * _moveSpeed, Time.deltaTime * _accel);
    }

    void AirMove()
    {
        if (_move.magnitude > 0.4f)
            _velocity = Vector3.Lerp(_velocity, _move * _airSpeed, Time.deltaTime * _airAccel);
    }

    void InputUpdate()
    {
        _input.x = 0;
        _input.y = 1;
        _move = transform.forward * _input.y + transform.right * _input.x;
        _move.y = 0.0f;
        _move.Normalize();
    }

    private void UpdateAnim()
    {
        if (animador == null)
            return;
        if (isServer)
        {
            _vel = _velocity;
            isgrounded = _cc.isGrounded;
        }
        Vector3 relativeVelocity = transform.InverseTransformDirection(_vel);
        relativeVelocity /= _moveSpeed;
        animador.SetFloat("forward", relativeVelocity.z, 0.25f, Time.deltaTime);
        float spdMult = relativeVelocity.magnitude <= 0.01f ? 1f : relativeVelocity.magnitude;
        animador.SetFloat("speedMult", spdMult);
        animador.SetFloat("right", relativeVelocity.x * 5f, 0.25f, Time.deltaTime);
        animador.SetBool("isGrounded", isgrounded);
        Vector2 move;
        move.x = animador.GetFloat("right");
        move.y = animador.GetFloat("forward");
        animador.SetBool("idle", move.magnitude <= 0.75f);
    }
}
