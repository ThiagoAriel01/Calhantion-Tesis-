using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "Data/Ghost Summon Skill")]
public class GhostSummonSkill : SkillData, INetworkHandler
{
    [SerializeField] protected string _ghostType;

    public struct SpawnGhost : NetworkMessage
    {
        public Vector3 atPos;
        public Quaternion atRot;
        public GameObject owner;
        public string type;
    }

    public struct EndGhost : NetworkMessage
    {
        public GameObject ghost;
        public float cool;
    }

    public struct EndGhost2 : NetworkMessage
    {
        public GameObject ghost;
        public float cool;
    }

    private void Awake()
    {
        (this as INetworkHandler).RegisterHandlers();
    }

    void INetworkHandler.RegisterHandlers()
    {
        ServerInit();
        ClientInit();
    }

    public override void Init()
    {
        base.Init();
    }

    void ServerInit()
    {
        NetworkServer.RegisterHandler<SpawnGhost>(SummonGhost);
        NetworkServer.RegisterHandler<EndGhost2>(CancelGhost);
    }
    void ClientInit()
    {
        NetworkClient.RegisterHandler<EndGhost>(OnEndGhost);
    }

    void OnEndGhost(EndGhost msg)
    {
        currentCooldown = cooldown;
    }

    void CancelGhost (NetworkConnection conn, EndGhost2 msg)
    {
        msg.ghost.GetComponent<Ghost>().Kill();
    }

    [Server]
    void SummonGhost(NetworkConnection conn, SpawnGhost msg)
    {
        if (conn != msg.owner.GetComponent<NetworkIdentity>().connectionToClient)
            return;
        Ghost g = Instantiate(Resources.Load<Ghost>("Prefabs/" + msg.type), msg.atPos, msg.atRot);
        g.Init(msg.owner);
        NetworkServer.Spawn(g.gameObject);
        msg.owner.GetComponent<PlayerState>().currentGhost = g.gameObject;
        g.onDestroy += (x) => OnGhostDestroy(conn,x);
    }

    [Server]
    void OnGhostDestroy(NetworkConnection conn, Ghost g)
    {
        //currentCooldown = cooldown;
        if (conn != g.owner.GetComponent<NetworkIdentity>().connectionToClient)
            return;
        EndGhost msg = new EndGhost { ghost = g.gameObject };
        conn.Send(msg);
    }

    public override bool CanCast(Character character)
    {
        if (character.GetComponent<PlayerState>().currentGhost != null)
            return true;

        if (!base.CanCast(character))
            return false;

        return true;
    }

    public override bool Cast(Character character, GameObject target, Vector3 direction)
    {
        /*PlayerState plrState = character.GetComponent<PlayerState>();
        if (plrState != null)
        {
            if (plrState.currentGhost != null) {
                Ghost g = plrState.currentGhost.GetComponent<Ghost>();
                if (g != null)
                {
                    NetworkClient.Send(new EndGhost2() { ghost = g.gameObject });
                    currentCooldown = cooldown;
                    return false;
                }
            }
        }*/

        if (!base.Cast(character, target, direction))
            return false;

        StartSummon(character);
        return true;
    }

    public override bool ForceCancel(Character character)
    {
        PlayerState plrState = character.GetComponent<PlayerState>();
        if (plrState != null)
        {
            if (plrState.currentGhost != null)
            {
                EndGhost2 sg = new EndGhost2() { ghost = plrState.currentGhost.gameObject };
                CancelGhost(NetworkClient.connection, sg);
                currentCooldown = cooldown;
                return true;
            }
        }
        return false;
    }

    public override bool SkillUpdate(Character character, float deltaTime)
    {
        if (!base.SkillUpdate(character, deltaTime))
            return false;

        try
        {
            currentCooldown = character.GetComponent<PlayerState>().currentGhost.GetComponent<Ghost>().remainingTime;
        }
        catch
        {

        }

        return true;
    }

    virtual protected void StartSummon(Character character)
    {
        SpawnGhost msg = new SpawnGhost() { 
            atPos = character.transform.position, 
            atRot = character.transform.rotation, 
            owner = character.gameObject,
            type = _ghostType};
        NetworkClient.Send(msg);
    }
}