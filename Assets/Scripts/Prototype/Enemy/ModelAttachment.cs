using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAttachment : MonoBehaviour
{
    [SerializeField] protected GameObject trueAttach;
    [SerializeField] protected GameObject falseAttach;
    [SerializeField] protected string animadorKey;
    protected bool toggled;
    protected Animator animador;

    private void Awake()
    {
        animador = GetComponentInParent<Animator>();
    }

    private void FixedUpdate()
    {
        if (string.IsNullOrEmpty(animadorKey))
            return;
        bool b = animador.GetBool(animadorKey);
        if (toggled != b)
            Toggle(b);
    }

    public void Toggle(bool value)
    {
        if (trueAttach != null)
            trueAttach.SetActive(value);
        if (falseAttach!=null)
            falseAttach.SetActive(!value);
        toggled = value;
    }

}
