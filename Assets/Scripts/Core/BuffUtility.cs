using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffUtility : MonoSingleton<BuffUtility>
{
    [SerializeField] protected BuffData[] buffs;

    public BuffData GetBuffByName(string n)
    {
        foreach (BuffData item in buffs)
        {
            if (item.name == n)
                return item;
        }
        return null;
    }

    public BuffData GetBuffByNameInstance(string n)
    {
        foreach (BuffData item in buffs)
        {
            if (item.name + "(Clone)" == n)
                return item;
        }
        return null;
    }
}
