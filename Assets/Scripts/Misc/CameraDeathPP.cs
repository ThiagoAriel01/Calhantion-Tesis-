using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraDeathPP : MonoBehaviour
{
    [SerializeField] protected PostProcessVolume _volume;
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
        _volume.weight = 1.0f;
    }
    void OnRespawn(ProtoPlayerMP p)
    {
        _volume.weight = 0.0f;
    }
}