using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostButtonUI : MonoBehaviour
{
    protected Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        MenuManager.instance.onChangedGameMode += (x) => Refresh();
        MenuManager.instance.onChangedMap += (x) => Refresh();
    }

    void Refresh()
    {
        MapData d = MenuManager.instance.maps[MenuManager.instance.selectedMapIndex];
        GameMode gm = GameModeManager.instance.selectedGameMode;
        bool isvalid = d.IsCompatibleWith(gm);
        _button.interactable = isvalid;
    }
}
