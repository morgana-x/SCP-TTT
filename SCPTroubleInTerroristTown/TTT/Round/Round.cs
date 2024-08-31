using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using PluginAPI.Core;
using MEC;
using SCPTroubleInTerroristTown.TTT.TraitorTester;

namespace SCPTroubleInTerroristTown.TTT
{
    public class DeathReason
    {
        public Team.Team team;
        public DamageHandlerBase handler;
    }

    public partial class Round
    {
        private Dictionary<Player, DeathReason> deathReason = new Dictionary<Player, DeathReason>();

        private MEC.CoroutineHandle think_task;

        public TTTConfig config;

        public Hud.Hud hudManager;

        public Karma.KarmaManager karmaManager;

        public PlayerManager.PlayerManager playerManager;

        public Map.MapManager mapManager;

        public Team.TeamManager teamManager;

        public TraitorTester.TraitorTester traitorTester;

        public DateTime NextRoundState;

        public RoundState currentRoundState;

        public Team.Team winner;
        public enum RoundState
        {
            WaitingForPlayers,
            Preperation,
            Running,
            Finished,
            Reset
        }
        public Round(TTTConfig config)
        {
            this.config = config;

            hudManager = new Hud.Hud(this);
            teamManager = new Team.TeamManager(this);
            playerManager = new PlayerManager.PlayerManager(this);
            karmaManager = new Karma.KarmaManager(this);
            mapManager = new Map.MapManager(this);
            traitorTester = new TraitorTester.TraitorTester();

            SetRoundState(RoundState.WaitingForPlayers);
        }

        private void SetRoundState(RoundState state)
        {
            currentRoundState = state;

            switch (state)
            {
                case RoundState.WaitingForPlayers:
                    break;
                case RoundState.Preperation:
                    NextRoundState = DateTime.Now.AddSeconds(config.roundConfig.PreRoundDuration);
                    break;
                case RoundState.Running:
                    NextRoundState = DateTime.Now.AddSeconds(config.roundConfig.RoundDuration);
                    break;
                case RoundState.Finished:
                    NextRoundState = DateTime.Now.AddSeconds(config.roundConfig.PostRoundDuration);
                    break;
            }

        }
        private void SpawnPlayers()
        {
            foreach (Player pl in Player.GetPlayers()) // Set player models, using Roles
            {
                if (pl == null)
                {
                    continue;
                }
                if (!pl.IsAlive)
                {
                    teamManager.SetTeam(pl, Team.Team.Spectator);
                    continue;
                }
                if (!config.teamsConfig.TeamRole.ContainsKey(teamManager.GetTeam(pl)))
                {
                    Log.Debug($"Player's team {teamManager.GetTeam(pl)} does not have an ingame role set!");
                    continue;
                }
                // Player should already be spawned with correct role
                playerManager.Spawn(pl);
            }
        }
        public void Start()
        {
            Log.Debug("Setting up the round!");

            karmaManager.ReplenishKarma();
            teamManager.AssignRoles();

            SpawnPlayers();

            PluginAPI.Core.Server.FriendlyFire = true;
            SetRoundState(RoundState.Running);

            Log.Info("Started the round!");
        }

        public void End(Team.Team winningTeam = Team.Team.Spectator)
        {
            winner = winningTeam;
            SetRoundState(RoundState.Finished);
            Log.Info(config.teamsConfig.TeamName[winningTeam] + " won! Starting new round!");
        }
        public void RestartRound()
        {
            TTTUtil.RestartServer();
        }
        private void CleanupPlayerCache()
        {
            teamManager.Cleanup();
            playerManager.Cleanup();
            deathReason.Clear();
        }
        private void Cleanup_Coroutines()
        {
            if (think_task != null && think_task.IsValid)
            {
                Timing.KillCoroutines(think_task);
            }
        }
        private void Cleanup_Round()
        {
            Log.Debug("Cleaning up round!");
            SetRoundState(RoundState.Reset);
            CleanupPlayerCache();
            Cleanup_Coroutines();
        }
        private void CheckWinConditions()
        {
            List<Player> Innocents = teamManager.GetTeamPlayers(Team.Team.Innocent);
            List<Player> Detectives = teamManager.GetTeamPlayers(Team.Team.Detective);
            List<Player> Traitors = teamManager.GetTeamPlayers(Team.Team.Traitor);

            if (DateTime.Now > NextRoundState) // Stalemate
            {
                if (Innocents.Count > 0) // If Traitors fail to kill Innocents then give innocents the win!
                { // Ignore detective I guess cause they fail their job pretty bad if all the innocents die, don't they.
                    End(Team.Team.Innocent);
                    return;
                }
                End(Team.Team.Spectator);
                return;
            }
            if (Innocents.Count + Detectives.Count + Traitors.Count < 1) // Stalemate
            {
                End(Team.Team.Spectator);
                return;
            }
            if (Innocents.Count + Detectives.Count < 1)
            {
                End(Team.Team.Traitor);
                return;
            }
            if (Traitors.Count < 1)
            {
                End(Team.Team.Innocent);
                return;
            }
            
        }
      
        private IEnumerator<float> Think()
        {
            while (this.currentRoundState != RoundState.Reset) 
            {
                //System.Threading.Thread.Sleep(10);
                yield return MEC.Timing.WaitForSeconds(0.01f);
                hudManager.hudUpdate();

                // Is round finished, check if it's time to start the new round
                if (currentRoundState == RoundState.Finished)
                {
                    if (DateTime.Now > NextRoundState)
                    {
                        RestartRound();
                        break;
                    }
                    continue;
                }

                // Is round running, check the win conditions
                if (currentRoundState == RoundState.Running)
                {
                    CheckWinConditions();
                    continue;
                }

                // Is the round in preperation, check if it's time to start the round
                if (currentRoundState == RoundState.Preperation)
                {
                    if (DateTime.Now > NextRoundState)
                    {
                        Start();
                    }
                    continue;
                }
            }
        }

    }
}
