using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelectorUI : MonoBehaviour
{
    [SerializeField] protected CharacterMenuUI _templateSkin;
    [SerializeField] protected Transform _panel;
    protected List<CharacterMenuUI> _skins = new List<CharacterMenuUI>();

    public void SetData (CharacterData character)
    {
        for (int i = 0; i < _skins.Count; i++)
        {
            Destroy(_skins[i].gameObject);
        }
        _skins.Clear();

        SkinData[] skins = SkinManager.instance.GetSkinsForCharacter(character.name);

        CharacterMenuUI u = Instantiate(_templateSkin, _panel);
        u.SetData(character.name, character.spr);
        u.GetComponent<Button>().onClick.AddListener(() => SkinManager.instance.ResetSkin(character.name));
        _skins.Add(u);

        for (int i = 0; i < skins.Length; i++)
        {
            CharacterMenuUI ui = Instantiate(_templateSkin, _panel);
            ui.SetData(skins[i]._displayName, skins[i]._sprite);
            int index = i;
            ui.GetComponent<Button>().onClick.AddListener(() => SkinManager.instance.SetSkin(character.name,skins[index].name));
            _skins.Add(ui);
        }
    }
}
