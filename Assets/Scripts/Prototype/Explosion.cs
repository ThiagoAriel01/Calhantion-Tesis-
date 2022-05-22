using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Explosion : NetworkBehaviour
{
    public delegate void ExplosionDelegate(Explosion e, Collider other);
    public ExplosionDelegate onHit;
    [SerializeField] private float stayTime = 0.6f;
    [SerializeField] private float damage = 50f;
    [SerializeField] private float delay = 0.0f;
    [SerializeField] protected BuffData buff;
    [SerializeField] private float force = 200f;
    [SerializeField] private bool useTrigger;
    [SerializeField] protected LayerMask layers;
    [SerializeField] protected bool stealsHP;
    [SerializeField] protected bool useCustomRadius;
    [SerializeField] protected float radius;
    [SerializeField] protected bool ignoreIfHasBuff;
    [SerializeField] protected float limitHeightDamage;
    [SerializeField] protected bool checkCover;
    private Collider trigger = null;
    private float t = 0.0f;
    private GameObject[] hittedObjects;
    [SyncVar] private GameObject owner;
    protected float duration;
    private int hittedCount = 0;
    private float rate = 0.0f;
    private float defDamage;
    private float defForce;
    protected bool inited;
    protected string _skill;
    private Vector3 defScale;

    private void OnEnable()
    {
        damage = defDamage;
        force = defForce;
        transform.localScale = defScale;
        trigger.enabled = true;
        hittedCount = 0;
        for (int i = 0; i < hittedObjects.Length; i++)
        {
            hittedObjects[i] = null;
        }
        t = 0.0f;
    }

    public GameObject Owner
    {
        get
        {
            return owner;
        }
    }

    private void OnDestroy()
    {
        ProyectileFactory.UnSpawn(gameObject);
    }

    public void SetOwner(GameObject powner)
    {
        this.owner = powner;
    }

    public void Init(float dmg, float force, float scale, GameObject owner, float rate = 0.0f, float duration = 5.0f, string skill = "")
    {
        _skill = skill;
        inited = true;
        this.rate = rate;
        damage = dmg;
        this.owner = owner;
        this.force = force;
        this.duration = duration;
        transform.localScale = defScale * scale;
        if (delay > 0)
        {
            Invoke("EndInit",delay);
            return;
        }
        EndInit();
    }

    void EndInit()
    {
        enabled = true;
        CancelInvoke();
        if (!useTrigger)
        {
            if (rate > 0)
                InvokeRepeating("Explode", rate, rate);
            Explode();
        }
        else
        {
            if (rate > 0)
                InvokeRepeating("ToggleCollision", rate / 2, rate / 2);
        }
        Invoke("End", duration);
    }

    void ToggleCollision()
    {
        if (t >= stayTime)
        {
            trigger.enabled = false;
            return;
        }
        trigger.enabled = !trigger.enabled;
        if (trigger.enabled)
        {
            hittedCount = 0;
            for (int i = 0; i < hittedObjects.Length; i++)
            {
                hittedObjects[i] = null;
            }
        }
    }

    void End()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        defDamage = damage;
        defForce = force;
        defScale = transform.localScale;
        trigger = GetComponent<Collider>();
        hittedObjects = new GameObject[128];
    }

    void Explode()
    {
        if (!enabled)
            return;
        if (t >= stayTime)
        {
            return;
        }
        SphereCollider sc = trigger as SphereCollider;
        if (sc == null)
            return;
        float r = sc.radius * transform.localScale.magnitude;
        if (useCustomRadius)
            r = radius;
        Collider[] cols = Physics.OverlapSphere(transform.position, r, layers);
        foreach (Collider other in cols)
        {
            HandleCollision(other,transform.position, other.transform.position);
        }
    }

    bool CheckCover(Vector3 pos,Vector3 endpos, GameObject obj)
    {
        RaycastHit hit;
        Vector3 dir = endpos - pos;
        if (Physics.Raycast(pos - dir.normalized * 0.1f, dir, out hit,dir.magnitude + 9f, ProyectileFactoryMP.coverLayers))
        {
            if (hit.collider.gameObject != obj)
                return false;
        }
        return true;
    }

    void HandleCollision (Collider other, Vector3 atPos, Vector3 hitpoint)
    {
        onHit?.Invoke(this, other);
        try
        {
            Entity e = other.gameObject.GetComponent<Entity>();
            Entity eo = owner.GetComponent<Entity>();
            if (e == eo)
                return;
            if (e != null && eo != null && e.IsFriendly(eo))
                return;
        }
        catch
        {

        }
        Vector3 pos = other.transform.position;
        Vector3 dir = (pos - transform.position);
        //rb.AddExplosionForce(force, transform.position, 999f, 1, ForceMode.Impulse);

        if (limitHeightDamage > 0)
        {
            if (Mathf.Abs(dir.y) > limitHeightDamage)
                return;
        }
        IHitable hitable = other.gameObject.GetComponent<IHitable>();

        dir.Normalize();
        if (hitable != null)
        {

            
        if (checkCover)
        {
            if (!CheckCover(atPos, (hitpoint + Vector3.up), other.gameObject))
                return;
        }

            if (ignoreIfHasBuff) {
                try
                {
                    if (other.gameObject.GetComponent<Character>().HasBuff(buff))
                    {
                        hittedObjects[hittedCount] = other.gameObject;
                        hittedCount++;
                        return;
                    }
                }
                catch
                {

                }
            }
            SHitInfo hit;
            hit.attacker = gameObject;
            hit.direction = dir;
            hit.dmg = damage;
            hit.buff = buff;
            hit.force = force;
            hit.knockback = 2;
            hit.point = pos;
            hit.victim = other.gameObject;
            hit.attacker = owner;
            hit.skill = _skill;
            hitable.TakeDamage(hit);
            if (stealsHP)
            {
                HealthComponent pscript = owner.GetComponent<HealthComponent>();
                if (pscript!=null)
                    pscript.Heal(damage);
            }
        }
        hittedObjects[hittedCount] = other.gameObject;
        hittedCount++;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!enabled)
            return;
        if (!inited || !useTrigger)
            return;
        if (!HaveHittedObj(other.gameObject))
        {
            HandleCollision(other,transform.position,other.ClosestPointOnBounds(transform.position));
        }
    }
    bool HaveHittedObj (GameObject obj)
    {
        for (int i = 0; i < hittedObjects.Length; i++)
        {
            if (hittedObjects[i] == obj)
                return true;
        }
        return false;
    }

    private void Update()
    {
        t += Time.deltaTime;
        if (t >= stayTime)
            trigger.enabled = false;
    }

}
