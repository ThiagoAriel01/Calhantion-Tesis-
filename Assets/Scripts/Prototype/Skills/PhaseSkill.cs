using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "New Phase Skill", menuName = "Data/Phase Skill", order = 0)]
public class PhaseSkill : SkillData, INetworkHandler
{
    [SerializeField] protected bool _invunerablePhase=true;
    [SerializeField] protected GameObject _particles;
    [SerializeField] protected float duration = 3f;
    [SerializeField] protected float _speedMult = 1.4f;
    protected float t;
    protected GameObject _p;
    protected bool _phasing;

    protected struct PhaseMessage : NetworkMessage
    {
        public string prefab;
        public GameObject owner;
        public float speedMult;
    }

    protected struct PhaseSpawnMessage : NetworkMessage
    {
        public GameObject obj;
        public GameObject owner;
    }

    protected struct PhaseSpawnOwnerMessage : NetworkMessage
    {
        public GameObject obj;
        public GameObject owner;
    }

    protected struct PhaseEndMessage : NetworkMessage
    {
        public GameObject p;
        public GameObject owner;
        public float speedMult;
    }
    virtual protected void Awake()
    {
        (this as INetworkHandler).RegisterHandlers();
    }

    void INetworkHandler.RegisterHandlers()
    {
        NetworkServer.RegisterHandler<PhaseMessage>(OnStartPhase);
        NetworkServer.RegisterHandler<PhaseEndMessage>(OnEndPhase);
        NetworkClient.RegisterHandler<PhaseSpawnMessage>(OnRPCStartPhase);
        //NetworkClient.RegisterHandler<PhaseSpawnOwnerMessage>(OnOwnerStartPhase);
    }

    public override void Init()
    {
        base.Init();
    }

    public override bool ForceCancel(Character character)
    {
        if (_phasing)
            EndPhase(character);
        return true;
    }

    [Server]
    virtual protected void OnStartPhase(NetworkConnection id, PhaseMessage msg)
    {
        if (id != msg.owner.GetComponent<NetworkIdentity>().connectionToClient)
            return;
        //GameObject pp = Instantiate(Resources.Load<GameObject>("Prefabs/" + msg.prefab), msg.owner.transform);
        //pp.SendMessage("SetOwner", msg.owner, SendMessageOptions.DontRequireReceiver);

        //ExplosionHardSet hard = pp.GetComponent<ExplosionHardSet>();
        //if (hard!=null)
        //    pp.GetComponent<ExplosionHardSet>().Init(msg.owner.gameObject);

        //pp.transform.SetParent(msg.owner.transform);
        //pp.transform.localPosition = Vector3.zero;
        //pp.transform.localRotation = Quaternion.identity;
        ProtoPlayerMP plr = msg.owner.GetComponent<ProtoPlayerMP>();
        plr.moveSpeed *= msg.speedMult;
        plr.moveSpeedAir *= msg.speedMult;

        //NetworkServer.Spawn(pp, msg.owner.gameObject);
        //NetworkServer.SendToAll(new PhaseSpawnMessage() { obj = pp, owner = msg.owner });
        //id.Send(new PhaseSpawnOwnerMessage() { obj = pp, owner = msg.owner });
    }

    virtual protected void OnRPCStartPhase(PhaseSpawnMessage msg)
    {
        msg.obj.transform.SetParent(msg.owner.transform);
        msg.obj.transform.localPosition = Vector3.zero;
        msg.obj.transform.localRotation = Quaternion.identity;
    }

    [Server]
    virtual protected void OnEndPhase(NetworkConnection id, PhaseEndMessage msg)
    {
        if (id != msg.owner.GetComponent<NetworkIdentity>().connectionToClient)
            return;
        ProtoPlayerMP plr = msg.owner.GetComponent<ProtoPlayerMP>();
        plr.moveSpeed /= msg.speedMult;
        plr.moveSpeedAir /= msg.speedMult;
        if (msg.p == null)
            return;
        NetworkServer.UnSpawn(msg.p);
        Destroy(msg.p.gameObject);
    }

    public override bool Cast(Character character, GameObject target, Vector3 dir)
    {
        if (!base.Cast(character, target, dir))
            return false;
        //character.uAnimator.SetTrigger("dash");
        //character.freeze = true;
        t = 0;
        _phasing = true;
        StartPhase(character);
        return true;
    }
    virtual protected void StartPhase(Character character)
    {
        _using = true;
        ProtoPlayerMP mp = character.GetComponentInChildren<ProtoPlayerMP>();
        mp.ShowPlayerModel(false);
        if (_invunerablePhase)
            character.invunerable = true;
        PlayerScript plr = character.GetComponent<PlayerScript>();
        PhaseMessage msg = new PhaseMessage { owner = character.gameObject, prefab = "" };
        msg.speedMult = _speedMult;
        NetworkClient.Send(msg);
        if (_particles==null)
            return;
        SpawnerMP.instance.SpawnPrefab(_particles.name, character.transform.position, character.transform.rotation,
            character.GetComponent<NetworkIdentity>(), (x) => OnSpawnedPhase(x), mp.netIdentity);
        //ProyectileFactory.RequestSpawnGameObject(_particles, character.gameObject, character.transform.position, character.transform.rotation);
    }

    void OnSpawnedPhase(NetworkIdentity ni)
    {
        _p = ni.gameObject;
        _p.transform.localPosition = Vector3.zero;
    }

    virtual protected void EndPhase(Character character)
    {
        _using = false;
        if (_invunerablePhase)
            character.invunerable = false;
        character.GetComponentInChildren<ProtoPlayerMP>().ShowPlayerModel(true);
        PhaseEndMessage msg = new PhaseEndMessage { owner = character.gameObject, p = _p };
        msg.speedMult = _speedMult;
        NetworkClient.Send(msg);
        t = 0;
        _phasing = false;
        _p = null;
        //ProyectileFactory.RequestUnSpawn(_p);
    }

    public override bool SkillUpdate(Character character, float deltaTime)
    {
        if (!base.SkillUpdate(character, deltaTime))
            return false;
        if (_phasing)
        {
            t += deltaTime;
        }
        if (t >= duration)
        {
            EndPhase(character);
        }
        return true;
    }
}
