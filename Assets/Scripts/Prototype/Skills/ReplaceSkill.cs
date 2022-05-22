using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "New Replace Skill", menuName = "Data/Skills/Replace Skill", order = 0)]
public class ReplaceSkill : PhaseSkill
{
    [SerializeField] protected GameObject _decoy;
    [SerializeField] protected BuffData _waitingHitBuff;
    [SerializeField] protected BuffData[] _hittedBuffs;
    [SerializeField] protected float _damageAbsorbRate = 0.2f;
    [SerializeField] protected float _damageThesehold = 25f;
    [SerializeField] protected float _hitMaxDuration = 3f;
    [SerializeField] protected bool _ignoreHitCondition;
    protected float _hitT = 0.0f;

    public override bool Cast(Character character, GameObject target, Vector3 dir)
    {
        if (!CanCast(character))
        {
            return false;
        }
        currentCooldown = cooldown;
        character.Mana -= mana;
        ProtoPlayerMP mp = character.GetComponent<ProtoPlayerMP>();
        RemoveDelegates(mp);
        mp.onTakeDamage += OnTakeDamage;
        mp.ClientAddBuff(_waitingHitBuff.name);
        _using = true;
        t = 0;
        _hitT = .0f;
        if (_ignoreHitCondition)
        {
            OnTakeDamage(character, null, new SHitInfo() { dmg = 9999f });
            _phasing = true;
        }
        else
        {
            mp.GetComponent<HealthComponent>().nextAbsorb = _damageAbsorbRate;
            _phasing = false;
        }
        return true;
    }

    public override bool SkillUpdate(Character character, float deltaTime)
    {
        if (!base.SkillUpdate(character, deltaTime))
            return false;
        if (_using && !_phasing)
            _hitT += deltaTime;
        if (_hitT >= _hitMaxDuration && !_phasing)
            EndCast(character);
        return true;
    }

    protected void EndCast(Character character)
    {
        _hitT = .0f;
        _using = false;
        ProtoPlayerMP mp = character.GetComponent<ProtoPlayerMP>();
        RemoveDelegates(mp);
        mp.ClientRemoveBuff(_waitingHitBuff.name);
        mp.GetComponent<HealthComponent>().nextAbsorb = 1f;
        mp.GetComponent<PlayerHeadUI>().Show();
        mp.ShowPlayerModel(true);
    }

    void RemoveDelegates(ProtoPlayerMP mp)
    {
        try
        {
            mp.onTakeDamage -= OnTakeDamage;
        }
        catch
        {

        }
    }

    protected override void EndPhase(Character character)
    {
        base.EndPhase(character);
        EndCast(character);
    }

    void OnTakeDamage (Character character, GameObject attacker, SHitInfo info)
    {
        if (!_using || info.dmg / _damageAbsorbRate < _damageThesehold)
            return;

        t = 0;
        _hitT = .0f;
        _using = true;
        _phasing = true;
        ProtoPlayerMP mp = character.GetComponent<ProtoPlayerMP>();
        PlayerSkills pr = character.GetComponent<PlayerSkills>();
        RemoveDelegates(mp);
        mp.ClientRemoveBuff(_waitingHitBuff.name);
        for (int i = 0; i < _hittedBuffs.Length; i++)
        {
            mp.ClientAddBuff(_hittedBuffs[i].name);
        }
        StartPhase(character);
        pr.tdelay = duration + postDelay;
        mp.GetComponent<PlayerHeadUI>().Hide();
        SpawnerMP.instance.SpawnPrefab(_decoy.name, character.transform.position, character.transform.rotation,
            null, (x) => OnDecoySpawn(x),mp.netIdentity);
    }
    public override bool ForceCancel(Character character)
    {
        if (_phasing)
        {
            EndPhase(character);
            return true;
        }
        if (_hitT > 0f)
            EndCast(character);
        return true;
    }

    virtual protected void OnDecoySpawn (NetworkIdentity ni)
    {
        //asd
    }
}
