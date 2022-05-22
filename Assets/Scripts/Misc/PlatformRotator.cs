using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlatformRotator : NetworkBehaviour
{
    [SerializeField] protected float _rotateSpeed = 5f;

    void LateUpdate()
    {
        if (!isServer)
            return;
        transform.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime);
    }
}