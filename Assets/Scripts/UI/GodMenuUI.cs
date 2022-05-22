using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GodMenuUI : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _name;
    [SerializeField] protected TextMeshProUGUI _story;
    [SerializeField] protected Image _image;
    [SerializeField] protected Transform _camera;
    [SerializeField] protected SkinSelectorUI _skins;
    protected CharacterData _data;

    public void SetGodCamera()
    {
        Vector3 v = _camera.transform.position;
        v.x = -1.75f;
        _camera.transform.position = v;
    }

    public void ResetCamera()
    {
        Vector3 v = _camera.transform.position;
        v.x = 0f;
        _camera.transform.position = v;
    }

    public void SetData (CharacterData d)
    {
        _data = d;
        LoadoutManager.instance.SetCharacter(d.name);
        _skins.SetData(d);
        Refresh();
    }

    void Refresh()
    {
        _name.text = _data.name.ToUpper();
        _story.text = _data.description;
        _image.sprite = _data.spr;
    }

}
