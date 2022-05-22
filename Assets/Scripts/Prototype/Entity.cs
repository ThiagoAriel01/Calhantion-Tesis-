using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public delegate void EntityDelegate(Entity entity, SHitInfo info);
    public EntityDelegate onDie;

    [SerializeField] protected int faction;
   
    virtual public Vector3 Velocity
    {
        get
        {
            return Vector3.zero;
        }
        set 
        {

        }
    }


    public bool IsFriendly (Entity e)
    {
        return IsFriendly(e.faction);
    }

    public bool IsFriendly (int f)
    {
        return false;
    }
}
