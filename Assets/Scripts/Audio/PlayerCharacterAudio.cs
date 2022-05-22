using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterAudio : MonoBehaviour
{

    [SerializeField] protected AudioCue _jump;
    [SerializeField] protected AudioCue _jumpAir;

    private void Awake()
    {
        PlayerScript sc = GetComponent<PlayerScript>();
        sc.onJump += OnJump;
        sc.onJumpAir += OnJumpAir;
    }

    void OnJump(PlayerScript plr)
    {
        //_jump.PlaySound(transform.position, gameObject);
    }

    void OnJumpAir(PlayerScript plr)
    {
        //_jumpAir.PlaySound(transform.position, gameObject);
    }
}
