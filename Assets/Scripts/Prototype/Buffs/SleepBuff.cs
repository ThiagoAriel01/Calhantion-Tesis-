using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Buffs/Sleep")]
public class SleepBuff : BuffData
{

    public override bool StartBuff(Character character)
    {
        if (!base.StartBuff(character))
            return false;
        ProtoPlayerMP ps = character.GetComponent<ProtoPlayerMP>();
        ps.totalInputFreeze = true;
        ps.SetBool("sleep", true);
        return true;
    }

    public override void OnTakeDamage(Character victim, GameObject attacker, SHitInfo info)
    {
        base.OnTakeDamage(victim, attacker, info);
        victim.EndBuff(this);
    }

    override public bool EndBuff(Character character)
    {
        base.EndBuff(character);
        ProtoPlayerMP ps = character.GetComponent<ProtoPlayerMP>();
        PlayerState state = character.GetComponent<PlayerState>();
        if (state.alive)
            ps.totalInputFreeze = false;
        ps.SetBool("sleep", false);
        return true;
    }
}