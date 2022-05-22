using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterElementUI : MonoBehaviour
{
    [SerializeField] protected string _character;

    public void Select()
    {
        LoadoutManager.instance.SetCharacter(_character);
    }
}