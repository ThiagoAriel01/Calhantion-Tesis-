using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MenuManager : MonoSingleton<MenuManager>
{
    public delegate void ChangedGameModeD(GameMode newmode);
    public delegate void ChangedMapD(MapData newmap);
    public ChangedGameModeD onChangedGameMode;
    public ChangedMapD onChangedMap;
    [SerializeField] protected MapData[] _maps;
    [SerializeField] protected InputField _ip;
    protected int _selectedMapIndex;
    [SerializeField] protected GameObject _connectingMenu;

    public string selectedMap
    {
        get => _maps[_selectedMapIndex].levelName;
    }

    public int selectedMapIndex
    {
        get => _selectedMapIndex;
    }

    public MapData[] maps
    {
        get
        {
            return _maps;
        }
    }

    public void SelectGameMode (int i)
    {
        GameModeManager.instance.SelectGameMode(i);
        onChangedGameMode?.Invoke(GameModeManager.instance.currentGameMode);
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }

    public void OnStartHostPressed()
    {
        TesisNetworkManager.instancia.StartHost();
    }

    public void SelectMap(int id)
    {
        _selectedMapIndex = id;
        onChangedMap?.Invoke(_maps[id]);
    }

    int GetIndexOf(MapData d)
    {
        for (int i = 0; i < _maps.Length; i++)
        {
            if (_maps[i] == d)
                return i;
        }
        return -1;
    }

    public void SelectMap(MapData d)
    {
        int index= GetIndexOf(d);
        if (index < 0)
            return;
        _selectedMapIndex = index;
        onChangedMap?.Invoke(d);
    }

    public void OnConnectPressed()
    {
        TesisNetworkManager.instancia.networkAddress = _ip.text;
        TesisNetworkManager.instancia.StartClient();
    }


    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (_connectingMenu==null)
            return;
        if (NetworkClient.isConnecting)
        {
            _connectingMenu.gameObject.SetActive(true);
        }
        else
        {
            _connectingMenu.gameObject.SetActive(false);
        }
    }
}
