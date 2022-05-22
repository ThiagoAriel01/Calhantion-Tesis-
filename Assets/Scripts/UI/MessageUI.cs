using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageUI : MonoBehaviour
{
    [SerializeField] protected int _channel;
    [SerializeField] protected Text _text;
    [SerializeField] protected Image _finishImage;
    [SerializeField] protected TextMeshProUGUI _finishText;
    [SerializeField] protected Sprite _finishVictory;
    [SerializeField] protected Sprite _finishDefeat;
    protected string _lastMessage;
    static protected List<MessageUI> msgs = new List<MessageUI>();

    private void Awake()
    {
        msgs.Insert(_channel, this);
    }

    private void OnDestroy()
    {
        msgs.Remove(this);
    }

    void EndMSG()
    {
        _text.gameObject.SetActive(false);
    }

    static public bool EndGame(int channel, bool win)
    {
        MessageUI msg = msgs[channel];
        msg._finishImage.gameObject.SetActive(true);
        msg._finishImage.sprite = win ? msg._finishVictory : msg._finishDefeat;
        msg._finishText.text = win ? "Victory" : "Defeat";
        return true;
    }

    static public bool Message (int channel,string txt, Color color, float duration=5f)
    {
        MessageUI msg = msgs[channel];
        msg._text.color = color;
        msg._text.text = txt;
        msg._text.gameObject.SetActive(true);
        msg.CancelInvoke("EndMSG");
        msg.Invoke("EndMSG", duration);
        return true;
    }
}