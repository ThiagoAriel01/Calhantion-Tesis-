using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Skills/Give Buff Skill")]
public class GiveBuffSkill : SkillData
{
    [SerializeField] protected BuffData _buff;

    public override bool Cast(Character character, GameObject target, Vector3 direction)
    {
        if (!CanCast(character))
            return false;
        base.Cast(character, target, direction);
        character.GetComponent<ProtoPlayerMP>().ClientAddBuff(_buff.name);
        return true;
    }
}
