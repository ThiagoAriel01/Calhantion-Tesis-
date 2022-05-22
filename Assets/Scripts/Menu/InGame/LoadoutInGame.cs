using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutInGame : MonoBehaviour
{
    [SerializeField] public GameObject _loadoutPanel;
    [SerializeField] public GameObject _skillsPanel;
    [SerializeField] protected LoadoutUI _loadoutUI;
    protected bool _showingLoadout;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleLoadout();
        }
    }

    public void ToggleLoadout()
    {
        _showingLoadout = !_showingLoadout;
        _loadoutPanel.gameObject.SetActive(_showingLoadout);
        _skillsPanel.gameObject.SetActive(false);
        PlayerHUD.ToggleLockCursor(!_showingLoadout);
        _loadoutUI.Save();
    }
}
