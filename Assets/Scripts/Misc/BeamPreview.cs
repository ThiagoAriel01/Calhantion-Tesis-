using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BeamPreview : NetworkBehaviour
{
    protected bool assigned;

    private void Update()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        if (transform.parent.name == "proyectilePoint")
            return;
        Transform t = transform.parent.Find("proyectilePoint");
        if (t == null)
            return;
        transform.SetParent(t);
        assigned = true;
    }
}
