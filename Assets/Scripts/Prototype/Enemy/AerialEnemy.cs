using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialEnemy : LightEnemy
{

    [SerializeField] protected float _turnSpeed = 200f;
    [SerializeField] protected float _acceleration = 8f;
    [SerializeField] protected float _stoppingDistance = 1f;
    protected float minHeight;
    protected float _curSpd;
    protected Vector3 _velocity;

    protected override void Awake()
    {
        base.Awake();
        minHeight = transform.position.y-_controller.height;
    }

    override public float stoppingDistance
    {
        get
        {
            return _stoppingDistance;
        }
    }
    public override float speed { get => _curSpd; set => _curSpd = value; }

    public override void SetDestination(Vector3 pos)
    {
        base.SetDestination(pos);
    }

    public override void MoveUpdate(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.Normalize();
        dir *= speed;
        Vector3 v = transform.position + dir;
        if (v.y <= minHeight)
        {
            dir.y = 0.0f;
        }
        _velocity = Vector3.Lerp(_velocity, dir, Time.deltaTime * _acceleration);
        dir.y = .0f;
        if (dir.magnitude>0.2f)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _turnSpeed * Mathf.Deg2Rad);
    }

    public override void AnimatorUpdate()
    {
       // base.AnimatorUpdate();
    }

    override protected void ProcessMove()
    {
        _controller.Move(delta + _velocity * Time.deltaTime);
        //transform.rotation *= deltaRot;
        delta = Vector3.zero;
        deltaRot = Quaternion.identity;
    }

}
