using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyManagerUI : MonoBehaviour
{
    [SerializeField] protected GameObject _lobbyPanel;
    [SerializeField] protected GameObject _mainPanel;
    [SerializeField] protected RectTransform _playersT;
    [SerializeField] protected LobbyPlayerUI _template;
    [SerializeField] protected GameObject _godsMenu;
    [SerializeField] protected GameObject _connecting;
    protected List<LobbyPlayerUI> _elements = new List<LobbyPlayerUI>();
    protected bool _onLobby;

    private void Awake()
    {
        _lobbyPanel.gameObject.SetActive(false);
        TesisNetworkManager.instancia.onLobbyConnect = OnLobbyConnect;
        TesisNetworkManager.instancia.onLobbyDisconnect = OnLobbyDisconnect;
        TesisNetworkManager.instancia.onClientChangedScene = ChangedScene;
        Init();
    }

    void ChangedScene()
    {
        if (_connecting!=null)
            _connecting.gameObject.SetActive(true);
    }

    void Init()
    {
        foreach (LobbyPlayerState plr in LobbyPlayerState.allStates)
        {
            AddPlayer(plr);
        }
        LobbyPlayerState.onPlayerAdded = OnPlayerAdded;
        LobbyPlayerState.onPlayerRemoved = OnPlayerRemoved;
    }

    void OnPlayerAdded(LobbyPlayerState state)
    {
        AddPlayer(state);
    }

    void OnPlayerRemoved(LobbyPlayerState state)
    {
        RemovePlayer(state);
    }

    void AddPlayer(LobbyPlayerState state)
    {
        if (HasPlayer(state))
            return;
        LobbyPlayerUI element = Instantiate(_template, _playersT);
        element.Init(state);
        _elements.Add(element);
    }

    void RemovePlayer(LobbyPlayerState state)
    {
        LobbyPlayerUI ui = GetPlayerUI(state);
        Destroy(ui.gameObject);
    }

    bool HasPlayer(LobbyPlayerState state)
    {
        foreach (LobbyPlayerUI item in _elements)
        {
            if (item.state == state)
                return true;
        }
        return false;
    }

    LobbyPlayerUI GetPlayerUI(LobbyPlayerState state)
    {
        foreach (LobbyPlayerUI item in _elements)
        {
            if (item.state == state)
                return item;
        }
        return null;
    }

    public void OnPressedReady()
    {
        if (LobbyPlayerState.localState != null)
        {
            LobbyPlayerState.localState.SetReady();
        }
    }

    private void FixedUpdate()
    {
        
    }

    void OnLobbyConnect()
    {
        _onLobby = true;
        ToggleLobby(true);
        _mainPanel.gameObject.SetActive(false);
    }

    public void ToggleLobby (bool v)
    {
        if (v && !_onLobby)
            v = false;
        _lobbyPanel.gameObject.SetActive(v);
        if (_onLobby)
            _godsMenu.SetActive(false);
    }

    void OnLobbyDisconnect()
    {
        _onLobby = false;
        ToggleLobby(false);
        _mainPanel.gameObject.SetActive(true);
    }
}