using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/AreaSkill")]
public class AreaSkillData : SkillData
{

    [SerializeField] protected float range;
    [SerializeField] protected float radius;
    [SerializeField] protected float duration;
    [SerializeField] protected Explosion explosion;
    [SerializeField] protected float rate = 0.0f;
    [SerializeField] protected string castPoint;
    [SerializeField] protected LayerMask layers;


    public override bool PrepareToCast(Character character)
    {
        if (base.PrepareToCast(character))
        {
            Transform point = character.GetPoint(castPoint);
            Vector3 dir = point.forward;
            PreviewManager.instance.InitPreview(this, dir, radius*2,range);
            return true;
        }
        return false;
    }

    public override void CancelCast(Character character)
    {
        PreviewManager.instance.EndPreview();
    }

    public override bool Cast(Character character, GameObject target, Vector3 direction)
    {
        PreviewManager.instance.EndPreview();
        if (!base.Cast(character, target, direction))
            return false;

        Transform point = character.GetPoint(castPoint);
        Vector3 dir = point.forward;

        if (direction != Vector3.zero)
            dir = direction;

        Vector3 targetPos = point.transform.position + dir * range;
        RaycastHit hit;
        if (Physics.Raycast(point.position,dir,out hit, range, layers))
        {
            targetPos = hit.point;
        }

        dir.y =.0f;
        ProyectileFactory.RequestSpawnExplosion(explosion, character.gameObject, targetPos, dir, damage, damage,radius/2, rate, duration,this.name);
        //Explosion e = Instantiate(explosion, targetPos, Quaternion.identity);
       // e.gameObject.SetActive(true);
        //e.Init(damage, damage, radius / 2, character.gameObject, rate,duration);

        return true;
    }
}
