using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCsUI : MonoBehaviour
{
    [SerializeField] protected Text _baseElement;
    List<Text> _texts = new List<Text>();
    ProtoPlayerMP plr;

    public void SetTarget(ProtoPlayerMP pplr)
    {
        plr = pplr;
        plr.onBuffAdded += AddCC;
        plr.onBuffRemoved += RemoveCC;
    }

    void AddCC (BuffData b)
    {
        Text t = Instantiate(_baseElement, transform);
        t.text = b.DisplayEffect;
        t.color = b.DisplayColor;
        t.gameObject.SetActive(true);
        _texts.Add(t);
    }

    void RemoveCC (BuffData b)
    {
        Text toDelete = GetTextByBuff(b);
        if (toDelete == null)
            return;
        _texts.Remove(toDelete);
        Destroy(toDelete.gameObject);
    }

    Text GetTextByBuff (BuffData b)
    {
        for (int i = 0; i < _texts.Count; i++)
        {
            if (_texts[i].text == b.DisplayEffect)
                return _texts[i];
        }
        return null;
    }
}
