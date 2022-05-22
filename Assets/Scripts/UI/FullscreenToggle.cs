using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    protected Toggle _toggle;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = Screen.fullScreen;
        _toggle.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(bool val)
    {
        TesisGameOptions.instance.ChangeFullscreen(val);
    }
}