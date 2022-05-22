using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesisGameOptions : MonoSingleton<TesisGameOptions>
{
    protected float _mouseSens = -1;
    protected float _globalVolume = -1;

    override protected bool Awake()
    {
        if (!base.Awake())
            return false;
        _mouseSens = mouseSens;
        _globalVolume = globalVolume;
        return true;
    }

    public float mouseSens
    {
        get
        {
            if (_mouseSens < 0f)
                _mouseSens = PlayerPrefs.GetFloat("mouseSens",1f);
            return _mouseSens;
        }
        set
        {
            _mouseSens = value;
            PlayerPrefs.SetFloat("mouseSens", value);
        }
    }

    public float globalVolume
    {
        get
        {
            if (_globalVolume < 0f)
                _globalVolume = PlayerPrefs.GetFloat("globalVolume", 1f);
            TesisAudioManager.instance.SetVolume(_globalVolume);
            return _globalVolume;
        }
        set
        {
            _globalVolume = value;
            TesisAudioManager.instance.SetVolume(value);
            PlayerPrefs.SetFloat("globalVolume", value);
        }
    }

    public void ChangeResolution (Resolution res)
    {
        Screen.SetResolution(res.width,res.height,Screen.fullScreen,res.refreshRate);
    }

    public void ChangeFullscreen (bool value)
    {
        Screen.fullScreen = value;
    }
}
