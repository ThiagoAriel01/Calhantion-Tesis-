using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUtlity
{
    static public SkillData InstantiateSkill (SkillData baseSkill)
    {
        if (baseSkill == null)
            return null;
        if (baseSkill.IsInstance)
            return null;
        SkillData sk = ScriptableObject.Instantiate(baseSkill) as SkillData;
        sk.Init();
        return sk;
    }

    static public SkillData GetSkillByName (string pname, SkillData[] skills)
    {
        foreach (SkillData item in skills)
        {
            if (item.name == pname)
                return item;
        }
        return null;
    }
    static public BuffData InstantiateBuff(BuffData baseBuff)
    {
        if (baseBuff == null)
            return null;
        if (baseBuff.IsInstance)
            return null;
        BuffData bd = ScriptableObject.Instantiate(baseBuff) as BuffData;
        bd.Init();
        return bd;
    }

}