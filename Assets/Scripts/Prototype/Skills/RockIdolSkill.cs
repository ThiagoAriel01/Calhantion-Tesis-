using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Rock Idol Skill ")]
public class RockIdolSkill : SkillData
{
    [SerializeField] protected Explosion explosion;
    [SerializeField] protected float explosionScale;
    [SerializeField] protected float gravity = 35;
    [SerializeField] protected float jumpForce = 5;
    [SerializeField] protected float duration = 5;
    [SerializeField] protected Material mat;
    protected bool transformed = false;
    protected float t = 0;

    public override bool Cast(Character character, GameObject target, Vector3 dir)
    {
        if (!base.Cast(character, target, dir))
            return false;
        character.uAnimator.SetTrigger("transform");
        character.freeze = true;
        character.invunerable = true;
        float y = character.Velocity.y;
        if (y > 0)
            y = 0;
        character.Velocity = new Vector3(0, y, 0);
        t = 0;
        transformed = true;
        character.GetComponent<ProtoPlayerMP>().ClientChangeMaterial(mat.name);
        return true;
    }

    public override bool SkillUpdate(Character character, float deltaTime)
    {
        if (!base.SkillUpdate(character, deltaTime))
            return false;
        if (transformed)
        {
            HandleTransformation(character, deltaTime);
            t += Time.deltaTime;
            if (t >= duration)
            {
                transformed = false;
                character.freeze = false;
                character.invunerable = false;
            }
            if (t>0.1f && character.isGrounded)
            {
                HitGround(character, Mathf.Abs(character.Velocity.y));
                EndIdol(character);
            }
        }
        return true;
    }

    void EndIdol(Character character)
    {
        character.GetComponent<ProtoPlayerMP>().ClientResetMaterial();
        transformed = false;
        character.invunerable = false;
        character.freeze = false;
    }

    public override bool ForceCancel(Character character)
    {
        if (!transformed)
            return false;
        EndIdol(character);
        return true;
    }

    public virtual void HandleTransformation (Character character, float deltaTime)
    {
        character.Velocity += Vector3.up * -gravity * deltaTime;
    }

    protected virtual void HitGround(Character character, float slamGravity)
    {
        ProyectileFactory.RequestSpawnExplosion(explosion, character.gameObject, character.transform.position, character.transform.forward, damage * (slamGravity / gravity), 
            Mathf.Clamp(damage * (slamGravity / gravity),15f,damage), explosionScale * (slamGravity / gravity), 0f, 10f,this.name);
        //Explosion explo = Instantiate(explosion, character.transform.position,character.transform.rotation);
        //explo.Init(damage * (slamGravity / gravity), damage * (slamGravity / gravity), explosionScale * (slamGravity / gravity), character.gameObject, 0, 10);
    }
}
