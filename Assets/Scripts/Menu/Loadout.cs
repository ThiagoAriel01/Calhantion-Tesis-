using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Loadout
{
    [SerializeField] public string _character;
    [SerializeField] public string[] _skills = new string[10];
}

[System.Serializable]
public class LoadoutsData
{
    [SerializeField] public int _version;
    [SerializeField] public Loadout[] _loadouts = new Loadout[3];
}