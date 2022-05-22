using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/GameModes/Capture The Point")]
public class CapturePointGamemode : TeamDeathmatch
{

    public override void StartGameMode()
    {
        base.StartGameMode();
        //TDMStartingState startingState = new TDMStartingState("starting");
        TeamDeathMatchState baseState = new TeamDeathMatchState("base");
        GameModeState[] states = new GameModeState[]
        {
            baseState,
        };
        _fsm = new GameModeFSM(states, baseState);
        _fsm.ChangeState(baseState);
    }
}
