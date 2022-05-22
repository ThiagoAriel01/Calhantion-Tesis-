using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupsManager : MonoBehaviour
{

    static public PickUpable CreateAt(PickUpable prefab, Vector3 atPos)
    {
        PickUpable p = Instantiate(prefab, atPos + Random.insideUnitSphere, Quaternion.identity);
        Destroy(p.gameObject, 6);
        return p;
    }
}