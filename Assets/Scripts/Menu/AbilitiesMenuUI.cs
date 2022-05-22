using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitiesMenuUI : MonoBehaviour
{
    [SerializeField] protected LoadoutElementUI _skillUIPrefab;
    [SerializeField] protected Text _textdesc;
    [SerializeField] protected Transform _panel;
    protected List<LoadoutElementUI> _skills = new List<LoadoutElementUI>();

    private void Awake()
    {
        SkillData[] skills = LoadoutManager.instance.skills;
        foreach (SkillData skill in skills)
        {
            if (!skill.HasTag("basic"))
            {
                LoadoutElementUI ui = Instantiate(_skillUIPrefab, _panel);
                ui.Set(skill.Icon, skill.DisplayName, skill);
                ui.GetComponent<Button>().onClick.AddListener(() => ClickedSkill(skill));
                _skills.Add(ui);
            }
        }
        Filter("primary");
    }

    public void Filter (string tag)
    {
        foreach (LoadoutElementUI e in _skills)
        {
            e.gameObject.SetActive(e.target.HasTag(tag));
        }
    }

    void ClickedSkill(SkillData data)
    {
        _textdesc.text = data.DisplayDesc;
    }
}