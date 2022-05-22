using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualityDropdown : MonoBehaviour
{
    protected Dropdown _dropdown;

    private void Awake()
    {

        _dropdown = GetComponent<Dropdown>();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            options.Add(new Dropdown.OptionData(QualitySettings.names[i]));
        }
        _dropdown.options = options;
        _dropdown.value = QualitySettings.GetQualityLevel();
        _dropdown.onValueChanged.AddListener((x) => OnValueChanged(x));
    }

    void OnValueChanged(int v)
    {
        QualitySettings.SetQualityLevel(v);
    }
}
