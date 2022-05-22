using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillNotifiesUI : MonoBehaviour
{
    [SerializeField] protected KillNotify _notifyPrefab;
    [SerializeField] protected int _maxNotifies = 5;
    protected KillNotify[] _notifies;
    protected List<KillNotify> _activeNotifies = new List<KillNotify>();
    static protected KillNotifiesUI _instance;

    static protected  KillNotifiesUI instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        _notifies = new KillNotify[_maxNotifies];
        for (int i = 0; i < _notifies.Length; i++)
        {
            KillNotify k = Instantiate(_notifyPrefab, transform);
            k.gameObject.SetActive(false);
            k.onEnd += OnNotifyEnd;
            _notifies[i] = k;
        }
    }

    void OnNotifyEnd(KillNotify kn)
    {
        if (_activeNotifies.Contains(kn))
            _activeNotifies.Remove(kn);
    }

    public void AddNotify (Sprite killerspr,string killername, Sprite skillspr, string victimname, Sprite victimspr)
    {
        KillNotify k = GetNotify();
        k.Init(killerspr, victimspr, skillspr, killername, victimname);
        if (_activeNotifies.Contains(k))
            _activeNotifies.Remove(k);
        _activeNotifies.Add(k);
        k.gameObject.SetActive(true);
    }

    public KillNotify GetNotify()
    {
        foreach (KillNotify item in _notifies)
        {
            if (!item.gameObject.activeSelf)
            {
                return item;
            }
        }
        return _activeNotifies[0];
    }

    static public bool Notify(Sprite killerspr, string killername, Sprite skillspr, string victimname, Sprite victimspr)
    {
        _instance.AddNotify(killerspr, killername, skillspr, victimname, victimspr);
        return true;
    }

}
