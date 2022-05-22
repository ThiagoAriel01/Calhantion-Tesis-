using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBarUI : MonoBehaviour
{
    [SerializeField] protected Animator _text;
    private PlayerNET _net;

    private void FixedUpdate()
    {
        if (_net == null)
        {
            _net = PlayerNET.localInstance;
            _net.GetComponent<ProtoPlayerMP>().onTakeDamage += OnTakeDamage;
            return;
        }
    }

    void OnTakeDamage(Character character, GameObject attacker, SHitInfo info)
    {
        _text.Play("hit", 0, 0);
    }
}
