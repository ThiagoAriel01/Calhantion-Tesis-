using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoSingleton<MusicManager>
{
    protected float _blendTime = 0.0f;
    protected MusicSource _current;
    protected MusicSource _previous;

    protected class MusicSource
    {
        public AudioSource source;
        protected MusicCue _cue;
        public MusicCue cue
        {
            get
            {
                return _cue;
            }
            set
            {
                _cue = value;
                if (_cue == null)
                    source.clip = null;
                else
                    source.clip = value._clip;
            }
        }
        public float volume
        {
            get
            {
                return source.volume;
            }
            set
            {
                source.volume = value;
            }
        }
    }

    override protected bool Awake()
    {
        if (!base.Awake())
            return false;
        _current = new MusicSource()
        {
            source = new GameObject("CurrentMusic").AddComponent<AudioSource>()
        };
        _previous = new MusicSource()
        {
            source = new GameObject("PreviousMusic").AddComponent<AudioSource>()
        };
        _current.source.loop = true;
        _previous.source.loop = true;
        _current.source.transform.SetParent(transform);
        _previous.source.transform.SetParent(transform);
        return true;
    }

    public bool PlayMusic (MusicCue cue, float blendTime = 2.0f)
    {
        if (_current.cue == cue)
            return false;

        _blendTime = blendTime;

        MusicCue ccue = _current.cue;
        float cvol = _current.volume;
        float ctime = _current.source.time;
        _previous.cue = ccue;
        _previous.volume = cvol;
        _previous.source.Play();
        _previous.source.time = ctime;

        _current.cue = cue;
        _current.volume = .0f;
        _current.source.Play();
        _current.source.time = 0.0f;

        return true;
    }

    private void Update()
    {
        _current.volume = Mathf.MoveTowards(_current.volume, 0.15f, Time.deltaTime / _blendTime);
        _previous.volume = Mathf.MoveTowards(_previous.volume, 0f, Time.deltaTime / _blendTime);
    }
}
