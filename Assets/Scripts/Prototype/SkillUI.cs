using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [SerializeField] protected int skillIndex;
    [SerializeField] protected bool tabbing;
    [SerializeField] protected Image image;
    [SerializeField] protected Image cooldown;
    [SerializeField] protected Text cooldownText;
    [SerializeField] protected Image cooldownFade;
    protected int defaultIndex;
    protected PlayerSkills sk;

    private void Awake()
    {
        defaultIndex = skillIndex;
    }

    public void Init(PlayerSkills p_sk)
    {
        sk = p_sk;
        sk.onSkillTab += OnTab;
    }

    void OnTab (PlayerSkills sk)
    {
        if (tabbing)
            skillIndex = sk.Tabbed ? defaultIndex + PlayerSkills.SkillsPerTab : defaultIndex;
    }

    private void Update()
    {
        try
        {
            SkillData skill = sk.GetSkill(skillIndex);
            cooldown.fillAmount = skill.CurrentCooldown / skill.Cooldown;
            image.sprite = skill.Icon;
            bool ac = skill.CurrentCooldown > 0;
            if (cooldownText.gameObject.activeSelf != ac)
            {
                cooldownText.gameObject.SetActive(ac);
                cooldownFade.gameObject.SetActive(ac);
            }
            if (skill.CurrentCooldown < 10)
                cooldownText.text = skill.CurrentCooldown.ToString("0.0");
            else
                cooldownText.text = ((int)skill.CurrentCooldown).ToString();
            image.color = sk.Char.Mana < skill.Mana ? Color.cyan : Color.white;
        }
        catch
        {

        }
    }

}
