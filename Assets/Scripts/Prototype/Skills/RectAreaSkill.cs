using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Skills/Rect Area Skill")]
public class RectAreaSkill : AreaSkillData
{
    [SerializeField] protected Vector3 _scale;

    public override bool PrepareToCast(Character character)
    {
        if (CanCast(character))
        {
            Transform point = character.GetPoint(castPoint);
            Vector3 dir = point.forward;
            PreviewManager.instance.InitPreviewCube(this, _scale, range, true);
            return true;
        }
        return false;
    }
}
