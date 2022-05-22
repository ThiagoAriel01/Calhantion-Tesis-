using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicOnStart : MonoBehaviour
{
    [SerializeField] protected MusicCue _cue;

    private void Start()
    {
        MusicManager.instance.PlayMusic(_cue);
    }
}
