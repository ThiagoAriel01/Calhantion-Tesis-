using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    [SerializeField] protected HealthComponent healthTarget;
    [SerializeField] protected Image healthBar;
    [SerializeField] protected Text healthNumber;
    [SerializeField] protected float _decayRate=100f;
    [SerializeField] protected float _decaydelay = 1f;
    [SerializeField] protected Image damageBar;
    [SerializeField] protected Text lastDMGNumber;
    [SerializeField] protected AudioCue _hurtCue;
    [SerializeField] protected AudioCue _hurtlargeCue;
    [SerializeField] protected Color _hurtColor;
    [SerializeField] protected Color _healColor;
    protected float accumulatedDmg;
    protected float lastDir;
    protected bool _decaying;

    public void SetTarget(HealthComponent p_health)
    {
        healthTarget = p_health;
        healthTarget.onHealthChangeClient += OnHealthChange;
        lastDMGNumber.gameObject.SetActive(false);
    }

    void OnHealthChange(HealthComponent health, float oldh, float newh)
    {
        float dmg = oldh - newh;
        float dir = dmg > 0 ? -1 : dmg < 0 ? 1 : 0;
        if (dmg == 0)
            return;

        if (dmg >= 100f)
            _hurtlargeCue.PlaySound2DLocal();
        else if (dmg>0)
            _hurtCue.PlaySound2DLocal();

        lastDMGNumber.gameObject.SetActive(true);
        lastDMGNumber.color = dir > 0 ? _healColor : _hurtColor;
        _decaying = false;
        if (lastDir != dir)
            accumulatedDmg = 0;
        accumulatedDmg += dmg;
        lastDir = dir;
        CancelInvoke();
        Invoke("DecayNow",_decaydelay);
    }

    void DecayNow()
    {
        lastDMGNumber.gameObject.SetActive(false);
        accumulatedDmg = 0;
        _decaying = true;
    }

    private void Update()
    {
        if (healthTarget==null)
            return;
        healthBar.fillAmount = healthTarget.Health / healthTarget.HealthMax;
        damageBar.fillAmount = Mathf.Clamp(damageBar.fillAmount, healthBar.fillAmount, 1f);
        lastDMGNumber.text = (lastDir > 0 ? "+" : "-") + ((int)Mathf.Abs(accumulatedDmg)).ToString();
        if (_decaying)
        {
            damageBar.fillAmount = Mathf.MoveTowards(damageBar.fillAmount, healthBar.fillAmount, Time.deltaTime * _decayRate);
        }
        healthNumber.text = ((int)healthTarget.Health).ToString();
    }
}
