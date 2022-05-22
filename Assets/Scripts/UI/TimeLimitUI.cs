using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLimitUI : MonoBehaviour
{
    [SerializeField] protected Text _time;

    private void Awake()
    {
        _time.text = "";
    }

    private void FixedUpdate()
    {
        if (GameModeNetworkState.instance.currentSecond <= 0 && GameModeNetworkState.instance.maxSeconds <= 0)
            return;
        System.TimeSpan time = System.TimeSpan.FromSeconds(GameModeNetworkState.instance.maxSeconds - GameModeNetworkState.instance.currentSecond);
        string timeText = string.Format("{0:D2}:{1:D2}",time.Minutes, time.Seconds);
        _time.text = timeText;
    }
}
