using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ExplosionPush : NetworkBehaviour
{
    [SerializeField] protected BuffData _buff;
    [SerializeField] protected float _force;
    [SerializeField] protected float _radius;
    [SerializeField] protected float _heightExtra;
    [SerializeField] protected LayerMask _layers;

    public override void OnStartServer()
    {
        base.OnStartServer();
        Push();
    }

    [Server]
    void Push()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, _radius, _layers);
        foreach (Collider col in cols)
        {
            if (col.CompareTag("Player"))
            {
                NetworkIdentity ni = col.GetComponent<NetworkIdentity>();
                if (ni != null)
                {
                    PlayerScript c = col.GetComponent<PlayerScript>();
                    if (c == null)
                        return;
                    if (!c.freeze)
                    {
                        Vector3 v = (c.transform.position - transform.position);
                        v.y += _heightExtra;
                        v.Normalize();
                        v *= _force;
                        Vector3 vec = c.velocity;
                        Vector3 ve = c.velocity + new Vector3(v.x, v.y, v.z);
                        c.GetComponent<ProtoPlayerMP>().TargetSetVelocity(ve);
                    }
                    if (_buff!=null)
                        c.AddBuff(_buff);
                }
            }
        }
    }
}
