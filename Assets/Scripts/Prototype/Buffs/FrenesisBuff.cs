using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Buffs/Frenesis")]
public class FrenesisBuff : BuffData
{
    [SerializeField] protected float speedMult = 1.25f;
    [SerializeField] protected float jumpMult = 1.25f;
    [SerializeField] protected float vampireHealPercent = 0.25f;
    [SerializeField] protected BuffData buffOnEnd;

    override public void UpdateBuff(Character character, float deltaTime)
    {
        base.UpdateBuff(character, deltaTime);
    }

    override public bool StartBuff(Character character)
    {
        base.StartBuff(character);
        ProtoPlayerMP ps = character.GetComponent<ProtoPlayerMP>();
        ps.moveSpeed *= speedMult;
        ps.jumpForce *= jumpMult;
        ps.moveSpeedAir *= speedMult;
        ps.jumpForceAir *= jumpMult;
        return true;
    }

    public override void OnAttacked(GameObject victim, Character attacker, SHitInfo info)
    {
        base.OnAttacked(victim, attacker, info);
        attacker.Health.Heal((int)(info.dmg * vampireHealPercent));
    }

    public override void OnDie(Entity entity, SHitInfo info)
    {
        base.OnDie(entity,info);
    }

    override public bool EndBuff(Character character)
    {
        base.EndBuff(character);
        ProtoPlayerMP ps = character.GetComponent<ProtoPlayerMP>();
        ps.moveSpeed /= speedMult;
        ps.jumpForce /= jumpMult;
        ps.moveSpeedAir /= speedMult;
        ps.jumpForceAir /= jumpMult;
        if (buffOnEnd != null)
        {
            character.AddBuff(buffOnEnd);
        }
        return true;
    }
}
