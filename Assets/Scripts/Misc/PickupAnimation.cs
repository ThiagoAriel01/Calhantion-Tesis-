using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAnimation : MonoBehaviour
{
    [SerializeField] protected float _rot;
    [SerializeField] protected float _amp;
    [SerializeField] protected float _rate;


    private void FixedUpdate()
    {
        float y = Mathf.Sin(Time.time * _rate) * _amp + (_amp/2);
        Vector3 v;
        v.x = 0;
        v.y = y;
        v.z = 0;
        transform.localPosition = v;
        transform.Rotate(Vector3.up * _rot * Time.deltaTime);
    }
}
