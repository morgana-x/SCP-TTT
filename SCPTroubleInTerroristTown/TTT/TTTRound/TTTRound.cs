using CommandSystem.Commands.RemoteAdmin;

using LightContainmentZoneDecontamination;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;
using PluginAPI.Core;
using static MapGeneration.ImageGenerator;
using System.Collections;
using MEC;
namespace SCPTroubleInTerroristTown.TTT
{
    public class DeathReason
    {
        public TTTRound.Team team;
        public DamageHandlerBase handler;
    }

    public partial class TTTRound
    {
        public Dictionary<Player, DeathReason> deathReason = new Dictionary<Player, DeathReason>();

        private MEC.CoroutineHandle think_task;

        public TTTConfig config;

        public TTTHud hud;

        private System.Random randomGenerator = new System.Random();

        public DateTime NextRoundState;

        public RoundState currentRoundState;
        public Team winner;
        public enum RoundState
        {
            WaitingForPlayers,
            Preperation,
            Running,
            Finished,
            Reset
        }
        public TTTRound(TTTConfig config)
        {
            this.config = config;

            hud = new TTTHud(this); // TTTHUD is just a utility for hud stuff really

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

        public void Start()
        {
            Log.Debug("Starting game!");

            SetRoundState(RoundState.Running);
            AssignRoles();
            SpawnPlayers();

            Server.FriendlyFire = true;

            Log.Info("Started the round!");
        }

        public void End(Team winningTeam = Team.Spectator)
        {
            winner = winningTeam;
            SetRoundState(RoundState.Finished);
            Log.Info(config.teamsConfig.TeamName[winningTeam] + " won! Starting new round!");
        }
        public void RestartRound()
        {
            Round.Restart(true);
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
            Cleanup_Coroutines();
        }
        private void CheckWinConditions()
        {
            List<Player> Innocents = GetTeamPlayers(Team.Innocent);
            List<Player> Detectives = GetTeamPlayers(Team.Detective);
            List<Player> Traitors = GetTeamPlayers(Team.Traitor);

            if (DateTime.Now > NextRoundState) // Stalemate
            {
                End(Team.Spectator);
                return;
            }
            if (Innocents.Count + Detectives.Count + Traitors.Count < 1) // Stalemate
            {
                End(Team.Spectator);
                return;
            }
            if (Innocents.Count + Detectives.Count < 1)
            {
                End(Team.Traitor);
                return;
            }
            if (Traitors.Count < 1)
            {
                End(Team.Innocent);
                return;
            }
            
        }
      
        private IEnumerator<float> Think()
        {
            while (this.currentRoundState != RoundState.Reset) 
            {
                //System.Threading.Thread.Sleep(10);
                yield return MEC.Timing.WaitForSeconds(0.01f);
                hudUpdate();

                // Is round finished, check if it's time to start the new round
                if (currentRoundState == RoundState.Finished)
                {
                    if (DateTime.Now > NextRoundState)
                    {
                        RestartRound();
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
