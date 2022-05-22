using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathsUI : MonoBehaviour
{
    [SerializeField] protected Text _text;
    private PlayerState _net;

    private void Awake()
    {
    }

    private void FixedUpdate()
    {
        if (_net == null)
        {
            try
            {
                _net = PlayerNET.localInstance.GetComponent<PlayerState>();
            }
            catch
            {

            }
            return;
        }
        _text.text = "" + _net.deaths.ToString();
    }
}
