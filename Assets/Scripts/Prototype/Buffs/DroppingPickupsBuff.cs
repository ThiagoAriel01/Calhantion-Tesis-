using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Dropping Pickups Buff")]
public class DroppingPickupsBuff : BuffData
{
    [SerializeField] protected PickUpable pickupPrefab;
    protected int dropCount;

    override public void UpdateBuff(Character character, float deltaTime)
    {
        base.UpdateBuff(character, deltaTime);
    }

    override public bool StartBuff(Character character)
    {
        base.StartBuff(character);
        dropCount = 0;
        return true;
    }
    public override void OnDie(Entity entity, SHitInfo info)
    {
        int totalDrops = (int)(duration / rate);
        int dropRemaining = totalDrops - dropCount;
        for (int i = 0; i < dropRemaining; i++)
        {
            SpawnPickup(entity);
        }
        base.OnDie(entity,info);
    }

    override public void OnTakeDamage(Character victim, GameObject attacker, SHitInfo info)
    {
        timer -= rate;
        if (timer < 0)
            timer = 0;
        curRates--;
        if (curRates < 0)
            curRates = 0;
        dropCount--;
        if (dropCount < 0)
            dropCount = 0;
    }

    override public bool OnRate(Character character)
    {
        SpawnPickup(character);
        dropCount++;
        return true;
    }

    virtual protected void SpawnPickup(Entity character)
    {
        PickupsManager.CreateAt(pickupPrefab, character.transform.position);
    }

    override public bool EndBuff(Character character)
    {
        base.EndBuff(character);
        dropCount = 0;
        return true;
    }
}