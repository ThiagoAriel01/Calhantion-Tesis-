using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodsMenuUI : MonoBehaviour
{
    [SerializeField] protected CharacterMenuUI _characterPrefab;
    [SerializeField] protected Transform _panel;
    [SerializeField] protected GameObject _godMenu;
    [SerializeField] protected GodMenuUI _godMenuUI;
    protected List<CharacterMenuUI> _charactersUI = new List<CharacterMenuUI>();

    private void Awake()
    {
        CharacterData[] chars = LoadoutManager.instance.characters;
        foreach (CharacterData character in chars)
        {
            CharacterMenuUI ui = Instantiate(_characterPrefab, _panel);
            ui.SetData(character.name,character.spr);
            ui.GetComponent<Button>().onClick.AddListener(()=>ClickedGod(character));
            _charactersUI.Add(ui);
        }
    }

    public void ClickedGod(CharacterData data)
    {
        gameObject.SetActive(false);
        _godMenuUI.SetGodCamera();
        _godMenu.gameObject.SetActive(true);
        _godMenuUI.SetData(data);
    }

    public void ClickedGod(string god)
    {
        CharacterData d = LoadoutManager.instance.GetCharacterData(god);
        if (d!=null)
        {
            ClickedGod(d);
        }
    }
}
