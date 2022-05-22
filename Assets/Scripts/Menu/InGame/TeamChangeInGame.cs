using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamChangeInGame : MonoBehaviour
{
    [SerializeField] protected Button _basebutton;
    [SerializeField] protected GameObject _panelButtons;
    [SerializeField] protected GameObject _panel;
    protected bool _showing;
    protected bool _instanced;

    private void Awake()
    {
        _basebutton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleChange();
        }
    }

    private void FixedUpdate()
    {
        if (_instanced)
            return;
        if (GameModeNetworkState.instance.HasTeams())
        {
            for (int i = 0; i < GameModeNetworkState.instance.teams.Count; i++)
            {
                Button b = Instantiate(_basebutton, _panelButtons.transform);
                int index = i;
                b.onClick.AddListener(() => ChangeTeam(index));
                TextMeshProUGUI tmpro = b.GetComponentInChildren<TextMeshProUGUI>();
                tmpro.text = GameModeNetworkState.instance.teams[index]._name;
                tmpro.color = GameModeNetworkState.instance.teams[index]._color;
                b.gameObject.SetActive(true);
            }
            _instanced = true;
        }
    }

    public void ToggleChange()
    {
        if (!GameModeNetworkState.instance.HasTeams())
            return;
        _showing = !_showing;
        _panel.gameObject.SetActive(_showing);
        PlayerHUD.ToggleLockCursor(!_showing);
    }

    public void ChangeTeam (int index)
    {
        PlayerNET.localInstance.GetComponent<ProtoPlayerMP>().Suicide();
        PlayerNET.localInstance.GetComponent<PlayerState>().ChangeTeam(index);
    }
}
