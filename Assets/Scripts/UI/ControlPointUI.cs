using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPointUI : MonoBehaviour
{
    [SerializeField] protected Text _label;
    [SerializeField] protected Image _domTeam;
    [SerializeField] protected Image _capturedTeam;
    [SerializeField] protected Image _capturingBy;
    protected ControlPoint _point;

    public ControlPoint p
    {
        get
        {
            return _point;
        }
    }

    public void Init (ControlPoint cp)
    {
        _point = cp;
        if (_point == null)
        {
            enabled = false;
            return;
        }
        else
        {
            enabled = true;
        }
        Refresh();
    }


    private void FixedUpdate()
    {
        Refresh();
    }

    void Refresh()
    {
        _label.text = _point.label.ToString();
        _domTeam.color = _point.domTeam == null ? Color.gray : _point.domTeam._color;
        _capturedTeam.color = _point.teamCaptured == null ? Color.gray : _point.teamCaptured._color;
        _capturingBy.color = _point.capturingBy == null ? Color.gray : _point.capturingBy._color;
        _capturingBy.fillAmount = _point.capturingT;
    }
}
