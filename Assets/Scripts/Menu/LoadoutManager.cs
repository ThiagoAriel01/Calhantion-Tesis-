using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutManager : MonoSingleton<LoadoutManager>
{
    [SerializeField] protected SkillData[] _skillsAvailable;
    [SerializeField] protected SkillData _defaultDash;
    [SerializeField] protected SkillData[] _defaultSkills;
    [SerializeField] protected CharacterData[] _characterDatas;
    [SerializeField] protected string _defaultCharacter = "Anubis";
    protected LoadoutsData _loadoutData = new LoadoutsData();
    protected int _selected = 0;
    static protected bool _pendingChange;
    static protected int _version = 7;

    static public bool pendingChange
    {
        get
        {
            return _pendingChange;
        }
        set
        {
            _pendingChange = value;
        }
    }

    public SkillData[] skills
    {
        get
        {
            return _skillsAvailable;
        }
    }

    public CharacterData[] characters
    {
        get
        {
            return _characterDatas;
        }
    }

    public SkillData GetSkill (string str)
    {
        string extra = "";
        if (str.Contains("(Clone)"))
        {
            extra = "(Clone)";
        }
            foreach (var item in _skillsAvailable)
            {
                if (item.name + extra == str)
                    return item;
            }
        return null;
    }

    public CharacterData GetCharacterData (string charname)
    {
        for (int i = 0; i < _characterDatas.Length; i++)
        {
            if (charname.Contains(_characterDatas[i].name))
                return _characterDatas[i];
        }
        return null;
    }

    public string GetCharacter()
    {
        return _loadoutData._loadouts[_selected]._character;
    }

    public SkillData[] GetSkills()
    {
        SkillData[] sks = new SkillData[11];
        int i = 0;
        foreach (string item in _loadoutData._loadouts[_selected]._skills)
        {
            sks[i] = SkillUtlity.GetSkillByName(item, _skillsAvailable);
            i++;
        }
        sks[10] = _defaultDash;
        CharacterData cd = GetCharacterData(_loadoutData._loadouts[_selected]._character);
        if (cd!=null && cd._basic != null)
            sks[0] = cd._basic;
        return sks;
    }

    public int selectedLoadout
    {
        get
        {
            return _selected;
        }
    }

    public Loadout GetLoadout (int index)
    {
        return _loadoutData._loadouts[index];
    }

    public void SetLoadout(int index)
    {
        _selected = index;
    }

    protected override bool Awake()
    {

        if (!base.Awake())
            return false;
        _selected = PlayerPrefs.GetInt("loadout_selected", 0);

        _loadoutData._loadouts = new Loadout[3];
        _loadoutData._version = _version;
        for (int i = 0; i < _loadoutData._loadouts.Length; i++)
        {
            _loadoutData._loadouts[i] = new Loadout();
            _loadoutData._loadouts[i]._skills = new string[10];
            for (int j = 0; j < _loadoutData._loadouts[i]._skills.Length; j++)
            {
                _loadoutData._loadouts[i]._skills[j] = "";
            }
        }

        string json = FileStreamer.instance.LoadText(FileStreamer.appdata + "/loadouts.json");
        LoadoutsData lds = JsonUtility.FromJson<LoadoutsData>(json);
        if (string.IsNullOrEmpty(json) || lds._version < _version)
        {
            for (int i = 0; i < _loadoutData._loadouts.Length; i++)
            {
                _loadoutData._loadouts[i]._character = _defaultCharacter;
                for (int j = 0; j < _loadoutData._loadouts[i]._skills.Length; j++)
                {
                    _loadoutData._loadouts[i]._skills[j] = _defaultSkills[j].name;
                }
            }
            return true;
        }
        _loadoutData = lds;
        lds._version = _version;
        return true;
    }

    public void SaveLoadout()
    {
        string json = JsonUtility.ToJson(_loadoutData);
        FileStreamer.instance.WriteText(json, FileStreamer.appdata + "/loadouts.json");
        PlayerPrefs.SetInt("loadout_selected",_selected);
        _pendingChange = true;
    }

    public void SetCharacter(string newCharacter)
    {

        newCharacter = SkinManager.instance.GetCurrentSkinID(newCharacter);
        _loadoutData._loadouts[_selected]._character = newCharacter;
    }

    public void SetSkill(int index,SkillData newSkill)
    {
        if (newSkill == null)
            return;
        //Debug.LogError(_loadoutData);
        //Debug.LogError(_loadoutData._loadouts);
        //Debug.LogError(_loadoutData._loadouts[_selected]);
       // Debug.LogError(_loadoutData._loadouts[_selected]._skills);
        //Debug.LogError(_loadoutData._loadouts[_selected]._skills[index]);
        _loadoutData._loadouts[_selected]._skills[index] = newSkill.name;
    }
}
