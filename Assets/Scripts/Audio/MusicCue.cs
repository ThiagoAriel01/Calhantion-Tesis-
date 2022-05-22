using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Music Cue")]
public class MusicCue : ScriptableObject
{
    public AudioClip _clip;
    public string _displayName;
}