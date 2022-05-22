using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Attack")]
public class Attack : ScriptableObject
{
    public delegate void AttackDelegate(Attack atk);
    public AttackDelegate onAttackStart;
    public AttackDelegate onAttackComboLink;
    public AttackDelegate onAttackEnd;

    [SerializeField] public string trigger;
    [SerializeField] public string goreID;
    [SerializeField] public bool hasArmor;
    [SerializeField] public string hitboxID;
    [HideInInspector] public Hitbox hbox;
    [SerializeField] public float damage = 1f;
    [SerializeField] public float force = 1f;
    [SerializeField] public float multiplier = 1f;
    [SerializeField] public float hitPoint = 0.3f;
    [SerializeField] public float holdTime = 0.1f;
    [SerializeField] public float endTime = 1.25f;
    [SerializeField] public float autoAlignFactor = 1f;
    private float t = 0;
    private bool working = false;
    private Character character;
    private GameObject target;
    protected bool _isInstance;

    public bool isInstance
    {
        get
        {
            return _isInstance;
        }
    }

    static public Attack InstantiateAtk(Character owner,Attack so)
    {
        if (so.isInstance)
            return so;
        Attack atk = ScriptableObject.Instantiate(so);
        atk.Init(owner);
        return atk;
    }

    public void Init(Character character)
    {
        if (_isInstance)
            return;
        _isInstance = true;
        hbox = character.GetHbox(hitboxID);
    }


    public void StartAttack(Character character, GameObject ptarget)
    {
        if (!_isInstance)
            return;
        target = ptarget;
        this.character = character;
        t = 0;
        working = true;
        hbox.gameObject.SetActive(false);
        character.LookAtPos(ptarget.transform.position);
        //character.LookAtPos(ptarget.transform, 90 * autoAlignFactor,hitPoint / multiplier);
        hbox.DamageMultiplier = damage;
        hbox.GoreID = goreID;
        try
        {
            hbox.onHit = null;
        }
        catch
        {

        }
        hbox.onHit += OnHit;
        onAttackStart?.Invoke(this);
    }

    void OnHit (Hitbox hbox, IHitable hitted, GameObject victim)
    {
        if (!_isInstance)
            return;
        Entity en = victim.GetComponent<Entity>();
        if (en != null && en.IsFriendly(character))
            return;
        SHitInfo info = new SHitInfo();
        info.attacker = character.gameObject;
        info.direction = (character.transform.forward).normalized;
        info.dmg = damage;
        info.force = force;
        info.knockback = 0;
        info.point = hbox.transform.position;
        info.victim = victim;
        hitted.TakeDamage(info);
    }

    public void UpdateAttack (float deltaTime)
    {
        if (!working || !_isInstance)
            return;
        t += Time.deltaTime / multiplier;

        if (t >= hitPoint && t < (hitPoint) + (holdTime))
            hbox.gameObject.SetActive(true);
        if (t >= (hitPoint) + (holdTime))
        {
            hbox.gameObject.SetActive(false);
            onAttackComboLink?.Invoke(this);
        }
        if (t >= endTime)
            End();
    }
    void End()
    {
        if (!_isInstance)
            return;
        t = 0;
        working = false;
        target = null;
        onAttackEnd?.Invoke(this);
    }

    public void Skip()
    {
        if (!_isInstance)
            return;
        t = 0;
        working = false;
        hbox.gameObject.SetActive(false);
    }

}
