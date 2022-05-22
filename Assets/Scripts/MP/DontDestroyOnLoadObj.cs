using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadObj : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
