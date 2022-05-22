using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDisplayer : MonoBehaviour
{
    protected string prevChar;
    protected string curChar;
    protected GameObject _character;

    private void Awake()
    {
        
    }

    private void Update()
    {
        curChar = LoadoutManager.instance.GetCharacter();
        if (curChar != prevChar)
        {
            prevChar = curChar;
            GameObject prefab = Resources.Load<GameObject>("Player/" + curChar);
            if (prefab == null)
                return;
            if (_character != null)
            {
                Destroy(_character.gameObject);
            }
            _character = Instantiate(prefab, transform.position, transform.rotation);
            _character.AddComponent<CharacterMenuModel>();
            _character.gameObject.SetActive(true);
            _character.transform.SetParent(transform);
        }
    }
}
