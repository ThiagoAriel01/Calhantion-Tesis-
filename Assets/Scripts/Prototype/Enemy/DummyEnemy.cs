using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DummyEnemy : Character
{
    protected NavMeshAgent agent;
    protected Vector3 startPoint;

    override protected void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        startPoint = transform.position;
        StartCoroutine(Patrol());
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            NavMeshHit hit;
            bool valid = NavMesh.SamplePosition(startPoint + Random.insideUnitSphere * 15f, out hit, 5f, NavMesh.AllAreas);
            float dist = 5f;
            while (!valid)
            {
                valid = NavMesh.SamplePosition(startPoint + Random.insideUnitSphere * 15f, out hit, dist, NavMesh.AllAreas);
                dist += 5f;
                yield return null;
            }
            agent.SetDestination(hit.position);
            while (valid && agent.remainingDistance> agent.stoppingDistance + 1f)
            {
                yield return null;
            }
            yield return null;
        }
    }

}