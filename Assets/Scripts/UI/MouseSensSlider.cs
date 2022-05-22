using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensSlider : MonoBehaviour
{
    [SerializeField] protected Slider _slider;
    [SerializeField] protected Text _text;

    private void Awake()
    {
        SetSens(GetSens());
    }

    public void SetSens(float v)
    {
        TesisGameOptions.instance.mouseSens = v;
        _slider.value = v;
        _text.text = v.ToString("0.00");
    }

    public float GetSens()
    {
        return TesisGameOptions.instance.mouseSens;
    }
}
