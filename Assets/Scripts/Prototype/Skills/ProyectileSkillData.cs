using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Proyectile Skill", menuName = "Data/Proyectile Skill", order = 0)]
public class ProyectileSkillData : SkillData
{
    [SerializeField] protected float range;
    [SerializeField] protected float duration;
    [SerializeField] protected float speed;
    [SerializeField] protected int shots=1;
    [SerializeField] protected float spread =0f;
    [SerializeField] protected string castPoint;
    [SerializeField] protected Proyectile proyectile;
    [SerializeField] protected Explosion explosion;
    [SerializeField] protected float explosionForceMult;
    [SerializeField] protected float explosionScale;
    [SerializeField] protected Vector2[] shotPattern;
    [SerializeField] protected float explosionRate=-1;

    public override bool PrepareToCast(Character character)
    {
        if (base.PrepareToCast(character))
        {
            Transform point = character.GetPoint(castPoint);
            Vector3 dir = point.forward;
            PreviewManager.instance.InitPreview(this, dir, proyectile.GetComponent<Rigidbody>().useGravity ? -Physics.gravity.y : 0f, explosionScale*2f, speed<=0 ? range / duration : speed
                ,range,proyectile.GetComponent<CapsuleCollider>().radius);
            return true;
        }
        return false;
    }

    protected virtual float GetDamage()
    {
        return damage;
    }

    public override void CancelCast(Character character)
    {
        PreviewManager.instance.EndPreview();
    }

    public override bool Cast(Character character, GameObject target, Vector3 direction)
    {
        PreviewManager.instance.EndPreview();
        if (!CanCast(character))
            return false;
        base.Cast(character, target, direction);

        Transform point = character.GetPoint(castPoint);
        Vector3 dir =  point.transform.forward;
        if (direction != Vector3.zero)
            dir = direction;
        for (int i = 0; i < shots; i++)
        {
            Vector3 spreadedDir = dir.normalized;
            if (shotPattern != null && shotPattern.Length > 0)
            {
                spreadedDir += point.right * shotPattern[i].x + point.up * shotPattern[i].y;
            }
            spreadedDir.x += Random.Range(-spread, spread);
            spreadedDir.y += Random.Range(-spread, spread);
            spreadedDir.z += Random.Range(-spread, spread);
            float finalSpeed = speed;
            if (speed <= 0)
                finalSpeed = range / duration;
            ProyectileFactory.RequestSpawnProyectile(proyectile,character.gameObject,point.position, spreadedDir, finalSpeed, duration, GetDamage(), explosionForceMult, explosionScale, explosion,this.name);
            //p.onExplode -= OnExplode;
            //p.onExplode += OnExplode;
            //Esto de que no se puede acceder a un objeto spawneado por el servidor desde el cliente que manda el mensaje.
        }
        return true;
    }

    virtual protected void OnExplode (Proyectile p, Explosion e)
    {
        e.onHit += OnExplosionHit;
        p.onExplode -= OnExplode;
    }

    virtual protected void OnExplosionHit (Explosion e, Collider c)
    {

    }
}