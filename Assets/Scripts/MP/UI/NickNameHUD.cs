using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NickNameHUD : MonoBehaviour
{
    [SerializeField] protected Text _text;
    protected PlayerNET _net;

    public void SetTarget(PlayerNET p_net)
    {
        _net = p_net;
        _net.onChangedNickName += Refresh;
    }

    void Refresh (PlayerNET plr)
    {
        _text.text = plr.nickName;
    }

}
