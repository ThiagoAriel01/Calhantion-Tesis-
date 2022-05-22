using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMessageUI : MonoBehaviour
{
    [SerializeField] protected GameObject _cartel;
    protected bool inited;

    private void Update()
    {
        if (ProtoPlayerMP.local == null)
        {
            return;
        }
        if (!inited)
        {
            ProtoPlayerMP.local.onDeath += OnDeath;
            ProtoPlayerMP.local.onRespawn += OnRespawn;
            inited = true;
            enabled = false;
        }
    }

    void OnDeath(ProtoPlayerMP p)
    {
        _cartel.gameObject.SetActive(true);
    }
    void OnRespawn(ProtoPlayerMP p)
    {
        _cartel.gameObject.SetActive(false);
    }
}
