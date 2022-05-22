using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private AudioCue missCue = null;
    [SerializeField] private AudioCue hitCue =null;
    private float damageMultiplier=1f;
    private AudioSource source=null;
    public delegate void HitboxDelegate(Hitbox hbox, IHitable target, GameObject victim);
    public HitboxDelegate onHit;
    private string goreID = "";
    private IHitable[] hitables = new IHitable[32];
    private int hits = 0;

    private void Awake()
    {
        //source = GetComponentInChildren<AudioSource>();
        //source.transform.SetParent(transform.parent);
        gameObject.SetActive(false);
    }

    public float DamageMultiplier
    {
        get
        {
            return damageMultiplier;
        }
        set
        {
            damageMultiplier = value;
        }
    }

    public void OnHitted ()
    {
        //AudioManager.PlayCue(hitCue,AudioManager.Channels.SFX3D,transform.position);
    }

    public void Skip()
    {
        ClearHitables();
    }

    void ClearHitables()
    {
        for (int i = 0; i < hitables.Length; i++)
        {
            hitables[i] = null;
        }
    }

    bool ContainsHitable (IHitable h)
    {
        for (int i = 0; i < hitables.Length; i++)
        {
            if (hitables[i] == h)
                return true;
        }
        return false;
    }

    private void OnEnable()
    {
        ClearHitables();
        //AudioManager.PlayCue(missCue, AudioManager.Channels.SFX3D, transform.position);
    }

    public string GoreID
    {
        get
        {
            return goreID;
        }
        set
        {
            goreID = value;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        IHitable h = other.GetComponent<IHitable>();
        if (h != null && other.gameObject != transform.root.gameObject && !ContainsHitable(h))
        {
            hitables[hits] = h;
            OnHitted();
            onHit?.Invoke(this, h,other.gameObject);
        }
    }

}
