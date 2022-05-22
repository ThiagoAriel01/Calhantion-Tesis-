using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] protected Slider _slider;
    [SerializeField] protected Text _text;

    private void Awake()
    {
        SetVolume(GetVolume());
    }

    public void SetVolume(float v)
    {
        TesisGameOptions.instance.globalVolume = v;
        _slider.value = v;
        _text.text = v.ToString("0.00");
    }

    public float GetVolume()
    {
        return TesisGameOptions.instance.globalVolume;
    }
}
