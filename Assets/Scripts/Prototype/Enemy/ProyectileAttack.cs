using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Proyectile Attack")]
public class ProyectileAttack : ScriptableObject
{
    [SerializeField] public string trigger;
    [SerializeField] public float damage;
    [SerializeField] public Proyectile proyectile;
    [SerializeField] public float delay = 1.5f;
    [SerializeField] public float fireRate = 2f;
    [SerializeField] public float spread = 0.02f;
    [SerializeField] public float proyectileSpeed = 25;
    [SerializeField] public float explosionScale=1;
    [SerializeField] public Explosion explosion;

}
