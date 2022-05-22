using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightComponent : MonoBehaviour
{
    public delegate void SightDelegate(SightComponent sight, Entity target);
    public SightDelegate onTargetAcquired;

    [SerializeField] protected float range;
    protected SphereCollider col;
    protected List<Entity> entitiesInside = new List<Entity>();
    protected Entity target;
    protected Entity owner;

    public Entity Target
    {
        get
        {
            return target;
        }
    }

    static public SightComponent Create(Entity owner, float prange)
    {
        GameObject obj = new GameObject("Sight");
        obj.transform.SetParent(owner.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        SightComponent sc = obj.AddComponent<SightComponent>();
        sc.owner = owner;
        sc.gameObject.AddComponent<Rigidbody>().isKinematic = true;
        sc.range = prange;
        sc.Init();
        return sc;
    }

    public void Init()
    {
        col = gameObject.AddComponent<SphereCollider>();
        col.radius = range;
        col.isTrigger = true;
        col.gameObject.layer = LayerMask.NameToLayer("CharactersOnly");
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity e = other.GetComponent<Entity>();
        if (e!=null && e != owner)
        {
            entitiesInside.Add(e);
            if (!owner.IsFriendly(e))
            {
                if (target == null)
                {
                    target = e;
                    onTargetAcquired?.Invoke(this, target);
                }
                e.onDie += OnEntityInsideDies;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Entity e = other.GetComponent<Entity>();
        if (e != null && e != owner)
        {
            entitiesInside.Remove(e);
            if (!owner.IsFriendly(e))
            {
                if (target == e)
                    ChooseNewTarget();
                e.onDie -= OnEntityInsideDies;
            }
        }
    }

    void ChooseNewTarget()
    {
        if (entitiesInside.Count > 0)
            //target = entitiesInside[Random.Range(0, entitiesInside.Count)];
            target = GetClosestEntity();
        else
            target = null;

        onTargetAcquired?.Invoke(this, target);
    }

    Entity GetClosestEntity()
    {
        if (entitiesInside.Count <= 0)
            return null;

        Entity closest = null;
        if (col == null)
            return null;
        float dist = col.radius;
        foreach (Entity e in entitiesInside)
        {
            if (!e.IsFriendly(owner))
            {
                float curDist = (e.transform.position - transform.position).magnitude;
                if (curDist < dist)
                {
                    closest = e;
                    dist = curDist;
                }
            }
        }
        return closest;
    }

    public List<Entity> GetAllEntitiesInside()
    {
        return entitiesInside;
    }

    public List<Entity> GetAlliesSortedByDistance()
    {
        List<Entity> es = new List<Entity>();
        foreach (Entity e in entitiesInside)
        {
            if (e.IsFriendly(owner))
            {
                es.Add(e);
            }
        }
        return es;
    }

    float CompareDistance (float a, float b)
    {
        return a < b ? a : (b < a ? b : 0);
    }

    void OnEntityInsideDies (Entity entity, SHitInfo info)
    {
        entitiesInside.Remove(entity);
        ChooseNewTarget();
    }

}
