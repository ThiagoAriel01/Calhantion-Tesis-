using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SHitInfo
{
    public float dmg;
    [System.NonSerialized] public BuffData buff;
    public int knockback;
    public float force;
    public Vector3 direction;
    public Vector3 point;
    public string skill;
    public GameObject attacker;
    public GameObject victim;
}
public interface IHitable
{
    float TakeDamage(SHitInfo hit);
}
