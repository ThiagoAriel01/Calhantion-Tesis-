using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnerMP : MonoSingleton<SpawnerMP>, INetworkHandler
{

    private List<PendingCall> pendingCalls = new List<PendingCall>();
    protected long count;

    public class PendingCall
    {
        public long id;
        public System.Action<NetworkIdentity> action;
    }

    public struct ParentMSG : NetworkMessage
    {
        public NetworkIdentity child;
        public NetworkIdentity parent;
    }

    public struct ParentCMSG : NetworkMessage
    {
        public NetworkIdentity child;
        public NetworkIdentity parent;
    }

    public struct FinishedMSG : NetworkMessage
    {
        public NetworkIdentity ni;
        public long spawnID;
    }

    public struct DestroyMSG : NetworkMessage
    {
        public NetworkIdentity ni;
    }

    public struct SpawnMSG : NetworkMessage
    {
        public string prefab;
        public Vector3 atPos;
        public Quaternion atRot;
        public NetworkIdentity parent;
        public NetworkIdentity owner;
        public long spawnID;
    }

    protected override bool Awake()
    {
        if (!base.Awake())
            return false;
        (this as INetworkHandler).RegisterHandlers();
        return true;
    }

    void INetworkHandler.RegisterHandlers()
    {
        NetworkServer.RegisterHandler<SpawnMSG>(OnSpawnMessage);
        NetworkServer.RegisterHandler<DestroyMSG>(OnDestroyMessage);
        NetworkServer.RegisterHandler<ParentMSG>(OnParentMessage);
        NetworkClient.RegisterHandler<FinishedMSG>(OnFinishMessage);
        NetworkClient.RegisterHandler<ParentCMSG>(OnParentClient);
    }

    [Server]
    void OnParentMessage(NetworkConnection conn, ParentMSG msg)
    {
        ParentCMSG cmsg = new ParentCMSG()
        {
            child = msg.child,
            parent = msg.parent
        };
        NetworkServer.SendToReady(cmsg);
    }

    [Client]
    void OnParentClient(ParentCMSG msg)
    {
        if (msg.child == null || msg.parent == null)
            return;

        msg.child.transform.SetParent(msg.parent.transform);
        msg.child.transform.localPosition = Vector3.zero;
    }

    [Client]
    void OnFinishMessage(FinishedMSG msg)
    {
        PendingCall call = pendingCalls.Find(x => x.id == msg.spawnID);
        if (call == null)
            return;
        pendingCalls.Remove(call);
        call.action.Invoke(msg.ni);
    }

    [Server]
    void OnSpawnMessage (NetworkConnection conn, SpawnMSG msg)
    {
        NetworkIdentity ni = Instantiate(Resources.Load<NetworkIdentity>("Prefabs/" + msg.prefab), msg.atPos, msg.atRot);
        NetworkServer.Spawn(ni.gameObject);

        if (msg.parent != null)
        {
            ni.transform.SetParent(msg.parent.transform);
            ni.transform.localPosition = Vector3.zero;
            ParentCMSG cmsg = new ParentCMSG()
            {
                child = ni,
                parent = msg.parent
            };
            NetworkServer.SendToReady(cmsg);
        }
        NetworkIdentity own = (msg.owner == null ? msg.parent : msg.owner);
        try
        {
            ni.SendMessage("SetOwner", (NetworkIdentity)own, SendMessageOptions.DontRequireReceiver);
        }
        catch
        {

        }

        ExplosionHardSet hard = ni.GetComponent<ExplosionHardSet>();
        if (hard != null)
            ni.GetComponent<ExplosionHardSet>().Init(msg.parent.gameObject);
        FinishedMSG mes = new FinishedMSG { ni = ni, spawnID = msg.spawnID };
        conn.Send(mes);
    }

    [Server]
    void OnDestroyMessage(NetworkConnection conn, DestroyMSG msg)
    {
        if (msg.ni == null)
            return;
        NetworkServer.UnSpawn(msg.ni.gameObject);
        Destroy(msg.ni.gameObject);
    }

    [Client]
    public void SpawnPrefab(string prefab, Vector3 atPos, Quaternion atRot, NetworkIdentity parent, System.Action<NetworkIdentity> onspawn, NetworkIdentity owner=null)
    {
        long newID = count++;
        SpawnMSG msg = new SpawnMSG
        {
            prefab = prefab,
            atPos = atPos,
            atRot = atRot,
            parent = parent,
            spawnID = newID,
            owner = owner
        };
        pendingCalls.Add(new PendingCall()
        {
            action = onspawn,
            id = newID
        });
        NetworkClient.Send(msg);
    }

    [Client]
    public void DestroyObject (NetworkIdentity ni)
    {
        DestroyMSG msg = new DestroyMSG { ni = ni };
        NetworkClient.Send(msg);
    }
}
