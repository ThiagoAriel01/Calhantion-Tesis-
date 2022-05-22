using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "New Dash Skill", menuName = "Data/Dash Skill", order = 0)]
public class DashSkill : SkillData
{
    [SerializeField] protected AudioCue _cue;
    [SerializeField] protected float distance;
    [SerializeField] protected float time;
    [SerializeField] protected bool toLookDir;
    [SerializeField] protected AnimationCurve ac;
    protected Vector3 desiredDirection;
    protected bool dashing = false;
    protected float t = 0;

    public override bool Cast(Character character, GameObject target, Vector3 dir)
    {
        if (!base.Cast(character,target,dir))
            return false;
        desiredDirection = character.charInput.normalized;
        if (toLookDir)
        {
            Transform p = character.GetPoint("proyectilePoint");
            Vector3 relativeInput = p.InverseTransformDirection(character.charInput.normalized);
            relativeInput = p.transform.forward * relativeInput.z + p.transform.right * relativeInput.x;
            desiredDirection = relativeInput;
        }
        character.uAnimator.SetTrigger("dash");
        if (_cue != null)
            _cue.PlaySound(character.transform.position, character.GetComponent<NetworkIdentity>());
        character.freeze = true;
        t = 0;
        dashing = true;
        return true;
    }

    public override bool SkillUpdate(Character character, float deltaTime)
    {
        if (!base.SkillUpdate(character,deltaTime))
            return false;
        if (dashing)
        {
            HandleDash(character, deltaTime);
            t += Time.deltaTime / time;
            if (t >= 1)
            {
                dashing = false;
                character.freeze = false;
                character.Velocity = desiredDirection * (distance / time) * ac.Evaluate(1);
            }
        }
        return true;
    }

    virtual protected void HandleDash(Character character, float deltaTime)
    {
        character.Velocity = desiredDirection * (distance / time) * ac.Evaluate(t);
    }
}
