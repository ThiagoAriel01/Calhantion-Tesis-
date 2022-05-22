using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System.Threading.Tasks;
using System.Threading;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;

public class PlayerHeadUI : NetworkBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private TextMeshPro _hp;
    [SerializeField] private GameObject _hpBar;
    protected float _defaultHPSize;
    protected PlayerState _state;

    [SyncVar(hook = nameof(OnShowChanged))]
    protected bool _showing=true;

    [SyncVar(hook = nameof(OnTextChanged))]
    protected string _textValue;

    [SyncVar(hook = nameof(OnTeamIndexChanged))]
    protected int _teamIndex = -1;

    protected PlayerNET _target;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        _state = GetComponent<PlayerState>();
        SetTarget(GetComponent<PlayerNET>(),_state.teamIndex);
        _panel.gameObject.SetActive(false);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        HealthComponent hc = GetComponent<HealthComponent>();
        OnHealthChanged(hc, hc.Health, hc.Health);
        GetComponent<HealthComponent>().onHealthChange += OnHealthChanged;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _state.onServerTeamChanged += OnTeamServerChanged;
        GetComponent<HealthComponent>().onHealthChange += OnHealthChanged;
    }

    void OnShowChanged (bool oldv, bool newv)
    {
        if (isLocalPlayer)
        {
            _panel.gameObject.SetActive(false);
        }
        else
        {
            _panel.gameObject.SetActive(newv);
        }
    }

    public void Show()
    {
        CmdChangeShow(true);
    }

    public void Hide()
    {
        CmdChangeShow(false);
    }

    [Command]
    void CmdChangeShow (bool newv)
    {
        _showing = newv;
    }

    void OnHealthChanged (HealthComponent hc, float oldvalue, float newValue)
    {
        _hpBar.transform.localScale = new Vector3(_defaultHPSize * newValue / hc.HealthMax, _hpBar.transform.localScale.y, 1f);
        _hp.text = ((int)newValue).ToString();
    }

    private void Awake()
    {
        _state = GetComponent<PlayerState>();
        _text.gameObject.SetActive(true);
        _defaultHPSize = _hpBar.transform.localScale.x;
    }

    void OnTextChanged(string oldValue, string newValue)
    {
        _text.text = newValue;
    }

    public void SetTarget(PlayerNET p_plr, int teamindex)
    {
        _target = p_plr;
        CmdSetText(gameObject, teamindex);
    }

    void OnTeamServerChanged (int newteam)
    {
        RpcSetText(_textValue, newteam);
    }

    void OnTeamIndexChanged(int oldindex, int teamindex)
    {
        SetTextTeam(teamindex);
    }

    [Command]
    void CmdSetText(GameObject _obj, int teamindex)
    {
        PlayerNET pnet = _obj.GetComponent<PlayerNET>();
        string nickName = pnet.nickName;
        _textValue = nickName;
        RpcSetText(nickName, teamindex);
    }

    private void FixedUpdate()
    {
        if (!isServer)
            return;
        _teamIndex = _state.teamIndex;
    }

    [ClientRpc]
    public void RpcSetText(string r_text, int teamindex)
    {
        _text.text = r_text;
        SetTextTeam(teamindex);
    }

    void SetTextTeam(int teamindex)
    {
        if (!GameModeNetworkState.instance.HasTeams())
            return;
        if (teamindex < 0)
            return;
        TeamDeathmatch.Team tm = GameModeNetworkState.instance.teams[teamindex];
        _text.color = tm._color;
        _hpBar.GetComponent<SpriteRenderer>().color = tm._color;
    }
}
