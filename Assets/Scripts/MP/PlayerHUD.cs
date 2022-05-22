using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{

    [SerializeField] protected SkillUI[] _skills;
    [SerializeField] protected HealthUI _health;
    [SerializeField] protected ManaUI _mana;
    [SerializeField] protected TeamUI _teamUI;
    [SerializeField] protected NickNameHUD _nickName;
    [SerializeField] protected CCsUI _ccs;
    [SerializeField] protected TabbedUI _tab;
    [SerializeField] protected Canvas _menuPrefab;
    [SerializeField] protected DamageContadorUI _dmgui;
    [SerializeField]
    static protected bool _lockedCursor;

    public void Init (PlayerScript p_plr)
    {
        PlayerSkills sklls = p_plr.GetComponent<PlayerSkills>();
        for (int i = 0; i < _skills.Length; i++)
        {
            _skills[i].Init(sklls);
        }
        _health.SetTarget(p_plr.Health);
        _mana.SetTarget(p_plr);
        PlayerNET plrNet = p_plr.GetComponent<PlayerNET>();
        _nickName.SetTarget(plrNet);
        _ccs.SetTarget(p_plr.GetComponent<ProtoPlayerMP>());
        _tab.SetTarget(sklls);
        PlayerState state = p_plr.GetComponent<PlayerState>();
        _teamUI.SetTarget(state);
        _dmgui.SetTarget(state);
        Canvas canvas = Instantiate(_menuPrefab, null);
        canvas.gameObject.SetActive(true);
        ToggleLockCursor(true);
        //_nickName.SetTarget(plrNet);
    }

    static public void ToggleLockCursor (bool val)
    {
        Cursor.lockState = val ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !val;
        _lockedCursor = val;
    }
}
