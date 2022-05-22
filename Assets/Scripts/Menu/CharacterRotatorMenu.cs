using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotatorMenu : MonoBehaviour
{
    [SerializeField] protected Transform _tRotator;
    [SerializeField] protected float _rotSpeed = 45f;
    protected int _moving;

    public void Press(int dir)
    {
        _moving = dir;
    }

    public void Release()
    {
        _moving = 0;
    }

    public void ResetRot()
    {
        _moving = 0;
        _tRotator.transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        if (_moving!=0)
            _tRotator.Rotate(Vector3.up * _rotSpeed * _moving * Time.deltaTime);
    }
}
