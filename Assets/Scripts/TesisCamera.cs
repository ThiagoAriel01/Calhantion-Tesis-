using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesisCamera : MonoBehaviour
{
    static protected Camera _cam;

    static public Camera cam
    {
        get
        {
            return _cam;
        }
    }

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }
}
