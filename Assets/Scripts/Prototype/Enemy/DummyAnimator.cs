using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DummyAnimator : MonoBehaviour
{
    protected Animator animador;
    protected NavMeshAgent agent;

    private void Awake()
    {
        animador = GetComponent<Animator>();
        agent = GetComponentInParent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        animador.SetFloat("forward", agent.velocity.magnitude);
    }

}
