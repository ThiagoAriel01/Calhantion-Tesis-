using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TeamScoreUI : MonoBehaviour
{
    [SerializeField] protected Text _teamName;
    [SerializeField] protected Text _teamScore;

    public void Refresh(TeamDeathmatch.Team team)
    {
        _teamName.text = team._name;
        _teamName.color = team._color;
        _teamScore.text = team._score.ToString();
        _teamScore.color = Color.white;
    }
}
