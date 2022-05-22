using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity, IHitable
{
    public delegate void CharDamageDelegate(Character victim, GameObject attacker, SHitInfo info);
    public delegate void CharAttackDelegate(GameObject victim, Character attacker, SHitInfo info);
    static public CharDamageDelegate onCharacterDamaged;
    public CharDamageDelegate onTakeDamage;
    public CharAttackDelegate onAttacked;
    public CharDamageDelegate onKilled;
    public delegate void CharBuffD(BuffData b);
    public CharBuffD onBuffAdded;
    public CharBuffD onBuffRemoved;

    [SerializeField] protected Transform[] points;
    [SerializeField] protected Hitbox[] hboxes;
    [SerializeField] protected ModelAttachment[] attachments;
    protected List<BuffData> buffs = new List<BuffData>();
    [SerializeField] protected float mana;
    [SerializeField] protected float manaMax;
    [SerializeField] protected float manaRegeneration;
    protected Attack currentAttack;
    protected Animator _uAnimator;
    protected bool inAnimation;

    protected HealthComponent health;

    public Animator uAnimator
    {
        get
        {
            if (_uAnimator == null)
                _uAnimator = GetComponentInChildren<Animator>();

            return _uAnimator;
        }
        set
        {
            _uAnimator = value;
        }
    }

    virtual public bool invunerable
    {
        get
        {
            return health.invunerable;
        }
        set
        {
            health.invunerable = value;
        }
    }

    virtual public bool isGrounded
    {
        get
        {
            return true;
        }
    }

    virtual public Vector3 charInput
    {
        get
        {
            return Vector3.zero;
        }
    }

    virtual public bool freeze
    {
        get
        {
            return false;
        }
        set
        {

        }
    }

    public float Mana
    {
        get => mana;
        set => mana = value;
    }

    public float ManaMax
    {
        get => manaMax;
    }

    public Hitbox GetHbox(string _hboxId)
    {
        foreach (var item in hboxes)
        {
            if (item.name == _hboxId)
                return item;
        }
        return null;
    }


    virtual public void LookAtPos(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        dir.y = .0f;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    virtual public void LookAtPos(Transform pos, float smooth, float duration)
    {
        StartCoroutine(LookAtPosSequence(pos, smooth, duration));
    }

    virtual public void LookAtPos(Vector3 pos, float smooth, float duration)
    {
        StartCoroutine(LookAtPosSequence(pos, smooth, duration));
    }

    IEnumerator LookAtPosSequence(Transform pos, float smooth, float duration)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            Vector3 dir = pos.position - transform.position;
            dir.y = .0f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), t * smooth * Mathf.Deg2Rad);
            yield return null;
        }
    }

    IEnumerator LookAtPosSequence(Vector3 pos, float smooth, float duration)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            Vector3 dir = pos - transform.position;
            dir.y = .0f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * smooth * Mathf.Deg2Rad);
            yield return null;
        }
    }

    virtual public void PlayAnimation(string trigger, float duration)
    {
        try
        {
            StopCoroutine("AnimationSequence");
            StartCoroutine(AnimationSequence(trigger, duration));
        }
        catch
        {

        }
    }

    virtual protected IEnumerator AnimationSequence(string trigger, float duration)
    {
        _uAnimator.SetTrigger(trigger);
        inAnimation = true;
        yield return new WaitForSeconds(duration);
        inAnimation = false;
    }

    virtual public bool PerformAttack(Attack atk, GameObject target = null)
    {
        if (currentAttack != null)
            return false;
        Attack f = Attack.InstantiateAtk(this, atk);
        currentAttack = f;
        currentAttack.StartAttack(this, target);
        PlayAnimation(atk.trigger, atk.endTime);
        currentAttack.onAttackEnd += OnAttackEnd;
        return true;
    }

    virtual public void CancelAttack()
    {
        if (currentAttack != null)
        {
            currentAttack = null;
            currentAttack.onAttackEnd -= OnAttackEnd;
        }
    }

    virtual public void OnAttackEnd(Attack atk)
    {
        currentAttack = null;
        atk.onAttackEnd -= OnAttackEnd;
    }

    public HealthComponent Health
    {
        get => health;
    }

    virtual protected void Awake()
    {
        health = GetComponent<HealthComponent>();
        health.onDie += OnDie;
        onTakeDamage += OnTakeDamage;
        _uAnimator = GetComponentInChildren<Animator>();
    }

    virtual protected void OnTakeDamage(Character character, GameObject target, SHitInfo hit)
    {

    }

    virtual protected void OnDie(HealthComponent health, float dmg, SHitInfo info)
    {
        onDie?.Invoke(this, info);
        Destroy(gameObject);
    }

    virtual protected void Update()
    {
        mana += manaRegeneration * Time.deltaTime;
        mana = Mathf.Clamp(mana, 0, manaMax);
        currentAttack?.UpdateAttack(Time.deltaTime);
    }

    virtual public void UpdateBuffs()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            buffs[i].UpdateBuff(this, Time.deltaTime);
        }
    }

    virtual public void AddBuff(BuffData b)
    {
        BuffData existent = buffs.Find(x => x.name == b.name + "(Clone)");
        if (existent != null)
        {
            existent.EndBuff(this);
            existent.StartBuff(this);
            return;
        }
        BuffData newBuff = SkillUtlity.InstantiateBuff(b);
        buffs.Add(newBuff);
        newBuff.StartBuff(this);
        onBuffAdded?.Invoke(b);
    }

    virtual public bool HasBuff(BuffData b)
    {
        BuffData existent = buffs.Find(x => x.name == b.name + "(Clone)");
        if (existent != null)
        {
            return true;
        }
        return false;
    }

    virtual public void EndBuff(BuffData b)
    {
        onBuffRemoved?.Invoke(b);
        b.EndBuff(this);
        buffs.Remove(b);
    }

    float IHitable.TakeDamage(SHitInfo hit)
    {
        if (health.Health <= 0.0f)
            return 0f;
        bool canDamage = false;
        try
        {
            canDamage = GameModeManager.instance.currentGameMode.CanDamagePlayer(hit.attacker.GetComponent<PlayerState>(), GetComponent<PlayerState>());
        }
        catch
        {

        }

        if (!canDamage && hit.skill != "suicide")
            return 0;
        onCharacterDamaged?.Invoke(this, hit.attacker, hit);
        onTakeDamage?.Invoke(this, hit.attacker, hit);
        float dmg = health.TakeDamage(hit.dmg,hit);
        if (health.Health <= 0.0f)
        {
            onKilled?.Invoke(this, hit.attacker, hit);
        }
        if (hit.buff != null && dmg>0)
            AddBuff(hit.buff);
        if (hit.attacker != null) {
            Character c = hit.attacker.GetComponent<Character>();
            if (c != null) {
                c.onAttacked?.Invoke(hit.victim.gameObject, c, hit);
            }
        }
        return hit.dmg;
    }

    public Transform GetPoint(string pname)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].name == pname)
                return points[i];
        }
        return null;
    }

    public bool ToggleAttachment(string pname, bool val)
    {
        for (int i = 0; i < attachments.Length; i++)
        {
            if (attachments[i].name == pname)
            {
                attachments[i].Toggle(val);
                return true;
            }
        }
        return false;
    }

}