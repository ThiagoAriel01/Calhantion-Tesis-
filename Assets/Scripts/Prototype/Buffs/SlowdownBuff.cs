using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Slowdown Buff")]
public class SlowdownBuff : BuffData
{
    [SerializeField] protected float speedMult = 0.25f;
    [SerializeField] protected float jumpMult = 0.25f;

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

    public override void OnDie(Entity entity, SHitInfo info)
    {
        base.OnDie(entity, info);
    }

    override public bool EndBuff(Character character)
    {
        base.EndBuff(character);
        ProtoPlayerMP ps = character.GetComponent<ProtoPlayerMP>();
        ps.moveSpeed /= speedMult;
        ps.jumpForce /= jumpMult;
        ps.moveSpeedAir /= speedMult;
        ps.jumpForceAir /= jumpMult;
        return true;
    }
}
