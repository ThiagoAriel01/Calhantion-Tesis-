using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "New AudioCue", menuName = "Data/AudioCue", order = 0)]
public class AudioCue : ScriptableObject
{
    [SerializeField] private AudioClip[] clips=null;
    [SerializeField] private float minPitch = 1.0f;
    [SerializeField] private float maxPitch = 1.0f;
    [SerializeField] private float minVolume = 1.0f;
    [SerializeField] private float maxVolume = 1.0f;
    [SerializeField] private float range = 100f;

    public AudioClip[] Clips { get { return clips; } }
    public float MinPitch { get { return minPitch; } }
    public float MaxPitch { get { return maxPitch; } }
    public float MinVolume { get { return minVolume; } }
    public float MaxVolume { get { return maxVolume; } }
    public float DefaultRange { get { return range; } }

    public AudioParams GetParams()
    {
        AudioParams ap = new AudioParams()
        {
            audiocue = this.name,
            volume = Random.Range(minVolume, maxVolume),
            pitch = Random.Range(minPitch, maxPitch),
            range = range,
            clipIndex = Random.Range(0, clips.Length),
            owner = null
        };
        return ap;
    }

    [Client]
    public void PlaySound2DLocal()
    {
        AudioParams ap = GetParams();
        TesisAudioManager.instance.PlaySound(ap);
    }


    public void PlaySound(Vector3 pos, NetworkIdentity owner = null)
    {
        AudioParams ap = GetParams();
        if (owner != null)
        {
            if (owner.isServer)
                TesisAudioManager.instance.ServerPlaySound(ap, pos);
            else
                TesisAudioManager.instance.ClientPlaySound(ap, pos);
        }
        else
        {
            TesisAudioManager.instance.PlaySound(ap, pos);
        }
    }

    [Server]
    public void PlaySound()
    {
        AudioParams ap = GetParams();
        TesisAudioManager.instance.ServerPlaySound(ap);
    }
}