using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoSingleton<SkinManager>
{
    [SerializeField] protected SkinData[] _availableSkins;
    protected SkinCharacter[] _skins;

    protected class SkinCharacter
    {
        public CharacterData _character;
        public SkinData _currentSkin;
    }

    public void SetSkin (string character, string skin)
    {
        SkinCharacter sc = FindByCharName(character);
        if (sc == null)
            return;
        SkinData sd = FindSkinData(sc, skin);
        if (sd == null)
            return;
        sc._currentSkin = sd;
        LoadoutManager.instance.SetCharacter(character);
    }

    public void ResetSkin (string character)
    {
        SkinCharacter sc = FindByCharName(character);
        if (sc == null)
            return;
        sc._currentSkin = null;
        LoadoutManager.instance.SetCharacter(character);
    }

    public string GetCurrentSkinID (string character)
    {
        SkinCharacter sc = FindByCharName(character);
        if (sc == null)
            return character;
        if (sc._currentSkin == null)
            return character;
        return sc._currentSkin.name;
    }

    SkinCharacter FindByCharName (string chara)
    {
        foreach (var item in _skins)
        {
            if (item._character.name == chara)
                return item;
        }
        return null;
    }

    SkinData FindSkinData (SkinCharacter character, string skinID)
    {
        foreach (SkinData skin in _availableSkins)
        {
            if (skin.name == skinID && skin._skinOf == character._character)
                return skin;
        }
        return null;
    }

    public SkinData[] GetSkinsForCharacter (string character)
    {
        List<SkinData> datas = new List<SkinData>();
        for (int i = 0; i < _availableSkins.Length; i++)
        {
            if (_availableSkins[i]._skinOf.name == character)
                datas.Add(_availableSkins[i]);
        }
        return datas.ToArray();
    }

    override protected bool Awake()
    {
        if (!base.Awake())
            return false;
        CharacterData[] chars = LoadoutManager.instance.characters;
        _skins = new SkinCharacter[chars.Length];
        for (int i = 0; i < chars.Length; i++)
        {
            _skins[i] = new SkinCharacter() { _character = chars[i], _currentSkin = null };
        }
        return true;
    }
}
