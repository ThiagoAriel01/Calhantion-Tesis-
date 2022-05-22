using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TesisAudioManager : MonoSingleton<TesisAudioManager>, INetworkHandler
{
    [SerializeField] protected bool mute;
    [SerializeField] protected int _maxSources = 64;
    protected AudioSource[] _sources;
    static protected Vector3 _graveyard = Vector3.one * -9999f;

    private void Update()
    {
        AudioListener.pause = TesisCamera.cam == null || !Application.isFocused;
    }

    public struct PlayAudioMsg : NetworkMessage
    {
        public AudioParams parameters;
        public Vector3 atPos;
    }

    void INetworkHandler.RegisterHandlers()
    {
        NetworkServer.RegisterHandler<PlayAudioMsg>(OnServerPlayAudio);
        NetworkClient.RegisterHandler<PlayAudioMsg>(OnClientPlayAudio);
    }

    [Client]
    void OnClientPlayAudio(PlayAudioMsg msg)
    {
        PlaySound(msg.parameters, msg.atPos);
    }

    [Server]
    void OnServerPlayAudio(NetworkConnection conn,PlayAudioMsg msg)
    {
        ServerPlaySound(msg.parameters, msg.atPos);
    }

    protected override bool Awake()
    {
        if (!base.Awake())
            return false;
        _sources = new AudioSource[_maxSources];
        for (int i = 0; i < _sources.Length; i++)
        {
            GameObject g = new GameObject("AudioSource " + i);
            AudioSource s = g.AddComponent<AudioSource>();
            s.rolloffMode = AudioRolloffMode.Linear;
            s.spatialBlend = 1f;
            DontDestroyOnLoad(g);
            _sources[i] = s;
        }
        (this as INetworkHandler).RegisterHandlers();
        return true;
    }

    [Server]
    public void ServerPlaySound(AudioParams param, Vector3 atPos)
    {
        if (mute)
            return;
        PlayAudioMsg msg = new PlayAudioMsg
        {
            parameters = param,
            atPos = atPos
        };
        NetworkServer.SendToAll(msg);
    }

    [Server]
    public void ServerPlaySound(AudioParams param)
    {
        if (mute)
            return;
        PlayAudioMsg msg = new PlayAudioMsg
        {
            parameters = param,
            atPos = _graveyard,
        };
        NetworkServer.SendToAll(msg);
    }

    [Client]
    public void ClientPlaySound(AudioParams param, Vector3 atPos)
    {
        if (mute)
            return;
        PlayAudioMsg msg = new PlayAudioMsg
        {
            parameters = param,
            atPos = atPos
        };
        NetworkClient.Send(msg);
    }

    public void PlaySound (AudioParams param)
    {
        PlaySound(param, _graveyard);
    }

    public void PlaySound(AudioParams param, Vector3 atPos)
    {
        if (mute)
            return;
        AudioSource ass = GetAudioSource();
        if (ass == null)
            return;
        AudioCue cue = Resources.Load<AudioCue>("AudioCue/" + param.audiocue);
        if (cue == null)
            return;
        if (param.clipIndex >= cue.Clips.Length)
            param.clipIndex = 0;
        ass.clip = cue.Clips[param.clipIndex];
        ass.volume = param.volume;
        ass.pitch = param.pitch;
        ass.maxDistance = param.range;
        if (atPos== _graveyard)
        {
            ass.spatialBlend = 0f;
            ass.transform.position = Vector3.zero;
        }
        else
        {
            ass.spatialBlend = 1f;
            ass.transform.position = atPos;
        }
        //if (param.owner != null)
        //    ass.transform.SetParent(param.owner.transform);
        ass.Play();
    }

    public AudioSource GetAudioSource()
    {
        foreach (AudioSource item in _sources)
        {
            if (item!=null && !item.isPlaying)
            {
                return item;
            }
        }
        return null;
    }

    public void SetVolume(float v)
    {
        AudioListener.volume = v;
    }

    public float GetVolume()
    {
        return AudioListener.volume;
    }
}

[System.Serializable]
public struct AudioParams
{
    [SerializeField] public string audiocue;
    [SerializeField] public float range;
    [SerializeField] public float volume;
    [SerializeField] public float pitch;
    [SerializeField] public int clipIndex;
    [SerializeField] public GameObject owner;
}