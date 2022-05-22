using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaUI : MonoBehaviour
{
    [SerializeField] protected PlayerScript manaTarget;
    [SerializeField] protected Image manaBar;
    [SerializeField] protected Text manaNumber;

    public void SetTarget (PlayerScript t)
    {
        manaTarget = t;
    }
    private void Update()
    {
        manaBar.fillAmount = manaTarget.Mana / manaTarget.ManaMax;
        manaNumber.text = ((int)manaTarget.Mana).ToString();
    }
}
