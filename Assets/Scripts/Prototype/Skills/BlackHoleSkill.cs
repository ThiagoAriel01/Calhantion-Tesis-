using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/BlackHole Skill")]
public class BlackHoleSkill : SkillData
{
    [SerializeField] protected float _duration;
    [SerializeField] protected float range;
    [SerializeField] protected float radius;
    [SerializeField] protected float durationExplosion;
    [SerializeField] protected Explosion explosion;
    [SerializeField] protected float rate = 0.0f;


    protected bool _phasing;
    protected float _t = 0.0f;

    public override bool Cast(Character character, GameObject target, Vector3 direction)
    {
        if (!base.Cast(character, target, direction))
            return false;

        PlayerScript plr = character.GetComponentInChildren<PlayerScript>();
        plr.freeze = true;
        plr.velocity = Vector3.zero;
        character.GetComponentInChildren<ProtoPlayerMP>().ShowPlayerModel(false);
        _t = 0.0f;
        _phasing = true;
        character.invunerable = true;
        ProyectileFactory.RequestSpawnExplosion(explosion, character.gameObject, character.transform.position, Vector3.zero, damage, damage, radius / 2, rate, durationExplosion, this.name);

        return true;
    }

    void EndCast(Character character)
    {
        PlayerScript plr = character.GetComponentInChildren<PlayerScript>();
        character.GetComponentInChildren<ProtoPlayerMP>().ShowPlayerModel(true);
        plr.freeze = false;
        character.invunerable = false;
        plr.velocity = Vector3.zero;
        _phasing = false;
        _t = 0.0f;
    }

    public override bool ForceCancel(Character character)
    {
        if (_phasing)
            EndCast(character);
        return true;
    }

    public override bool SkillUpdate(Character character, float deltaTime)
    {
        base.SkillUpdate(character, deltaTime);
        if (_phasing)
        {
            _t += deltaTime;
        }
        if (_t >= _duration)
        {
            EndCast(character);
        }
        return true;
    }
}