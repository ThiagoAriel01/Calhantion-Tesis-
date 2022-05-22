using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class MenuInGame : MonoBehaviour
{
    [SerializeField] protected GameObject _menuPanel;
    protected LoadoutInGame _loadout;
    protected bool _showingMenu;

    private void Awake()
    {
        _loadout = GetComponent<LoadoutInGame>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        _showingMenu = !_showingMenu;
        _menuPanel.gameObject.SetActive(_showingMenu);
        _loadout._loadoutPanel.gameObject.SetActive(false);
        _loadout._skillsPanel.gameObject.SetActive(false);
        PlayerHUD.ToggleLockCursor(!_showingMenu);
    }

    public void Disconnect()
    {
        try
        {
            try { TesisNetworkManager.instancia.StopHost(); } catch { };
            try { TesisNetworkManager.instancia.StopServer(); } catch { };
            try { TesisNetworkManager.instancia.StopClient(); } catch { };
            GameModeManager.instance.Clear();
            TesisNetworkManager.CreateHandlers();
        }
        catch
        {

        }
    }

    public void ExitGame()
    {
        Disconnect();
        Application.Quit();
    }
}
