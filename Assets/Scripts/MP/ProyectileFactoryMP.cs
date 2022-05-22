using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ProyectileFactoryMP : MonoSingleton<ProyectileFactoryMP>, INetworkHandler
{
    [SerializeField] protected LayerMask _coverLayers;

    static public LayerMask coverLayers
    {
        get
        {
            return instance._coverLayers;
        }
    }

    public struct ProyectileMessage : NetworkMessage
    {
        public string prefab;
        public Vector3 pos;
        public Vector3 direction;
        public GameObject owner;
        public float spd;
        public float duration;
        public float damage;
        public float force;
        public float scale;
        public string pexplosion;
        public string skill;
    }

    public struct ExplosionMessage : NetworkMessage
    {
        public string prefab;
        public Vector3 pos;
        public Vector3 direction;
        public GameObject owner;
        public float damage;
        public float force;
        public float scale;
        public float rate;
        public float duration;
        public string skill;
    }
    public struct GameObjectMessage : NetworkMessage
    {
        public string id;
        public Vector3 pos;
        public Vector3 direction;
        public GameObject owner;
    }

    public struct UnSpawnMessage : NetworkMessage
    {
        public GameObject obj;
    }

    override protected bool Awake()
    {
        if (!base.Awake())
            return false;
        GameObject[] pros = Resources.LoadAll<GameObject>("Proyectiles/");
        for (int i = 0; i < pros.Length; i++)
        {
            TesisNetworkManager.instancia.spawnPrefabs.Add(pros[i].gameObject);
        }
        (this as INetworkHandler).RegisterHandlers();
        ProyectileFactory.onSpawnProyectile += OnSpawnProyectile;
        ProyectileFactory.onSpawnExplosion += OnSpawnExplosion;
        ProyectileFactory.onSpawnGameObject += OnSpawnGameObject;
        ProyectileFactory.onUnSpawnGameObject += OnUnSpawnGameObject;
        return true;
    }

    void INetworkHandler.RegisterHandlers()
    {
        NetworkServer.RegisterHandler<ProyectileMessage>(OnProyectile);
        NetworkServer.RegisterHandler<ExplosionMessage>(OnExplosion);
        NetworkServer.RegisterHandler<GameObjectMessage>(OnGameObject);
        NetworkServer.RegisterHandler<UnSpawnMessage>(OnUnSpawn);
    }

    [Server]
    static public void UnSpawn(GameObject p)
    {
        NetworkServer.UnSpawn(p);
    }

    [Server]
    static public void OnUnSpawn(NetworkConnection conn, UnSpawnMessage msg)
    {
        UnSpawn(msg.obj);
        Destroy(msg.obj.gameObject);
    }

    [Server]
    void OnProyectile(NetworkConnection conn, ProyectileMessage msg)
    {
        Proyectile p = ProyectileFactory.SpawnProyectile(Resources.Load<Proyectile>(msg.prefab), msg.owner, msg.pos, msg.direction);
        ApplyProyectile(p, conn, msg);
    }

    [Server]
    void OnGameObject(NetworkConnection conn, GameObjectMessage msg)
    {
        NetworkServer.Spawn(Instantiate(Resources.Load<GameObject>(msg.id), msg.owner.transform));
    }

    [Server]
    void OnExplosion(NetworkConnection conn, ExplosionMessage msg)
    {
        Explosion p = ProyectileFactory.SpawnExplosion(Resources.Load<Explosion>(msg.prefab), msg.owner, msg.pos,msg.direction,msg.damage,msg.force,msg.scale);
        p.enabled = true;
        p.SendMessage("SetOwner",msg.owner.gameObject,SendMessageOptions.DontRequireReceiver);
        p.Init(msg.damage, msg.force, msg.scale, msg.owner,msg.rate,msg.duration,msg.skill);
        NetworkServer.Spawn(p.gameObject);
        p.enabled = true;
    }

    [Server]
    void ApplyProyectile(Proyectile p, NetworkConnection conn, ProyectileMessage msg)
    {
        p.Init(msg.owner, msg.direction, msg.spd, msg.duration, msg.damage, msg.force, msg.scale, Resources.Load<Explosion>(msg.pexplosion),msg.skill);
        NetworkServer.Spawn(p.gameObject);
        p.enabled = true;
    }

    void OnSpawnProyectile(Proyectile prefab, GameObject owner, Vector3 pos, Vector3 direction, float spd, float duration, float damage, float force, float scale, Explosion pexplosion, string skill="")
    {

        ProyectileMessage msg = new ProyectileMessage()
        {
            prefab = "Proyectiles/" + prefab.name,
            pos = pos,
            direction = direction,
            spd = spd,
            owner = owner,
            duration = duration,
            damage = damage,
            force = force,
            scale = scale,
            skill = skill,
            pexplosion = "Proyectiles/" + pexplosion.name
        };

        NetworkClient.Send(msg);
    }

    void OnSpawnExplosion(Explosion prefab, GameObject owner, Vector3 pos, Vector3 direction, float damage, float force, float scale, float rate, float duration, string skill)
    {
        ExplosionMessage msg = new ExplosionMessage()
        {
            prefab = "Proyectiles/" + prefab.name,
            pos = pos,
            direction = direction,
            damage = damage,
            owner = owner,
            force = force,
            scale = scale,
            rate = rate,
            duration = duration,
            skill = skill
        };

        NetworkClient.Send(msg);
    }

    void OnSpawnGameObject(GameObject id, GameObject owner, Vector3 pos, Quaternion rot)
    {
        GameObjectMessage msg = new GameObjectMessage()
        {
            id = "Prefabs/" + id.name,
            pos = pos,
        };
        NetworkClient.Send(msg);
    }

    void OnUnSpawnGameObject(GameObject iobd)
    {
        UnSpawnMessage msg = new UnSpawnMessage()
        {
            obj = iobd
        };
        NetworkClient.Send(msg);
    }
}
