using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreElementUI : MonoBehaviour
{
    [SerializeField] protected Text _character;
    [SerializeField] protected Text _name;
    [SerializeField] protected Text _kills;
    [SerializeField] protected Text _deaths;
    [SerializeField] protected Text _team;
    [SerializeField] protected Text _damage;
    [SerializeField] protected Image _icon;
    protected int kills;
    protected string team;
    protected PlayerState _target;

    public string nametext
    {
        get { return _name.text; }
    }
    public int Kills
    {
        get { return kills; }
    }
    public string Team
    {
        get { return team; }
    }

    public void Init(PlayerState target)
    {
        _target = target;
    }

    public void Init(string pname, int kills, string deaths, string team, Color teamColor, string dmg, Sprite icon)
    {
        _character.text = "";
        this.kills = kills;
        this.team = team;
        _name.text = pname;
        _kills.text = kills.ToString();
        _deaths.text = deaths;
        _team.text = team;
        _team.color = teamColor;
        _damage.text = dmg;
        _icon.sprite = icon;
    }

    public PlayerState target
    {
        get
        {
            return _target;
        }
    }

    private void FixedUpdate()
    {
        if (_target == null || !gameObject.activeInHierarchy)
            return;

        _character.text = "";
        _name.text = _target.GetComponent<PlayerNET>().nickName;
        _kills.text = _target.kills.ToString();
        _deaths.text = _target.deaths.ToString();
        if (!GameModeNetworkState.instance.HasTeams())
        {
            _team.text = "";
        }
        else
        {
            _team.text = GameModeNetworkState.instance.teams[_target.teamIndex]._name;
            _team.color = GameModeNetworkState.instance.teams[_target.teamIndex]._color;
        }

        _damage.text = DamageContadorUI.kFormatter(_target.damage);

        if (_icon.isActiveAndEnabled && _icon.sprite == null)
        {
            CharacterData d = LoadoutManager.instance.GetCharacterData(target.GetComponent<ProtoPlayerMP>().modelName);
            if (d != null)
                _icon.sprite = d.icon;
            else
                _icon.gameObject.SetActive(false);
        }
    }
}