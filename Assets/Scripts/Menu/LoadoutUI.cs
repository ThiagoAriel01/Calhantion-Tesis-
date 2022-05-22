using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadoutUI : MonoSingleton<LoadoutUI>
{

    [SerializeField] protected LoadoutElementUI[] _skillsSlots;
    [SerializeField] protected LoadoutElementUI _baseElement;
    [SerializeField] protected TextMeshProUGUI _characterName;
    [SerializeField] protected Text _desc;
    [SerializeField] protected RectTransform _panel;
    protected SkillData _selected;
    protected Loadout _curLoadout;
    protected string _filter;
    protected delegate void ConfirmDelegate(SkillData sk);
    protected ConfirmDelegate onConfirm;

    List<LoadoutElementUI> elements = new List<LoadoutElementUI>();

    public void Select (SkillData data)
    {
        _selected = data;
        _desc.text = data.DisplayName + "\n" + data.DisplayDesc;
    }

    public bool CanConfirm()
    {
        for (int i = 0; i < _skillsSlots.Length; i++)
        {
            if (_skillsSlots[i].target == null)
                return false;
        }
        return true;
    }

    public void Confirm()
    {
        onConfirm?.Invoke(_selected);
    }

    public void Save()
    {
        LoadoutManager.instance.SaveLoadout();
    }

    public void LoadLoadout(int index)
    {
        //Save();
        _curLoadout = LoadoutManager.instance.GetLoadout(index);
        _filter = string.Empty;
        for (int i = 0; i < _curLoadout._skills.Length; i++)
        {
            SetSlotUI(i, SkillUtlity.GetSkillByName(_curLoadout._skills[i], LoadoutManager.instance.skills));
        }
        LoadoutManager.instance.SetLoadout(index);
    }

    void SetSlotUI (int slot,SkillData sk)
    {
        if (sk == null)
        {
            _skillsSlots[slot].Set(null, "None", null);
            return;
        }

        _skillsSlots[slot].Set(sk.Icon, sk.DisplayName, sk);
    }

    void FilterElements (string tag)
    {
        foreach (LoadoutElementUI element in elements)
        {
            element.gameObject.SetActive(element.target.HasTag(tag));
        }
    }

    void FilterElements (SkillData excludedata)
    {
        foreach (LoadoutElementUI element in elements)
        {
            if (element.target == excludedata)
                element.gameObject.SetActive(false);
        }
    }

    public void SetFilter (string f)
    {
        _filter = f;
    }

    public void StartSelectingSkill(int index)
    {
        _selected = null;
        _desc.text = "None";
        FilterElements(_filter);
        foreach (var item in _skillsSlots)
        {
            if (_skillsSlots[index] != item)
                FilterElements(item.target);
        }
        onConfirm = (x) => { LoadoutManager.instance.SetSkill(index, x); SetSlotUI(index, x); };
    }

    private void Update()
    {
        _characterName.text = LoadoutManager.instance.GetCharacter();
    }
    private void Start()
    {
        _baseElement.gameObject.SetActive(false);
        for (int i = 0; i < LoadoutManager.instance.skills.Length; i++)
        {
            LoadoutElementUI element = Instantiate(_baseElement, _panel);
            element.Set(LoadoutManager.instance.skills[i].Icon, LoadoutManager.instance.skills[i].DisplayName, LoadoutManager.instance.skills[i]);
            element.gameObject.SetActive(true);
            elements.Add(element);
        }
        LoadLoadout(LoadoutManager.instance.selectedLoadout);
    }
}