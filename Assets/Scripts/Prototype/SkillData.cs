using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    [SerializeField] protected string displayName;
    [TextArea(3,3)]
    [SerializeField] protected string displayDesc;
    [SerializeField] protected string[] _tags;
    [SerializeField] protected float damage;
    [SerializeField] protected float cooldown;
    [SerializeField] protected bool canReduceCooldown = true;
    [SerializeField] protected float mana;
    [SerializeField] protected float recoilVertical;
    [SerializeField] protected float recoil;
    [SerializeField] protected float delay;
    [SerializeField] protected float postDelay;
    [SerializeField] protected Sprite icon;
    public float cancelPostDelay;
    protected bool isInstance;
    protected bool _using;
    protected float currentCooldown = 0.0f;
    [SerializeField] protected bool castsPreview = true;

    public bool CanReduceCooldown
    {
        get
        {
            return canReduceCooldown;
        }
    }

    public string[] tags
    {
        get => _tags;
    }

    public bool HasTag (string ptag)
    {
        if (string.IsNullOrEmpty(ptag))
            return true;
        for (int i = 0; i < _tags.Length; i++)
        {
            if (_tags[i] == ptag)
                return true;
        }
        return false;
    }

    public bool HasTags(string[] ptags)
    {
        int matched = 0;
        for (int i = 0; i < ptags.Length && matched < ptags.Length; i++)
        {
            if (HasTag(ptags[i]))
                matched++;
        }
        return matched >= ptags.Length;
    }

    public string DisplayDesc
    {
        get
        {
            return displayDesc;
        }
    }
    public string DisplayName
    {
        get
        {
            return displayName;
        }
    }

    public bool IsUsing
    {
        get
        {
            return _using;
        }
    }

    public bool CastsPreview
    {
        get
        {
            return castsPreview;
        }
    }
    public bool IsInstance
    {
        get
        {
            return isInstance;
        }
    }

    public Sprite Icon
    {
        get
        {
            return icon;
        }
    }

    public float Cooldown
    {
        get
        {
            return cooldown;
        }
    }

    public float CurrentCooldown
    {
        get
        {
            return currentCooldown;
        }
        set
        {
            currentCooldown = value; 
        }
    }

    public float RecoilVertical
    {
        get
        {
            return recoilVertical;
        }
    }

    public float Recoil
    {
        get
        {
            return recoil;
        }
    }
    public float PostDelay
    {
        get
        {
            return postDelay;
        }
    }

    public float Mana
    {
        get
        {
            return mana;
        }
    }

    public virtual void Init()
    {
        if (isInstance)
            return;
        isInstance = true;
    }

    public virtual bool PrepareToCast(Character character)
    {
        if (CanCast(character))
            return true;
        return false;
    }

    public virtual void CancelCast(Character character)
    {

    }

    public virtual bool Cast(Character character)
    {
        return Cast(character, null,Vector3.zero);
    }

    public virtual bool Cast(Character character, GameObject target,Vector3 direction)
    {
        if (CanCast(character))
        {
            currentCooldown = cooldown;
            character.Mana -= mana;
            return true;
        }
        return false;
    }

    public virtual bool SkillUpdate(Character character, float deltaTime)
    {
        if (!isInstance)
            return false;

        HandleCooldown();
        return true;
    }

    protected virtual bool HandleCooldown()
    {
        if (currentCooldown > 0)
            currentCooldown -= Time.deltaTime;
        if (currentCooldown < 0)
            currentCooldown = 0.0f;
        return true;
    }

    public virtual bool CanCast (Character character)
    {
        if (currentCooldown<=0 && character.Mana>=mana)
            return true;
        return false;
    }   

    public virtual bool ForceCancel (Character character)
    {
        return false;
    }

    public virtual void OnSkillSucess(Character character)
    {

    }

    public virtual void OnSkillHit(Character character, GameObject victim)
    {

    }

    public virtual void OnSkillFail(Character character)
    {

    }

}