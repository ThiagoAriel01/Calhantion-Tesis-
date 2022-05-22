using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class TeamUI : MonoBehaviour
{
    [SerializeField] protected Text _text;
    protected PlayerState _state;
    protected bool fullfilled = false;

    private void Awake()
    {
        _text.text = "";
    }

    void OnTeamChanged (int newTeam)
    {

    }

    public void SetTarget(PlayerState p_net)
    {
        _state = p_net;
        p_net.onTeamChanged += (x) => Refresh();
        Refresh();
    }

    void Refresh()
    {
        if (_state == null || !GameModeNetworkState.instance.HasTeams())
            return;

        if (_state.teamIndex < 0)
            return;

        _text.color = GameModeNetworkState.instance.teams[_state.teamIndex]._color;
        _text.text = GameModeNetworkState.instance.teams[_state.teamIndex]._name;
    }
}
