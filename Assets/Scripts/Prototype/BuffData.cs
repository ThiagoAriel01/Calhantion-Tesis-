using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//[CreateAssetMenu(menuName = "Data/Buff")]
public abstract class BuffData : ScriptableObject
{
    [SerializeField] protected string displayEffect;
    [SerializeField] protected AudioCue _cueStart;
    [SerializeField] protected Color displayColor;
    [SerializeField] protected ParticleSystem fxPrefab;
    [SerializeField] protected float duration;
    [SerializeField] protected float rate;
    protected bool isInstance;
    protected float timer;
    protected int curRates=0;
    protected ParticleSystem fxInstance;

    public bool IsInstance
    {
        get
        {
            return isInstance;
        }
    }

    public string DisplayEffect
    {
        get
        {
            return displayEffect;
        }
    }

    public Color DisplayColor
    {
        get
        {
            return displayColor;
        }
    }

    public virtual void Init()
    {
        if (isInstance)
            return;
        isInstance = true;
        //fxInstance = Instantiate(fxPrefab);
        //fxInstance.gameObject.SetActive(false);
    }

    virtual public void UpdateBuff (Character character, float deltaTime)
    {
        timer += deltaTime;
        if (timer>=rate * curRates)
        {
            curRates++;
            OnRate(character);
        }
        if (timer >= duration)
            character.EndBuff(this);
    }

    virtual public bool OnRate (Character character)
    {
        return true;
    }

    virtual public bool StartBuff(Character character)
    {
        timer = 0.0f;
        character.onDie -= OnDie;
        character.onDie += OnDie;
        character.onTakeDamage -= OnTakeDamage;
        character.onTakeDamage += OnTakeDamage;
        character.onAttacked -= OnAttacked;
        character.onAttacked += OnAttacked;
        curRates = 0;
        if (_cueStart != null)
            _cueStart.PlaySound(character.transform.position, character.GetComponent<NetworkIdentity>());
        /*fxInstance.gameObject.SetActive(true);
        fxInstance.transform.SetParent(character.transform);
        fxInstance.transform.localPosition = Vector3.zero;
        fxInstance.transform.localRotation = Quaternion.identity;
        fxInstance.transform.localScale = character.transform.localScale;
        if (!fxInstance.isPlaying)
            fxInstance.Play(true);*/
        return true;
    }

    virtual public void OnDie(Entity entity, SHitInfo info)
    {

    }

    virtual public void OnTakeDamage (Character victim, GameObject attacker, SHitInfo info)
    {

    }

    virtual public void OnAttacked (GameObject victim, Character attacker, SHitInfo info)
    {

    }

    virtual public bool EndBuff(Character character)
    {
        character.onDie -= OnDie;
        character.onTakeDamage -= OnTakeDamage;
        character.onAttacked -= OnAttacked;
        timer = 0.0f;
        //fxInstance.Stop(true);
        //Destroy(fxInstance, 3f);
        curRates = 0;
        return true;
    }
}
