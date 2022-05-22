using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutDoneButton : MonoBehaviour
{
    protected Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Update()
    {
        if (!isActiveAndEnabled)
            return;

        _button.interactable = LoadoutUI.instance.CanConfirm();
    }
}
