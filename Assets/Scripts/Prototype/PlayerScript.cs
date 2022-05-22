using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerScript))]

public class PlayerScript : Character
{
    public delegate void PlayerDelegate(PlayerScript plr);
    public delegate void PlayerRespawnDelegate(PlayerScript plr, SHitInfo info);
    public PlayerDelegate onJump;
    public PlayerDelegate onJumpAir;
    public PlayerRespawnDelegate onRespawn;

    [SerializeField] protected Transform _aimTransform;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _accel;
    [SerializeField] protected float _gravity = 10f;
    [SerializeField] protected float _turnSpeed;
    [SerializeField] protected float _maxFallSpeed = 30f;
    [SerializeField] protected float _stickForce = 5f;
    [SerializeField] protected float _airControl;
    [SerializeField] protected float _airSpeed;
    [SerializeField] protected float _airAccel;
    [SerializeField] protected float _airturnSpeed;
    [SerializeField] protected float _jumpForce;
    [SerializeField] protected float _airJumpForce;
    [SerializeField] protected int _airJumps = 1;
    [SerializeField] protected float _glideMaxGravity;
    [SerializeField] protected float _casterGlideGravity;
    [SerializeField] protected float _casterGlideFriction;
    [SerializeField] protected bool _cancelGlideOnCast = true;
    protected bool _gliding;
    protected bool _glideCanceled;
    protected bool _casting;
    protected bool _freezed;
    protected int _currentJumps = 0;
    protected CharacterController _controller;
    protected Camera _cam;
    protected Vector3 _delta;
    protected Vector3 _move;
    protected Vector3 _velocity;
    protected Vector3 _input;
    protected Vector3 _relativeInput;
    protected Vector3 _animVelocity;
    protected bool _wishJump;
    protected float _currentGravity = 0.0f;
    protected float _pendingJump;
    protected bool _invunerable;
    protected bool _inputFreze;
    protected bool _freezeRotation;
    static protected PlayerScript _instance;
    protected CollisionFlags _colFlags;
    protected Transform _parent;

    public Vector3 relativeInput
    {
        get => _relativeInput;
    }

    public bool inputFreeze
    {
        get
        {
            return _inputFreze;
        }
        set
        {
            _inputFreze = value;
        }
    }

    public bool freezeRotation
    {
        get
        {
            return _freezeRotation;
        }
        set
        {
            _freezeRotation = value;
        }
    }

    static public PlayerScript instance
    {
        get
        {
            return _instance;
        }
    }

    public Vector3 input
    {
        get => _input;
    }
    public Vector3 velocity
    {
        get => _velocity;
        set => _velocity = value;
    }

    override public bool isGrounded
    {
        get => _controller.isGrounded;
    }

    public bool isGliding
    {
        get => _gliding;
    }

