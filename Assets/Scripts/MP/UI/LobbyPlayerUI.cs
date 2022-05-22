using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerUI : MonoBehaviour
{
    [SerializeField] protected Text _nametext;
    protected LobbyPlayerState _state;

    public LobbyPlayerState state
    {
        get
        {
            return _state;
        }
    }

    public void Init(LobbyPlayerState s)
    {
        _state = s;
        _nametext.text = _state.nickname;
    }

    private void FixedUpdate()
    {
        _nametext.text = _state.nickname;
        _nametext.color = _state.ready ? Color.green : Color.white;
    }
}
