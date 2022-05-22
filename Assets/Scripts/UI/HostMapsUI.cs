using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostMapsUI : MonoBehaviour
{
    [SerializeField] protected MapUI _baseUI;
    protected MapUI[] _mapsUI;

    private void Awake()
    {
        _mapsUI = new MapUI[MenuManager.instance.maps.Length];
        for (int i = 0; i < MenuManager.instance.maps.Length; i++)
        {
            _mapsUI[i] = Instantiate(_baseUI, transform);
            _mapsUI[i].Init(MenuManager.instance.maps[i]);
        }
        MenuManager.instance.onChangedGameMode += (x) =>
        {
            for (int i = 0; i < _mapsUI.Length; i++)
            {
                _mapsUI[i].Refresh();
            }
        };
    }
}