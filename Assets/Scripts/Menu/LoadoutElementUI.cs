using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutElementUI : MonoBehaviour
{
    [SerializeField] protected Image _image;
    [SerializeField] protected Text _text;

    [SerializeField] protected int _index;
    [SerializeField] protected string _filter;
    protected SkillData _target;

    public SkillData target
    {
        get => _target;
    }

    public void Set (Sprite spr, string text, SkillData target)
    {
        _text.text = text;
        _image.sprite = spr;
        _target = target;
    }

    public void PressedChange()
    {
        LoadoutUI.instance.SetFilter(_filter);
        LoadoutUI.instance.StartSelectingSkill(_index);
    }

    public void Pressed()
    {
        LoadoutUI.instance.Select(_target);
    }
}