using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KillNotifiesManager : MonoSingleton<KillNotifiesManager>, INetworkHandler
{
    protected struct KMessage : NetworkMessage
    {
        public string killerSprite;
        public string killerName;
        public string skillSprite;
        public string victimName;
        public string victimSprite;
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
        NetworkClient.RegisterHandler<KMessage>(OnKillNotify);
    }


    void OnKillNotify(KMessage msg)
    {
        Sprite killerspr = Resources.Load<Sprite>("Sprites/" + msg.killerSprite);
        Sprite victimSpr = Resources.Load<Sprite>("Sprites/" + msg.victimSprite);
        Sprite skillSpr = Resources.Load<Sprite>("Sprites/" + msg.skillSprite);
        KillNotifiesUI.Notify(killerspr,msg.killerName,skillSpr,msg.victimName, victimSpr);
    }

    [Server]
    public void SendNotifyToAll(string killerspr, string killername, string skillspr, string victimname, string victimspr)
    {
        KMessage msg = new KMessage
        {
            killerSprite = killerspr,
            killerName = killername,
            skillSprite = skillspr,
            victimName = victimname,
            victimSprite = victimspr,
        };
        NetworkServer.SendToAll(msg);
        AudioCue cue = Resources.Load<AudioCue>("AudioCue/" + "KillNotify");
        if (cue != null)
            cue.PlaySound();
    }
}
