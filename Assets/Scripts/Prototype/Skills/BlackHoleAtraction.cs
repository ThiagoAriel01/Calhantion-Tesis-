using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BlackHoleAtraction : NetworkBehaviour
{
    [SerializeField] protected float _delay;
    [SerializeField] protected float _attractionForce;
    [SerializeField] protected float _attractionForceMax;
    [SerializeField] protected float _duration;
    [SerializeField] protected float _lostRange;
    [SyncVar] protected GameObject _owner;
    protected float _t;
    protected bool _works;
    protected bool _friendly = false;
    protected PlayerScript _plrScr;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Invoke("StartWork", _delay);
    }

    public void SetOwner(GameObject powner)
    {
        this._owner = powner;
        //CheckForFriendly(powner, PlayerNET.localInstance.gameObject);
    }

    void StartWork()
    {
        _works = true;
        _t = 0.0f;
    }

    [Server]
    void CheckForFriendly(GameObject attacker, GameObject victim)
    {
        bool isfriendly = !GameModeManager.instance.currentGameMode.CanDamagePlayer(
            attacker.GetComponent<PlayerState>(), victim.GetComponent<PlayerState>());
        TargetSetFriendly(victim.GetComponent<NetworkIdentity>().connectionToClient, isfriendly);
    }

    [TargetRpc]
    void TargetSetFriendly(NetworkConnection target,bool f)
    {
        _friendly = f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_works || _owner==null || _friendly)
            return;
        PlayerScript p = other.gameObject.GetComponent<PlayerScript>();
        if (p == null || PlayerNET.localInstance.gameObject != p.gameObject || p.gameObject == _owner.gameObject)
            return;

        _plrScr = p;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_works || _owner == null || _friendly)
            return;
        PlayerScript p = other.gameObject.GetComponent<PlayerScript>();
        if (p == null || PlayerNET.localInstance.gameObject != p.gameObject)
            return;

        _plrScr = p;
    }

    private void Update()
    {
        if (!_works || _owner == null || _friendly)
            return;
        _t += Time.deltaTime / _duration;
        if (_t >= 1f)
        {
            _works = false;
            return;
        }
        if (_plrScr != null)
        {
            if ((_plrScr.transform.position - transform.position).magnitude > _lostRange || _plrScr.gameObject == _owner.gameObject)
            {
                _plrScr = null;
                return;
            }
            Vector3 m = ((transform.position + Vector3.up * 1.5f) - _plrScr.transform.position).normalized * Mathf.Lerp(_attractionForce, _attractionForceMax, _t);
            _plrScr.AddDelta(m * Time.deltaTime, Quaternion.identity, Vector3.zero);
        }
    }
}