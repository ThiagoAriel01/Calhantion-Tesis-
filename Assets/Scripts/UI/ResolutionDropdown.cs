using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionDropdown : MonoBehaviour
{
    protected Dropdown _dropdown;

    private void Awake()
    {

        _dropdown = GetComponent<Dropdown>();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            options.Add(new Dropdown.OptionData(Screen.resolutions[i].ToString()));
        }
        _dropdown.options = options;
        _dropdown.value = 0;
        _dropdown.onValueChanged.AddListener((x) => OnValueChanged(x));
    }

    void OnValueChanged (int v)
    {
        TesisGameOptions.instance.ChangeResolution(Screen.resolutions[v]);
    }
}
