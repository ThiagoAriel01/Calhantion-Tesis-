using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class ResultsScreenUI : MonoBehaviour
{
    [SerializeField] protected Image _cuadro;
    [SerializeField] protected Sprite _victory;
    [SerializeField] protected Sprite _defeat;
    [SerializeField] protected TextMeshProUGUI _victorydefeatText;
    [SerializeField] protected TextMeshProUGUI _overview;
    [SerializeField] protected RectTransform _scoreBoard;
    [SerializeField] protected ScoreElementUI _baseUI;
    protected List<ScoreElementUI> _elements = new List<ScoreElementUI>();
    public UnityEvent onGameEnded;

    private void Start()
    {
        if (!ResultsScreenManager.instance.received)
            return;
        ResultsScreenManager.instance.received = true;
        GameModeManager.GameEndedMsg msg = ResultsScreenManager.instance.msg;
        _overview.text = msg.localPlayer.name + "\n" + 
            "Kills " + msg.localPlayer.kills + "\n" +
            "Deaths " + msg.localPlayer.deaths + "\n" +
            "Damage " + DamageContadorUI.kFormatter(msg.localPlayer.damage) + "\n" +
            "Team " + msg.localPlayer.teamName
            ;
        _victorydefeatText.text = msg.victory ? "Victory" : "Defeat";
        _cuadro.sprite = msg.victory ? _victory : _defeat;

        for (int i = 0; i < msg.players.Length; i++)
        {
            ScoreElementUI ui = Instantiate(_baseUI, _scoreBoard);
            CharacterData d = LoadoutManager.instance.GetCharacterData(msg.players[i].model);
            Sprite ic = d.icon;
            ui.Init(msg.players[i].name,msg.players[i].kills, msg.players[i].deaths.ToString(), msg.players[i].teamName, msg.players[i].teamColor, DamageContadorUI.kFormatter(msg.players[i].damage), ic);
            _elements.Add(ui);
        }

        _elements = _elements.OrderBy(c => c.Kills).ThenBy(n => n.Team).ThenBy(p => p.nametext).ToList();
        for (int i = _elements.Count - 1; i >= 0; i--)
        {
            _elements[i].transform.SetAsLastSibling();
        }
        onGameEnded?.Invoke();
    }
}
