using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [SerializeField] protected Text _nameText;
    protected Button _button;
    protected MapData _data;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void Init(MapData data)
    {
        _data = data;
        _button.onClick.AddListener(Select);
        Refresh();
    }

    public void Refresh()
    {
        _button.interactable = _data.IsCompatibleWith(GameModeManager.instance.selectedGameMode);
        _nameText.text = _data.displayName;
    }

    public void Select()
    {
        MenuManager.instance.SelectMap(_data);
    }
}
