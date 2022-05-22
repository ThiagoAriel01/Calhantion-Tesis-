using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    protected Enemy en;
    protected Animator _animador;

    private void Awake()
    {
        _animador = GetComponent<Animator>();
        en = GetComponentInParent<Enemy>();
    }

    public Animator animador
    {
        get
        {
            return _animador;
        }
    }

    private void OnAnimatorMove()
    {
        if (animador == null)
            return;

        en.AddDelta(animador.deltaPosition, animador.deltaRotation,animador.velocity);
    }
}
