using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleTrigger : MonoBehaviour
{
    public UnityEvent onTrigger = new UnityEvent();
    private bool once;


    private void OnTriggerEnter(Collider other)
    {
        if (once)
            return;
        if (other.tag == "Player")
        {
            onTrigger?.Invoke();
            once = true;
        }
    }
}
