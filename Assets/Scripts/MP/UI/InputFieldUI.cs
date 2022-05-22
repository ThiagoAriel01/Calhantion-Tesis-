using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldUI : MonoBehaviour
{
    [SerializeField] protected TMP_InputField _inputfield;
    [SerializeField] protected string defaultInput = "Player";
    protected string _playerPrefs = "PlayerNickName";

    private void Awake()
    {
        _inputfield.text = PlayerPrefs.GetString(_playerPrefs, defaultInput);
        _inputfield.onValueChanged.AddListener((x) => SetValue(x));
    }

    void SetValue (string p_value)
    {
        PlayerPrefs.SetString(_playerPrefs, p_value);
    }

}
