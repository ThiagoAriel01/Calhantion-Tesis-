using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumberUI : MonoBehaviour
{
    [SerializeField] protected float _refdamagemax=300f;
    [SerializeField] protected float _refmaxScale = 1.5f;
    [SerializeField] protected Text text;
    [SerializeField] protected Animator animador;
    protected GameObject _victim;
    protected int _value;

    public GameObject victim
    {
        get
        {
            return _victim;
        }
        set
        {
            _victim = value;
        }
    }

    public int value
    {
        get
        {
            return _value;
        }
        set
        {
            text.text = value.ToString();
            //transform.localScale = Vector3.one + (Vector3.one * (value / _refdamagemax) * _refmaxScale);
            _value = value;
        }
    }

    public void Bump()
    {
        animador.Play("bump", 0, 0);
        CancelInvoke("EndNow");
        Invoke("EndNow", 1.5f);
    }

    void EndNow()
    {
        value = 0;
        victim = null;
        gameObject.SetActive(false);
    }

}
