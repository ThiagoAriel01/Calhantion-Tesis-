using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillNotify : MonoBehaviour
{
    public delegate void KND(KillNotify kn);
    public KND onEnd;
    [SerializeField] protected Image _killerImage;
    [SerializeField] protected TextMeshProUGUI _killerName;
    [SerializeField] protected Image _skillImage;
    [SerializeField] protected TextMeshProUGUI _victimName;
    [SerializeField] protected Image _victimImage;
    [SerializeField] protected float _secs = 5;

    public void Init(Sprite killer, Sprite victim, Sprite skill, string kname, string vname)
    {
        _killerImage.sprite = killer;
        _killerName.text = kname;
        _skillImage.sprite = skill;
        _victimName.text = vname;
        _victimImage.sprite = victim;
        CancelInvoke();
        Invoke("End",_secs);
    }

    void End()
    {
        gameObject.SetActive(false);
        onEnd?.Invoke(this);
    }

}
