using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepTrigger : MonoBehaviour
{

    public delegate void OnFootstepD();
    public OnFootstepD onFootstep;

    private void OnTriggerEnter(Collider other)
    {
        onFootstep?.Invoke();
    }
}
