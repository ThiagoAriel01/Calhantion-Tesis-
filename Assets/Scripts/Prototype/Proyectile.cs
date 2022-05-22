using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour
{
    [SerializeField] protected bool destroyOnTouch=false;
    [SerializeField] protected bool stopOnTouch = true;
    [SerializeField] protected bool destroyOnDuration=false;
    [SerializeField] protected bool explodesOnDuration = false;
    [SerializeField] protected bool explodesOnTouch = true;
    [SerializeField] protected bool explodesOnlyOnEnemies = false;
    [SerializeField] protected bool penetrates = false;
    [SerializeField] protected bool usesRate;
    [SerializeField] protected float explosionRate;
    [SerializeField] protected float explosionDuration;
    public delegate void ProyectileDelegate(Proyectile p, Explosion e);
    public ProyectileDelegate onExplode;
    private Explosion explosion;
    private Rigidbody body;
    private float dmg;
    private bool exploded;
    private float force;
    private bool inited;
    private float scale;
    private List<Collider> hittedObjs = new List<Collider>();
    private CapsuleCollider col;
    private GameObject owner;
    private float dur;
    private string _skill;
    private float speed;
    private bool entered;

    public GameObject Owner
    {
        get => owner;
    }
    public float Dmg
    {
        get => dmg;
    }
    public Explosion Explo
    {
        get => explosion;
    }
    public float Duration
    {
        get => dur;
    }
    public float Scala
    {
        get => scale;
    }
    public float Force
    {
        get => force;
    }
    public float Speed
    {
        get => speed;
    }

    public void Init(GameObject owner, Vector3 direction, float spd, float duration, float damage, float force, float scale, Explosion pexplosion, string skill = "")
    {
        exploded = false;
        dmg = damage;
        dur = duration;
        this.owner = owner;
        this.force = force;
        this.scale = scale;
        speed = spd;
        _skill = skill;
        explosion = pexplosion;
        Physics.IgnoreCollision(GetComponent<Collider>(), owner.GetComponent<Collider>(), true);
        body.velocity = Vector3.zero;
        body.AddForce(direction.normalized * spd, ForceMode.VelocityChange);
        inited = true;
        RefreshHitboxes();
        body.transform.rotation = Quaternion.LookRotation(direction);

        RaycastHit hit;
        string[] lyrs = new string[] { "Default", "World", "PlayerCharacter", "Characters" };
        LayerMask layers = LayerMask.GetMask(lyrs);
        if (Physics.Raycast(transform.position,direction,out hit, spd * Time.fixedDeltaTime, layers))
        {
            EvaluateCollider(hit.collider,hit.point);
        }

        if (!explodesOnDuration)
            Invoke("StopProyectile", duration);
        else
            Invoke("ExplodeAtMe",duration);

        if (penetrates)
            Invoke("DestroyNow",duration);
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        col = GetComponent<CapsuleCollider>();
    }

    private void OnTriggerEnter(Collider cole)
    {
        if (!enabled)
            return;
        EvaluateCollider(cole, cole.ClosestPoint(transform.position));
    }

    private void EvaluateCollider(Collider cole, Vector3 point)
    {
        bool isEnemy = false;
        try
        {
            Entity e = owner.GetComponent<Entity>();
            Entity ce = cole.GetComponent<Entity>();

            if (e == ce)
                return;

            if (ce != null && e != null && ce.IsFriendly(e))
                return;
            if (ce != null && e != null && !ce.IsFriendly(e))
                isEnemy = true;
        }
        catch
        {

        }

        if (penetrates && hittedObjs.Contains(cole))
            return;

        if (explodesOnlyOnEnemies && !isEnemy)
            return;

        if (explodesOnTouch && (cole.gameObject != owner.gameObject))
        {
            ExplodeAt(point);
            hittedObjs.Add(cole);
        }
    }

    /*private void FixedUpdate()
    {
        col.height = Time.fixedDeltaTime * body.velocity.magnitude * 2f;
        Vector3 c = col.center;
        c.z = col.height / 2;
        col.center = c;
        if (body.velocity.magnitude>0.25f && !exploded)
            transform.rotation = Quaternion.LookRotation(body.velocity);
    }*/

    private void FixedUpdate()
    {
        if (!enabled)
            return;
        RefreshHitboxes();
    }

    void RefreshHitboxes()
    {
        col.height = Mathf.Clamp(Time.fixedDeltaTime * body.velocity.magnitude * 2f,1f,9999f);
        Vector3 c = col.center;
        c.z = col.height / 2;
        col.center = c;
        if (body.velocity.magnitude > 0.1f && !exploded)
            transform.rotation = Quaternion.LookRotation(body.velocity);
    }

    private void OnDestroy()
    {
        ProyectileFactory.UnSpawn(gameObject);
    }

    void ExplodeAt(Vector3 atPos)
    {
        if (!inited)
            return;
        if (stopOnTouch)
            body.velocity = Vector3.zero;
        if (exploded && !penetrates)
            return;

        if (usesRate)
            ProyectileFactory.RequestSpawnExplosion(explosion, owner, atPos, transform.forward, dmg, force, scale, explosionRate,explosionDuration, _skill);
        else
            ProyectileFactory.RequestSpawnExplosion(explosion, owner, atPos, transform.forward, dmg, force, scale, _skill);
        //explo.transform.position = atPos;

        onExplode?.Invoke(this, null);
        //explo.gameObject.SetActive(true);
        //explo.Init(dmg, force, scale, owner);
        try
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), owner.GetComponent<Collider>(), false);
        }
        catch
        {

        }
        exploded = true;
        if (!penetrates)
            StopProyectile();
        CancelInvoke("StopProyectile");
        CancelInvoke("ExplodeAtMe");
    }

    void StopProyectile()
    {
        exploded = true;
        DisableAllParticles(transform);
        //body.velocity = Vector3.zero;
        if (!destroyOnTouch)
            Destroy(gameObject, 5);
        else
            Destroy(gameObject);
    }

    void DisableAllParticles (Transform curTransform)
    {
        if (curTransform == null)
            return;

        ParticleSystem ps = curTransform.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            curTransform.GetComponent<ParticleSystem>().Stop(true);
            return;
        }
        for (int i = 0; i < curTransform.childCount; i++)
        {
            DisableAllParticles(curTransform.GetChild(i));
        }
    }

    void DestroyNow()
    {
        Destroy(gameObject);
    }

    void ExplodeAtMe()
    {
        ExplodeAt(transform.position);
        if (destroyOnDuration)
        {
            Destroy(gameObject);
        }
    }
}
