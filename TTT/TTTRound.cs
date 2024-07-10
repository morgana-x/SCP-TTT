using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using SCP_SL_Trouble_In_Terrorist_Town.TTT;
using SCP_SL_Trouble_In_Terrorist_Town.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCP_SL_Trouble_In_Terrorist_Town
{
    public class TTTRound
    {
        public Dictionary<Player, Team> playerTeams = new Dictionary<Player, Team>();
        public Dictionary<Player, Team> previousTeams = new Dictionary<Player, Team>();
        public Dictionary<Player, DateTime> spawnTimes = new Dictionary<Player, DateTime>();

        private Task think_task;

        public TTTConfig config;

        public TTTHud hud;

        public System.Random randomGenerator = new System.Random();

        public DateTime NextRoundState; // Todo! Stalemate variable

        public RoundState currentRoundState;
        public Team winner;

        public TTTRound(TTTConfig config)
        {
            this.config = config;
            hud = new TTTHud(this);
            SetRoundState(RoundState.WaitingForPlayers);
           
        }
        public void On_NewRound()
        {

            Round.IsLocked = true;
            Server.FriendlyFire = false;
            Cassie.Announcer.enabled = false;
            winner = Team.Undecided;
            Exiled.API.Features.Log.Debug("Setting up round!");

            if (config.lockDownSpawnZone)
            {
                var zone = config.spawnZone;

                foreach (Room room in Room.List)//.Where((x) => x.Zone == zone))
                {
                    foreach (var d in (room.Doors.Where((x) => x.IsElevator)))
                    {
                        d.ChangeLock(DoorLockType.AdminCommand);
                    }
                }
            }


            SetRoundState(RoundState.Preperation);
       
            foreach (Player pl in Player.List)
            {
                SetTeam(pl, Team.Undecided);
                Spawn(pl, spawnPoint:config.spawnPoint) ;
            }
            think_task = Task.Run(Think);
        }
        public void On_Round_Restarting()
        {
            Cleanup_Round();
        }
        public void On_Waiting_For_Players()
        {
            Cleanup_Round();
            SetRoundState(RoundState.WaitingForPlayers);
            Round.IsLocked = false;
            Server.FriendlyFire = false;
            if (config.spawnDebugNPCS)
            {
                for (int i = 0; i < 29; i++)
                {
                    Npc.Spawn("Bob", RoleTypeId.Spectator);
                }
            }
        }
        public void On_Map_Loaded()
        {
            Cleanup_Round();
            TTTWeaponSpawner.SpawnWeapons(TTTWeaponSpawner.tempWeaponList);
        }
        public void On_Player_Leave(Player player)
        {
            if (playerTeams.ContainsKey(player))
                playerTeams.Remove(player);
            if (previousTeams.ContainsKey(player))
                previousTeams.Remove(player);
            if (spawnTimes.ContainsKey(player))
                spawnTimes.Remove(player);

            hud.RemovePlayer(player);
        }
        public void On_Player_Joined(Player player)
        {
            if (currentRoundState == RoundState.Preperation)
            {
                SetTeam(player, Team.Undecided);
                Spawn(player, spawnPoint: config.spawnPoint);
                return;
            }
            SetTeam(player, Team.Spectator);
            Spawn(player);
        }

        public void StartNextRound()
        {
            SetRoundState(RoundState.Reset); // Set this or server crashes, yo!
            Log.Debug("Restarting round!");
            Cleanup_Round();
            //Exiled.API.Features.Round.EndRound(true, true, );
            foreach(Player pl in Player.List)
            {
                if (pl.IsNPC)
                {
                    pl.Kick("");
                }
            }
            Round.Restart(true);
        }

        private void Cleanup_Round()
        {
            Log.Debug("Cleaning up round!");
            SetRoundState(RoundState.Reset);
            if (think_task != null && think_task.Status == TaskStatus.Running)
            {
                think_task.Dispose();
            }
        }
        private void SetRoundState(RoundState state)
        {
            currentRoundState = state;

            switch (state) 
            {
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

       
        private void AssignRoles()
        {
            Log.Debug("Assigning roles!");
            List<Player> remainingPlayers = Player.List.ToList();
            TTTUtil.RandomShuffle(remainingPlayers);

            foreach (Player pl in remainingPlayers)
            {
                SetTeam(pl, Team.Innocent);
            }
            if (remainingPlayers.Count == 0)
            {
                return;
            }
            int targetNumOfTraitors = GetTraitorCount(remainingPlayers.Count);
            int targetNumofDetectives = GetDetectiveCount(remainingPlayers.Count);

            int numTerrorists = 0;
            int numDetectives = 0;
            // Assign Traitors
            while (numTerrorists < targetNumOfTraitors)
            {
                Player pick = remainingPlayers.RandomItem();
                if (pick.IsVerified && (!(previousTeams.ContainsKey(pick) && previousTeams[pick] == Team.Traitor) || (randomGenerator.Next(3) == 2)))
                {
                    remainingPlayers.Remove(pick);
                    SetTeam(pick, Team.Traitor,true);
                    numTerrorists++;
                }
            }
            while (numDetectives < targetNumofDetectives)
            {
                if (remainingPlayers.Count <= targetNumofDetectives - numDetectives)
                {
                    foreach(Player pl in remainingPlayers)
                    {
                        if (!pl.IsVerified)
                        {
                            continue;
                        }
                        SetTeam(pl, Team.Detective,true);
                    }
                    break;
                }
                Player pick = remainingPlayers.RandomItem();
                if (true) // Karma and player choices later
                {
                    SetTeam(pick, Team.Detective, true);
                    numDetectives++;
                }
                remainingPlayers.Remove(pick);
            }
            Log.Debug("Finished Assigned roles");
        }
        public void OnPlayerSpawned(Player pl)
        {

        }
        public void OnPlayerHurt(Player victim, Player attacker, DamageType damageType)
        { 
        }
        public void OnPlayerDeath(Player victim, Player attacker)
        {
            SetTeam(victim, Team.Spectator, false);
        }
        public void Spawn(Player pl, SpawnReason reason = SpawnReason.ForceClass, RoleTypeId spawnPoint = RoleTypeId.None)
        {
            Log.Debug("Spawning player " + pl.DisplayNickname);
            RoleTypeId role = config.teamsConfig.TeamRole[GetTeam(pl)];

            if (pl.Role.Type == role)
            {
                return;
            }
     
            pl.Role.Set(role, reason, RoleSpawnFlags.None);

            if (spawnPoint != RoleTypeId.None)
            {
                pl.Teleport(spawnPoint.GetRandomSpawnLocation());
            }

            Log.Debug("Set role and teleported!");

            if (pl.IsNPC) // NPC Jank
            {
                pl.Health = 100;
            }
      
            setSpawnTime(pl);

            Log.Debug("Spawned " + pl.DisplayNickname);

        }
        private DateTime getSpawnTime(Player pl)
        {
            if (!spawnTimes.ContainsKey(pl))
            {
                spawnTimes.Add(pl, DateTime.Now);
            }
            return spawnTimes[pl];
        }
        private void setSpawnTime(Player pl)
        {
            if (!spawnTimes.ContainsKey(pl))
            {
                spawnTimes.Add(pl, DateTime.Now);
                return;
            }
            spawnTimes[pl] = DateTime.Now;
        }
        public void Start()
        {
            Log.Debug("Starting game!");
            SetRoundState(RoundState.Running);
            try
            {
                AssignRoles();

                foreach (Player pl in Player.List) //Should already be spawned, I guess?
                {
                    if (pl.IsNPC) // This silent restarts server for some reason! If npc
                    {
                        continue;
                    }
                    try
                    {
                        // Player should already be spawned with correct role
                            Spawn(pl, spawnPoint: config.spawnPoint); 
                    }
                    catch(Exception e)
                    {
                        Log.Debug("Error occured while spawning player!");
                        Log.Debug(e.ToString());
                    }
                }

                Log.Debug("Spawned players!");
                Log.Debug("Set game state");
            }
            catch(Exception e)
            {
                Log.Debug("Error occured while Starting game!");
                Log.Debug(e.ToString());
            }
            Server.FriendlyFire = true;
            Log.Debug("Started Game!");
         

        }

        public void End(Team winningTeam = Team.Spectator)
        {
            winner = winningTeam;
            string winText = config.teamsConfig.TeamWinText[winningTeam].Replace("{TeamColor}", config.teamsConfig.TeamColor[winningTeam]);
            foreach (Player pl in Player.List)
            {
                if (pl.IsNPC)
                {
                    continue;
                }
                pl.Broadcast((ushort)config.roundConfig.PostRoundDuration, winText, shouldClearPrevious:true);
            }
            Log.Info(config.teamsConfig.TeamName[winningTeam] + " won! Starting new round!");
            SetRoundState(RoundState.Finished);
        }
        public void OnSpawnedCorpse(Player player, Ragdoll ragdoll, RagdollData info)
        {
            // Todo: Add "They were a traitor! etc"
            //ragdoll.DamageHandler. += TTTCorpse.GetCorpseInfo(config, player, previousTeams[player]);
        }
        private void CheckWinConditions()
        {
            List<Player> Innocents = GetTeamPlayers(Team.Innocent);
            List<Player> Detectives = GetTeamPlayers(Team.Detective);
            List<Player> Traitors = GetTeamPlayers(Team.Traitor);

            // Sorry some guy that hates when someone uses if return chains instead of if else etc
            // I just like to read code :pensive:
            // I say after making this ginormously sized class

            if (DateTime.Now > NextRoundState) // Todo add stalemate after time limit!
            {
                End(Team.Spectator);
                return;
            }
            if (Innocents.Count + Detectives.Count + Traitors.Count < 1)
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
        DateTime nextHudShow = DateTime.Now;

        private void hudUpdate()
        {
            if (DateTime.Now < nextHudShow)
            {
                return;
            }
            nextHudShow = DateTime.Now.AddSeconds(1.1);

            foreach (Player pl in Player.List)
            {
                if (pl.IsNPC || (!playerTeams.ContainsKey(pl)))
                {
                    continue;
                }
                //float ping = (pl.Ping) + 0.1f;
                //Math.Min(ping, 1.9f); // 1.8 is the max I get on satelitte on AU servers, so I sure hope noone gets worse than this!
                hud.ShowHud(pl, DateTime.Now.Subtract(getSpawnTime(pl)).TotalSeconds < config.hudConfig.ShowCustomSpawnMessageDuration, 1.3f);
            }
        }
        private async Task Think()
        {
            while (this.currentRoundState != RoundState.Reset) 
            {
                System.Threading.Thread.Sleep(10);
                
                hudUpdate();

                if (currentRoundState == RoundState.Finished)
                {
                    if (DateTime.Now > NextRoundState)
                    {
                        StartNextRound();
                    }
                    continue;
                }
                if (currentRoundState == RoundState.Running)
                {
                    CheckWinConditions();
                    continue;
                }

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
        public void SetTeam(Player pl, Team team)
        {
            SetTeam(pl, team, false);
        }
        public Team GetTeam(Player pl)
        {
            if (!playerTeams.ContainsKey(pl))
            {
                SetTeam(pl, Team.Spectator);
            }
            return playerTeams[pl];
        }
        public void SetTeam(Player pl, Team team, bool SetPreviousTeam=true)
        {
            if (!playerTeams.ContainsKey(pl))
            {
                if (!previousTeams.ContainsKey(pl))
                {
                    previousTeams.Add(pl, team);
                }
                playerTeams.Add(pl, team);
                return;
            }
            if (SetPreviousTeam)
            {
                if (!previousTeams.ContainsKey(pl))
                {
                    previousTeams.Add(pl, team);
                }
                else
                {
                    previousTeams[pl] = team;
                }
            }
            playerTeams[pl] = team;
        }
        public List<Player> GetTeamPlayers(Team team)
        {
           return playerTeams.Keys.Where((x) => (GetTeam(x) == team)).ToList();
        }
        public enum Team
        {
            Innocent,
            Traitor,
            Detective,
            Spectator,
            Undecided
        }
        public enum RoundState
        {
            WaitingForPlayers,
            Preperation,
            Running,
            Finished,
            Reset
        }





        private int GetTraitorCount(int numberOfPlayers)
        {
            int traitorCount = (int)Math.Floor(numberOfPlayers * config.teamsConfig.TraitorPercentage);
            traitorCount = Math.Min(traitorCount, config.teamsConfig.TraitorMax);
            return traitorCount;
        }
        private int GetDetectiveCount(int numberOfPlayers) 
        {
            if (numberOfPlayers < config.teamsConfig.DetectiveMin)
            {
                return 0;
            }
            int detectiveCount = (int)Math.Floor(numberOfPlayers * config.teamsConfig.DetectivePercentage);
            detectiveCount = Math.Min(detectiveCount, config.teamsConfig.DetectiveMax);
            return detectiveCount;
        }


    }
}
