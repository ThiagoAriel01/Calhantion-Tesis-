using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ControlPointGMUI : MonoBehaviour
{

    [SerializeField] protected ControlPointUI _baseUI;
    protected List<ControlPointUI> _pointsUI;

    private void Awake()
    {
        _baseUI.gameObject.SetActive(false);
    }

    void CreatePointsUI(SyncList<ControlPoint> points)
    {
        _pointsUI = new List<ControlPointUI>();
        for (int i = 0; i < points.Count; i++)
        {
            ControlPointUI cpui = Instantiate(_baseUI, transform);
            cpui.Init(points[i]);
            cpui.gameObject.SetActive(true);
            _pointsUI.Add(cpui);
        }
        _pointsUI.Sort((ControlPointUI t1, ControlPointUI t2) => { return t1.p.label.CompareTo(t2.p.label); });
        for (int i = 0; i < _pointsUI.Count; i++)
        {
            _pointsUI[i].transform.SetAsLastSibling();
        }
    }

    private void FixedUpdate()
    {
        if ((_pointsUI==null) && GameModeNetworkState.instance.controlPoints.Count>0)
        {
            CreatePointsUI(GameModeNetworkState.instance.controlPoints);
            enabled = false;
        }
    }

}
