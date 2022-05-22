using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabbedUI : MonoBehaviour
{
    protected PlayerSkills sks;
    [SerializeField] protected GameObject target;

    public void SetTarget (PlayerSkills psk)
    {
        sks = psk;
        sks.onSkillTab += OnTab;
        target.gameObject.SetActive(false);
    }

    void OnTab(PlayerSkills psk)
    {
        target.gameObject.SetActive(psk.Tabbed);
    }

}