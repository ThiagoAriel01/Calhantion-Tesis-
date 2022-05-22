using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadElementUI : MonoBehaviour
{
    [SerializeField] protected bool invert;
    [SerializeField] protected float maxDistance;
    [SerializeField] protected float minSize;
    [SerializeField] protected float maxSize;
    static private Camera _camera;


    void Update()
    {
        if (_camera == null)
            _camera = Camera.main;

        Vector3 v = _camera.transform.position - transform.position;
        float d = v.magnitude;
        d /= maxDistance;

        transform.localScale = Vector3.one * Mathf.Lerp(minSize, maxSize, d);


        transform.rotation = Quaternion.LookRotation(invert ? -v : v);
    }   
}
