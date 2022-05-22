using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterMenuUI : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _name;
    [SerializeField] protected Image _image;

    public void SetData(string gname, Sprite sprte)
    {
        _image.sprite = sprte;
        _name.text = gname.ToUpper();
    }
}
