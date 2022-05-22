using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Characters/Skin")]
public class SkinData : ScriptableObject
{
    [SerializeField] public CharacterData _skinOf;
    [SerializeField] public string _displayName;
    [SerializeField] public Sprite _sprite;
}