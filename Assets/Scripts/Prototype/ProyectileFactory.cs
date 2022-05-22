using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileFactory : MonoBehaviour
{
    public delegate void ProyectileFactoryDelegate(Proyectile prefab, GameObject owner, Vector3 pos, Vector3 direction, float spd, float duration, float damage, float force, float scale, Explosion pexplosion, string skill);
    public delegate void ExplosionFactoryDelegate(Explosion prefab, GameObject owner, Vector3 pos, Vector3 direction, float damage, float force, float scale,float rate, float duration, string skill);
    static public ProyectileFactoryDelegate onSpawnProyectile;
    static public ExplosionFactoryDelegate onSpawnExplosion;
    public delegate void GODelegate(GameObject id, GameObject owner, Vector3 pos, Quaternion rot);
    public delegate void G2ODelegate(GameObject objecto);
    static public GODelegate onSpawnGameObject;
    static public G2ODelegate onUnSpawnGameObject;

    static public void UnSpawn(GameObject p)
    {
        ProyectileFactoryMP.UnSpawn(p);
    }

    static public void RequestSpawnProyectile(Proyectile prefab, GameObject owner, Vector3 pos, Vector3 direction, float spd, float duration, float damage, float force, float scale, Explosion pexplosion, string skill = "")
    {
        //Proyectile p = NoDelegateSpawnProyectile(prefab, owner, pos, direction, spd, duration, damage, force, scale, pexplosion);
        onSpawnProyectile?.Invoke(prefab,owner, pos, direction, spd, duration, damage, force, scale, pexplosion,skill);
    }

    static public void RequestSpawnExplosion(Explosion prefab, GameObject owner, Vector3 pos, Vector3 direction, float damage, float force, float scale, string skill="")
    {
        //Proyectile p = NoDelegateSpawnProyectile(prefab, owner, pos, direction, spd, duration, damage, force, scale, pexplosion);
        onSpawnExplosion?.Invoke(prefab, owner, pos, direction, damage, force, scale,0.0f,5.0f, skill);
    }

    static public void RequestUnSpawn (GameObject p)
    {
        onUnSpawnGameObject?.Invoke(p);
    }

    static public void RequestSpawnExplosion(Explosion prefab, GameObject owner, Vector3 pos, Vector3 direction, float damage, float force, float scale, float rate, float duration, string skill = "")
    {
        //Proyectile p = NoDelegateSpawnProyectile(prefab, owner, pos, direction, spd, duration, damage, force, scale, pexplosion);
        onSpawnExplosion?.Invoke(prefab, owner, pos, direction, damage, force, scale,rate,duration,skill);
    }

    static public void RequestSpawnGameObject(GameObject id,GameObject owner, Vector3 pos, Quaternion rot)
    {
        onSpawnGameObject?.Invoke(id, owner,pos,rot);
    }

    static public Proyectile SpawnProyectile(Proyectile prefab, GameObject owner, Vector3 pos, Vector3 direction)
    {
        Proyectile p = Instantiate(prefab, pos, Quaternion.LookRotation(direction));
        p.gameObject.SetActive(true);
        return p;
    }

    static public Explosion SpawnExplosion(Explosion prefab, GameObject owner, Vector3 pos, Vector3 direction, float damage, float force, float scale)
    {
        Explosion p = Instantiate(prefab, pos, Quaternion.LookRotation(direction));
        p.gameObject.SetActive(true);
        return p;
    }
}
