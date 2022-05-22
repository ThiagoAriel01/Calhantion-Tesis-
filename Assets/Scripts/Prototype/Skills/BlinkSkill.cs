using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Blink Skill", menuName = "Data/Skills/Blink Skill", order = 0)]
public class BlinkSkill : PhaseSkill
{
    [SerializeField] protected float range;
    [SerializeField] protected string castPoint;
    [SerializeField] protected LayerMask _layers;
    protected Vector3 _targetPos;
    protected Vector3 _startPos;

    public override bool PrepareToCast(Character character)
    {
        if (base.PrepareToCast(character))
        {
            Transform point = character.GetPoint(castPoint);
            Vector3 dir = point.forward;
            PreviewManager.instance.InitPreview(this, dir, 0f, 3f, range / duration, range,3f);
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
        if (!CanCast(character))
            return false;
        base.Cast(character, target, direction);

        _startPos = character.transform.position;
        Transform point = character.GetPoint(castPoint);
        Vector3 dir = point.transform.forward;
        if (direction != Vector3.zero)
            dir = direction;

        RaycastHit hit;
        _targetPos = point.position + direction.normalized * range;
        if (Physics.Raycast(point.position,direction,out hit, range, _layers))
        {
            _targetPos = hit.point;
        }
        character.freeze = true;
        Vector3 dire = (_targetPos - _startPos) / duration;
        character.Velocity = dire;
        return true;
    }

    public override bool ForceCancel(Character character)
    {
        if (_phasing)
            EndPhase(character);
        return true;
    }

    protected override void EndPhase(Character character)
    {
        base.EndPhase(character);
        character.Velocity = Vector3.zero;
        character.freeze = false;
    }

}
