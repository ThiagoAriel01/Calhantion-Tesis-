using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotator : MonoBehaviour
{
    [SerializeField] protected Vector3 _localRot;

    private void Update()
    {
        transform.localRotation *= Quaternion.Euler(_localRot * Time.deltaTime);
    }
}
