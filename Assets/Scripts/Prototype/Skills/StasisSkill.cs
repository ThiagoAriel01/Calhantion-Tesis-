using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "New Stasis Skill", menuName = "Data/Stasis Skill", order = 0)]
public class StasisSkill : PhaseSkill, INetworkHandler
{

    [SerializeField] protected float _healRate = 0.5f;
    [SerializeField] protected float _healAmount = 25f;
    protected int _healRates=1;

    protected override void StartPhase(Character character)
    {
        base.StartPhase(character);
        PlayerScript plr = character.GetComponentInChildren<PlayerScript>();
        if (plr == null)
            return;
        _phasing = true;
        _healRates = 0;
        plr.freeze = true;
        plr.velocity = Vector3.zero;
    }

    protected override void EndPhase(Character character)
    {
        base.EndPhase(character);
        _healRates = 0;
        _phasing = false;
        PlayerScript plr = character.GetComponentInChildren<PlayerScript>();
        if (plr == null)
            return;

        plr.freeze = false;
    }

    public override bool ForceCancel(Character character)
    {
        if (_phasing)
            EndPhase(character);
        return true;
    }

    public override bool SkillUpdate(Character character, float deltaTime)
    {
        base.SkillUpdate(character, deltaTime);
        if (_phasing && t>=_healRate * _healRates)
        {
            character.GetComponent<HealthComponent>().CmdHeal(_healAmount);
            _healRates++;
        }
        return true;
    }
}
