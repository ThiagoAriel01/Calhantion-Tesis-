using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/GameModes/Deathmatch")]
public class DeathMatchGameMode : GameMode
{
    public override void StartGameMode()
    {
        base.StartGameMode();
        DeathMatchState baseState = new DeathMatchState("base");
        GameModeState[] states = new GameModeState[]
        {
            baseState,
        };
        _fsm = new GameModeFSM(states,baseState);
        _fsm.ChangeState(baseState);
    }
}