    public float moveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value;
    }

    public float jumpForce
    {
        get => _jumpForce;
        set => _jumpForce = value;
    }

    public float airSpeed
    {
        get => _airSpeed;
        set => _airSpeed = value;
    }

    public float airJumpForce
    {
        get => _airJumpForce;
        set => _airJumpForce = value;
    }

    public float currentGravity
    {
        get => _currentGravity;
        set => _currentGravity = value;
    }

    public Camera camara
    {
        get => _cam;
        set => _cam = value;
    }

    protected override void OnDie(HealthComponent health, float dmg, SHitInfo info)
    {
        onDie?.Invoke(this,info);
        Respawn(info);
    }

    public void ClearBuffs()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            EndBuff(buffs[i]);
        }
    }

    public BuffData GetBuff (string buffname)
    {
        BuffData existent = buffs.Find(x => x.name == buffname + "(Clone)");
        return existent;
    }

    private void Respawn(SHitInfo info)
    {
        onRespawn?.Invoke(this, info);
    }

    private void ResetGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Teleport(Vector3 pos)
    {
        _controller.enabled = false;
        transform.position = pos;
        _controller.enabled = true;
    }

    override public Vector3 Velocity
    {
        get
        {
            return _velocity + currentGravity * Vector3.up;
        }
        set
        {
            _velocity = value;
        }
    }

    override public bool freeze
    {
        get
        {
            return _freezed;
        }
        set
        {
            _freezed = value;
        }
    }


    override public Vector3 charInput
    {
        get
        {
            return _move.magnitude>0.1f ? _move : transform.forward;
        }
    }

    public Transform aimTransform
    {
        get => _aimTransform;
    }

    public void SetCurGravity (float v)
    {
        _currentGravity = v;
        _pendingJump = v;
        _wishJump = true;
    }

    public bool casting
    {
        get
        {
            return _casting;
        }
        set
        {
            if (value)
                OnEnterCast();
            else
                OnExitCast();

            _casting = value;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (_colFlags == CollisionFlags.Below && hit.collider.CompareTag("Platform"))
        {
            _parent = hit.collider.transform.root;
        }
    }

    void OnEnterCast()
    {

    }

    void OnExitCast()
    {

    }

    override protected void Awake()
    {
        base.Awake();
        _controller = GetComponent<CharacterController>();
        _instance = this;
    }

    public void AddDelta (Vector3 p_deltaPos, Quaternion p_deltaRot, Vector3 p_animVelocity)
    {
        transform.rotation *= p_deltaRot;
        _delta += p_deltaPos;
        _animVelocity = p_animVelocity;
    }

    public void AddJump(float p_force)
    {
        if (_inputFreze)
            return;
        _pendingJump = p_force;
        _wishJump = true;
    }

    override protected void Update()
    {
        if (_invunerable)
            health.invunerable = true;

        base.Update();
        InputUpdate();
        Move();
        GravityUpdate();
        ApplyMove();
        Rotate();
        LookUpdate();

        //if (_parent != null && transform.root != _parent)
        //    transform.SetParent(_parent);

        //if (_parent == null && transform.root != null)
        //    transform.SetParent(null);

        _parent = null;

    }

    void LookUpdate()
    {
        Vector3 dir = (_cam.transform.position + _cam.transform.forward * 1000f) - _aimTransform.transform.position;
        _aimTransform.transform.rotation = Quaternion.LookRotation(dir);
    }

    void InputUpdate()
    {
        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");
        _wishJump = Input.GetButtonDown("Jump");
        _gliding = Input.GetButton("Jump") && _currentGravity < 0 && !_controller.isGrounded && !_glideCanceled;

        if ((Input.GetButtonUp("Jump") && _glideCanceled) || !Input.GetButton("Jump"))
            _glideCanceled = false;
        if (_inputFreze)
        {
            _wishJump = false;
            _gliding = false;
            _input.x = 0.0f;
            _input.y = 0.0f;
        }
        _move = _cam.transform.forward * _input.y + _cam.transform.right * _input.x;
        _move.y = 0.0f;
        _move.Normalize();
        _relativeInput = transform.InverseTransformDirection(_move);
    }

    public void CancelGlide()
    {
        if (_cancelGlideOnCast)
            _glideCanceled = true;
    }

    void Move()
    {
        if (_freezed)
            return;

        if (_controller.isGrounded)
            GroundMove();
        else
        {
            if (!_casting)
                AirMove();
            else
                CastingMove();
        }
    }

    void CastingMove()
    {
        velocity = Vector3.Lerp(_velocity, Vector3.zero, Time.deltaTime * _casterGlideFriction);
    }

    void GroundMove()
    {
        _currentJumps = 0;
        if (_wishJump)
        {
            _pendingJump = _jumpForce;
            onJump?.Invoke(this);
        }
        _velocity = Vector3.Lerp(_velocity, _move * _moveSpeed, Time.deltaTime * _accel);
    }

    void AirMove()
    {
        if (_wishJump && _currentJumps < _airJumps)
        {
            _pendingJump = _airJumpForce;
            _currentJumps++;
            onJumpAir?.Invoke(this);
        }
        if (_move.magnitude>0.4f)
            _velocity = Vector3.Lerp(_velocity, _move * _airSpeed, Time.deltaTime * _airAccel);
    }

    void GravityUpdate()
    {
        if (_freezed)
        {
            _currentGravity = 0;
            return;
        }

        if (_pendingJump>0)
        {
            _currentGravity = _pendingJump;
            _pendingJump = 0.0f;
            return;
        }

        if (_controller.isGrounded)
            _currentGravity = -_stickForce;
        else
            _currentGravity -= _gravity * Time.deltaTime;

        float maxFall = !_casting && _currentGravity < 0 ? (_gliding ? _glideMaxGravity : _maxFallSpeed) : (!_controller.isGrounded ? _casterGlideGravity : _stickForce);
        if (_currentGravity <= -maxFall)
            _currentGravity = -maxFall;
    }


    void ApplyMove()
    {
        _colFlags = _controller.Move(_delta + _velocity * Time.deltaTime + _currentGravity * Vector3.up * Time.deltaTime);
        _delta = Vector3.zero;
    }

    void Rotate()
    {
        if (_freezeRotation)
            return;

        float finalTurnSpeed = _controller.isGrounded ? _turnSpeed : _airturnSpeed;
        Vector3 finalVel = _velocity;
        finalVel.y = .0f;
        if (finalVel.magnitude>0.1f)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(finalVel.normalized), Time.deltaTime * finalTurnSpeed);
    }
}