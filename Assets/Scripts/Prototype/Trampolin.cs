using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampolin : MonoBehaviour
{
    [SerializeField] protected Vector3 force;
    [SerializeField] protected float duration = 10f;
    [SerializeField] protected AudioCue _enterCue;
    [SerializeField] protected bool onlyOwner;
    protected PlayerScript plr;

    private void Start()
    {
        if (duration>0)
            Invoke("End", duration);
    }

    void End()
    {
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((onlyOwner && (GetComponentInParent<Explosion>().Owner != other.gameObject)))
            return;
        if (other.CompareTag("Player"))
        {
            PlayerScript plr = other.GetComponent<PlayerScript>();
            if (plr != null)
            {
                Vector3 vec = plr.velocity;
                plr.SetCurGravity(force.y);
                if (_enterCue!=null)
                    _enterCue.PlaySound(transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            plr = null;
        }
    }

}
