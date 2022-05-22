using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LavaPlatform : NetworkBehaviour
{
    [SerializeField] protected AudioCue _cue;
    [SerializeField] protected float _delay;
    [SerializeField] protected float _respawnTime;
    [SerializeField] protected Collider _col;
    [SerializeField] protected Renderer _r;
    [SerializeField] protected GameObject _breakParent;
    protected LavaPlatformDebris[] _debris;
    [SyncVar(hook = nameof(OnBroke))] protected bool _isBroken;

    protected class LavaPlatformDebris
    {
        public Rigidbody obj;
        public Vector3 defPos;
        public Quaternion defRot;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;
        if (!other.CompareTag("Player"))
            return;
        if (_isBroken)
            return;
        Invoke("BreakNow", _delay);
    }

    public override void OnStartServer()
    {
        _debris = new LavaPlatformDebris[_breakParent.transform.childCount];
        for (int i = 0; i < _debris.Length; i++)
        {
            _debris[i] = new LavaPlatformDebris();
            _debris[i].obj = _breakParent.transform.GetChild(i).GetComponent<Rigidbody>();
            _debris[i].defPos = _breakParent.transform.GetChild(i).transform.localPosition;
            _debris[i].defRot = _breakParent.transform.GetChild(i).transform.localRotation;
        }
    }

    void OnBroke (bool prevv, bool newv)
    {
        if (newv)
        {
            _breakParent.gameObject.SetActive(true);
            _r.enabled = false;
            _col.enabled = false;
        }
        else
        {
            _breakParent.gameObject.SetActive(false);
            _r.enabled = true;
            _col.enabled = true;
        }
    }

    [Server]
    void BreakNow()
    {
        _isBroken = true;
        if (_cue != null)
            _cue.PlaySound(transform.position, netIdentity);
        CancelInvoke();
        Invoke("Respawn", _respawnTime);
    }

    [Server]
    void Respawn()
    {
        CancelInvoke();
        _isBroken = false;
        for (int i = 0; i < _debris.Length; i++)
        {
            _debris[i].obj.transform.localPosition = _debris[i].defPos;
            _debris[i].obj.transform.localRotation = _debris[i].defRot;
        }
    }
}
