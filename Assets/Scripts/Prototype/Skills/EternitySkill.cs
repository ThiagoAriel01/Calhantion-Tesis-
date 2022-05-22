using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "Data/Skills/Eternity")]
public class EternitySkill : SkillData, INetworkHandler
{
    [SerializeField] protected AudioCue _cue;
    [SerializeField] protected float _duration;
    [SerializeField] protected GameObject _fxPrefab;
    [SerializeField] protected Material _materialResource;
    protected bool _working;
    protected float _t = 0.0f;

    protected struct StartEternityMessage : NetworkMessage
    {
        public GameObject owner;
        public string fx;
        public string sharedMat;
    }

    protected struct EndEternityMessage : NetworkMessage
    {
        public GameObject owner;
    }

    private void Awake()
    {
        (this as INetworkHandler).RegisterHandlers();
    }

    void INetworkHandler.RegisterHandlers()
    {
        NetworkServer.RegisterHandler<StartEternityMessage>(StartEternity);
        NetworkServer.RegisterHandler<EndEternityMessage>(EndEternity);
    }


    [Server]
    void StartEternity(NetworkConnection conn, StartEternityMessage msg)
    {
        //GameObject fxobj = Instantiate(Resources.Load<GameObject>("Prefabs/" + msg.fx), msg.owner.transform);
        msg.owner.GetComponent<ProtoPlayerMP>().ChangeMaterial(msg.sharedMat);
    }

    [Server]
    void EndEternity(NetworkConnection conn, EndEternityMessage msg)
    {
        msg.owner.GetComponent<ProtoPlayerMP>().ResetMaterial();
    }

    public override bool Cast(Character character, GameObject target, Vector3 direction)
    {
        if (!base.Cast(character,target,direction))
            return false;
        character.invunerable = true;
        _working = true;
        _t = 0.0f;
        if (_cue != null)
            _cue.PlaySound(character.transform.position, character.GetComponent<NetworkIdentity>());
        StartEternityMessage msg = new StartEternityMessage
        {
            owner = character.gameObject,
            sharedMat = _materialResource.name
        };
        NetworkClient.Send(msg);

        return true;
    }

    virtual protected void EndCast(Character character)
    {
        _t = 0;
        _working = false;
        EndEternityMessage msg = new EndEternityMessage
        {
            owner = character.gameObject
        };
        NetworkClient.Send(msg);
        _working = false;
        character.invunerable = false;
    }

    public override bool ForceCancel(Character character)
    {
        if (_working)
            EndCast(character);
        return base.ForceCancel(character);
    }

    public override bool SkillUpdate(Character character, float deltaTime)
    {
        base.SkillUpdate(character, deltaTime);
        if (_working)
        {
            _t += deltaTime;
        }
        if (_t >= _duration)
        {
            EndCast(character);
        }
        return true;
    }

}
