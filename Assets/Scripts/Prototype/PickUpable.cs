using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class PickUpable : NetworkBehaviour
{
    [SerializeField] protected AudioCue _cue;
    [SerializeField] protected float range = 10f;
    [SerializeField] protected float catchSpeed = 30f;
    [SerializeField] protected float curveDuration = 1f;
    [SerializeField] protected AnimationCurve curve;
    [SerializeField] protected float respawnTime = 30f;
    [SyncVar(hook = nameof(OnChangePickuped))]protected bool pickuped;
    protected PlayerScript curPlr;
    protected PlayerScript inside;
    protected float t;

    private void Start()
    {
    }

    void OnChangePickuped(bool oldv, bool newv)
    {
        gameObject.SetActive(!newv);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        enabled = false;
        OnChangePickuped(false, pickuped);
    }

    virtual protected void OnEnter(PlayerScript plr)
    {
        if (curPlr != null)
            return;
        curPlr = plr;
        t = 0.0f;
    }

    [Server]
    virtual protected void OnPickedUp(PlayerScript plr)
    {
        if (_cue!=null)
            _cue.PlaySound(transform.position,netIdentity);
        Invoke("Respawn",respawnTime);
        pickuped = true;
        curPlr = plr;
        t = 0.0f;
    }

    void Respawn()
    {
        pickuped = false;
    }

    virtual protected bool NeedsPickup (PlayerScript plr)
    {
        return true;
    }

    [Server]
    virtual protected void OnTriggerEnter(Collider other)
    {
        if (pickuped)
            return;

        PlayerScript plrs = other.GetComponent<PlayerScript>();
        if (plrs != null && NeedsPickup(plrs))
            OnPickedUp(plrs);
    }

    void CancelPickUp()
    {
        curPlr = null;
    }

}