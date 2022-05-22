using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamDeathMatchState : DeathMatchState
{

    public TeamDeathMatchState(string pid) : base(pid)
    {

    }

    override public Transform ChooseSpawnLocation()
    {
        return base.ChooseSpawnLocation();
    }

    public override void Enter(GameModeFSM fsm)
    {
        base.Enter(fsm);
        //TesisNetworkManager.instancia.TrySpawnAllPlayers();
    }

    override public Transform ChooseSpawnLocation(PlayerState state)
    {
        Transform t = TesisNetworkManager.instancia.GetStartPositionName(("TEAM" + state.teamIndex));
        if (t == null)
            return base.ChooseSpawnLocation();
        return t;
    }

    public override bool CanDamagePlayer(PlayerState attacker, PlayerState victim)
    {
        return attacker.teamIndex != victim.teamIndex;
    }

    public override void OnPlayerSpawn(NetworkConnection conn, PlayerState state, GameMode gm)
    {
        base.OnPlayerSpawn(conn, state,gm);

        TeamDeathmatch tdm = gm as TeamDeathmatch;
        if (tdm == null)
            return;

        TeamDeathmatch.Team[] teams = tdm.GetTeams();
        int[] playersInTeam = new int[teams.Length];
        for (int i = 0; i < teams.Length; i++)
        {
            playersInTeam[i] = tdm.GetPlayersInTeam(i);
        }

        int smallestTeamIndex = 0;
        int smallestPlayers = 999;
        for (int i = 0; i < playersInTeam.Length; i++)
        {
            if (playersInTeam[i] < smallestPlayers)
            {
                smallestPlayers = playersInTeam[i];
                smallestTeamIndex = i;
            }
        }

        state.teamIndex = smallestTeamIndex;
        state.onServerTeamChanged?.Invoke(smallestTeamIndex);
        state.GetComponent<ProtoPlayerMP>().RespawnNow();
        //state.GetComponent<ProtoPlayerMP>().RespawnNow();
    }
}
