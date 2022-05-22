using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Character")]
public class CharacterData : ScriptableObject
{
    [TextArea(5,10)]
    public string description;
    public Sprite spr;
    public Sprite icon;
    public SkillData[] _skills;
    public SkillData _basic;
}