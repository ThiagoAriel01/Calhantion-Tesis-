using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MessageManager : MonoSingleton<MessageManager>, INetworkHandler
{

    protected struct STxtMessage : NetworkMessage
    {
        public string text;
        public Color color;
        public int channel;
        public float duration;
    }

    protected struct EndMSG : NetworkMessage
    {
        public int channel;
        public bool win;
    }

    protected override bool Awake()
    {
        if (!base.Awake())
            return false;
        (this as INetworkHandler).RegisterHandlers();
        return true;
    }

    void INetworkHandler.RegisterHandlers()
    {
        NetworkClient.RegisterHandler<STxtMessage>(OnMessage);
        NetworkClient.RegisterHandler<EndMSG>(OnEnd);
    }

    void OnMessage(STxtMessage msg)
    {
        MessageUI.Message(msg.channel, msg.text, msg.color, msg.duration);
    }

    void OnEnd(EndMSG msg)
    {
        MessageUI.EndGame(msg.channel,msg.win);
    }

    [Server]
    public void EndToEspecific(NetworkConnection conn,int channel, bool win)
    {
        EndMSG msg = new EndMSG
        {
            win = win,
        };
        conn.Send(msg);
    }

    [Server]
    public void SendMessageToAll (int channel, string txt, Color color, float duration = 5f)
    {
        STxtMessage msg = new STxtMessage
        {
            channel = channel,
            color = color,
            text = txt,
            duration = duration
        };
        NetworkServer.SendToAll(msg);
    }
}
