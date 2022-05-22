using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TeamScoresUI : MonoBehaviour
{
    [SerializeField] protected TeamScoreUI _baseUI;
    protected List<TeamScoreUI> _pointsUI;

    private void Awake()
    {
        _baseUI.gameObject.SetActive(false);
    }

    void CreateTeamsUI(SyncList<TeamDeathmatch.Team> teams)
    {
        _pointsUI = new List<TeamScoreUI>();
        for (int i = 0; i < teams.Count; i++)
        {
            TeamScoreUI cpui = Instantiate(_baseUI, transform);
            cpui.Refresh(teams[i]);
            cpui.gameObject.SetActive(true);
            _pointsUI.Add(cpui);
        }
    }

    private void FixedUpdate()
    {
        if ((_pointsUI == null) && GameModeNetworkState.instance.teams.Count > 0)
        {
            CreateTeamsUI(GameModeNetworkState.instance.teams);
        }
        if (_pointsUI != null)
        {
            for (int i = 0; i < GameModeNetworkState.instance.teams.Count; i++)
            {
                _pointsUI[i].Refresh(GameModeNetworkState.instance.teams[i]);
            }
        }
    }
}
