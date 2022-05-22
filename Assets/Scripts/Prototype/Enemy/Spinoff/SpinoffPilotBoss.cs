using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinoffPilotBoss : AerialEnemy
{
    [SerializeField] protected float strafeDurationMin = 0.3f;
    [SerializeField] protected float strafeDurationMax = 0.5f;
    [SerializeField] protected int strafesMin = 3;
    [SerializeField] protected int strafesMax = 11;
    protected bool strafing;
    protected bool strafingRight;
}
