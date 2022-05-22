using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Linq;

public class ScoreBoardUI : MonoBehaviour
{
    [SerializeField] protected GameObject _base;
    [SerializeField] protected ScoreElementUI _elementUI;
    [SerializeField] protected TeamElementUI _teamElementUI;
    [SerializeField] protected RectTransform _panel;
    protected List<ScoreElementUI> _elements = new List<ScoreElementUI>();
    protected List<TeamElementUI> _teams = new List<TeamElementUI>();

    protected bool _waiting=true;

    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        //Sort();
    }

    void Init()
    {
        foreach (PlayerState plr in PlayerState.allPlayers)
        {
            AddPlayer(plr);
        }
        PlayerState.onPlayerAdded += OnPlayerAdded;
        PlayerState.onPlayerRemoved += OnPlayerRemoved;
    }

    private void OnDestroy()
    {
        PlayerState.onPlayerAdded -= OnPlayerAdded;
        PlayerState.onPlayerRemoved -= OnPlayerRemoved;
    }

    void OnPlayerAdded(PlayerState state)
    {
        AddPlayer(state);
    }

    void OnPlayerRemoved(PlayerState state)
    {
        RemovePlayer(state);
    }

    public void Sort()
    {
        try
        {
            _elements = _elements.OrderBy(c => c.target.teamIndex).ThenBy(n => n.target.kills).ThenBy(p => p.target.GetComponent<PlayerNET>().nickName).ToList();
            for (int i = _elements.Count - 1; i >= 0; i--)
            {
                _elements[i].transform.SetAsLastSibling();
            }
        }
        catch
        {

        }
    }

    void AddPlayer(PlayerState state)
    {
        if (HasPlayer(state))
            return;
        ScoreElementUI element = Instantiate(_elementUI, _panel.transform);
        element.Init(state);
        _elements.Add(element);
    }

    void RemovePlayer (PlayerState state)
    {
        ScoreElementUI ui = GetPlayerUI(state);
        Destroy(ui.gameObject);
    }

    bool HasPlayer(PlayerState state)
    {
        foreach (ScoreElementUI item in _elements)
        {
            if (item.target == state)
                return true;
        }
        return false;
    }

    ScoreElementUI GetPlayerUI(PlayerState state)
    {
        foreach (ScoreElementUI item in _elements)
        {
            if (item.target == state)
                return item;
        }
        return null;
    }

    void OnConnected()
    {

    }

    private void Update()
    {
        bool activate = Input.GetKey(KeyCode.Tab);
        if (_base.gameObject.activeSelf != activate)
        {
            _base.gameObject.SetActive(activate);
            if (activate)
                Sort();
        }
    }

    private void FixedUpdate()
    {
        UpdateScores();
    }

    void UpdateScores()
    {
        List<PlayerState> players = PlayerState.allPlayers;
    }
}
