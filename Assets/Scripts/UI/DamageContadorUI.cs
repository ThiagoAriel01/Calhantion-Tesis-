using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageContadorUI : MonoBehaviour
{
    [SerializeField] protected Text _text;
    protected PlayerState plrState;

    public void SetTarget (PlayerState state)
    {
        plrState = state;
        plrState.onDamagedChanged += (x) => { _text.text = kFormatter(plrState.damage); };
    }

    public static string kFormatter(float val)
    {
        return val < 1000 ? ((int)val).ToString() : (val / 1000).ToString("F1") + "K";
    }
}