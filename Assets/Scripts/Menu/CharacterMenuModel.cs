using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterMenuModel : MonoBehaviour
{
    private void Awake()
    {
        Destroy(GetComponent<NetworkAnimator>());
        Destroy(GetComponent<NetworkIdentity>());
        GetComponent<Animator>().SetFloat("speedMult", 1f);
        GetComponent<Animator>().SetBool("isGrounded", true);
    }
}
