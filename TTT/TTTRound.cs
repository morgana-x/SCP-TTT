using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.Events.Commands.Reload;
using Exiled.Events.EventArgs.Player;
using LightContainmentZoneDecontamination;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using SCP_SL_Trouble_In_Terrorist_Town.TTT;
using SCP_SL_Trouble_In_Terrorist_Town.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;
using static MapGeneration.ImageGenerator;
using static SCP_SL_Trouble_In_Terrorist_Town.TTTRound;

namespace SCP_SL_Trouble_In_Terrorist_Town
{
    public class DeathReason
    {
        public TTTRound.Team team;
        public DamageHandler handler;
    }

    public class TTTRound
    {
        public Dictionary<Player, Team> playerTeams = new Dictionary<Player, Team>();
        public Dictionary<Player, Team> previousTeams = new Dictionary<Player, Team>();
        public Dictionary<Player, DateTime> spawnTimes = new Dictionary<Player, DateTime>();
        public Dictionary<Player, int> Karma = new Dictionary<Player, int>();
        public Dictionary<Player, int> OldKarma = new Dictionary<Player, int>();
        public Dictionary<Player, DeathReason> deathReason = new Dictionary<Player, DeathReason>();
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

            hud = new TTTHud(this); // TTTHUD is just a utility for hud stuff really

            SetRoundState(RoundState.WaitingForPlayers);
           
        }
        public void On_NewRound()
        {

            // Make sure old round checking logic is gone 
            Round.IsLocked = true;
            // Disable friendly fire (Until the round actually starts, nice easy way to stop people killing before round starts)
            Server.FriendlyFire = false;
            // Disable CASSIE
            Cassie.Announcer.enabled = false;
            // Disable Decontamination
            DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;

            winner = Team.Undecided;
            Exiled.API.Features.Log.Debug("Setting up round!");

            if (config.lockDownSpawnZone) // Lock down elevators to decrease play area, game gets boring if map is too large (probably) 
            {
                var zone = config.spawnZone;

                foreach (Exiled.API.Features.Room room in Exiled.API.Features.Room.List)//.Where((x) => x.Zone == zone))
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
            playerTeams.Clear();
            spawnTimes.Clear();
            deathReason.Clear();
            SetRoundState(RoundState.WaitingForPlayers);
            Round.IsLocked = false;
            Server.FriendlyFire = false; // Disable friendly fire to avoid killing before round start
            if (config.spawnDebugNPCS)
            {
                for (int i = 0; i < 12; i++)
                {
                    // Npc spawning doesn't work on Exiled 9, Version 10
                    Npc.Spawn("Bob", RoleTypeId.Spectator);
                    //TTTNPC.Spawn("Bob", RoleTypeId.Spectator);
                }
            }
        }
        public void On_Map_Loaded() // Spawning weapons logic, needs updating
        {
            Cleanup_Round();
            //TTTWeaponSpawner.SpawnWeapons(TTTWeaponSpawner.tempWeaponList);
            TTTWeaponSpawner.SpawnRandomWeapons(config.spawnZone);
        }
        public void On_Player_Leave(Player player)
        {
            if (playerTeams.ContainsKey(player))
                playerTeams.Remove(player);
            if (previousTeams.ContainsKey(player)) // Needed for corpse info currently
                previousTeams.Remove(player);
            if (spawnTimes.ContainsKey(player))
                spawnTimes.Remove(player);
            if (deathReason.ContainsKey(player))
                deathReason.Remove(player);

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
            if (currentRoundState != RoundState.WaitingForPlayers)
            {
                SetTeam(player, Team.Spectator);
                Spawn(player, spawnPoint: config.spawnPoint);
            }
        }

        public void StartNextRound()
        {
            SetRoundState(RoundState.Reset);
            Log.Debug("Restarting round!");
            Cleanup_Round();
            //Exiled.API.Features.Round.EndRound(true, true, );
            foreach(Player pl in Player.List)
            {
                if (pl.IsNPC)
                {
                    pl.Kick(""); // Kick NPCs, they cause server to mess up!
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

       
        private void AssignRoles() // Semi-Port of the original GMOD function
        {
            Log.Debug("Assigning roles!");
            List<Player> remainingPlayers = Player.List.ToList();
            TTTUtil.RandomShuffle(remainingPlayers); // This is redundant but oh well!

            foreach (Player pl in remainingPlayers)
            {
                SetTeam(pl, Team.Innocent, true);
            }

            if (remainingPlayers.Count == 0)
            {
                return;
            }

            // Get target numbers of Traitors/Detectives
            int targetNumOfTraitors = GetTraitorCount(remainingPlayers.Count);
            int targetNumofDetectives = GetDetectiveCount(remainingPlayers.Count);

            int numTerrorists = 0;
            int numDetectives = 0;



            // Assign Traitors
            while (numTerrorists < targetNumOfTraitors)
            {
                Player pick = remainingPlayers.RandomItem(); // No need to shuffle when you can do this!
                if (pick.IsVerified && (!(previousTeams.ContainsKey(pick) && previousTeams[pick] == Team.Traitor) || (randomGenerator.Next(3) == 2)))
                {
                    remainingPlayers.Remove(pick);
                    SetTeam(pick, Team.Traitor,true);
                    numTerrorists++;
                }
            }

            // Assign Detectives
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
                if (GetKarma(pick) > config.teamsConfig.DetectiveMinKarma) // Karma and player choices later
                {
                    SetTeam(pick, Team.Detective, true);
                    numDetectives++;
                }
                remainingPlayers.Remove(pick);
            }


            foreach(Player pl in Player.List)
            {
                setSpawnTime(pl); // Make sure their spawn text is shown!
            }

            Log.Debug("Finished Assigned roles");
        }
        public void OnPlayerSpawned(Player pl)
        {
            UpdateOldKarma(pl);
        }
        public void OnPlayerHurt(Player victim, Player attacker, DamageType damageType, DamageHandler handler)
        {
            // Causes crashes with npcs, yay?!?! Might be to do with EXILED version mismatch with current setup
            if (attacker != null) // Karma checks
            {
                if (GetTeam(victim) == GetTeam(attacker) || (GetTeam(victim) == Team.Innocent && GetTeam(attacker) == Team.Detective) || (GetTeam(victim) == Team.Detective && GetTeam(attacker) == Team.Innocent))
                {
                    AddKarma(attacker, -1); // Lose karma for killing same team
                }
            }
        }
        public void OnPlayerDeath(Player victim, Player attacker, DamageHandler handler)
        {
           
            if (attacker != null) // Karma checks
            {
                Team victimTeam = GetTeam(victim);
                Team attackerTeam = GetTeam(attacker);
                if (victimTeam == GetTeam(attacker) || (victimTeam == Team.Innocent && attackerTeam == Team.Detective) || (victimTeam == Team.Detective && attackerTeam == Team.Innocent) )
                {
                    AddKarma(attacker, -5); // Lose karma for killing same team
                }
                else if (victimTeam == Team.Traitor)
                {
                    AddKarma(attacker, 5); // Gain less karma for killing correct team as traitor as that's easy
                }
                else
                {
                    AddKarma(attacker, 10); // Gain karma for killing correct team
                }
            }
            SetTeam(victim, Team.Spectator, false);

            setDeathReason(victim, handler);
        }
        private void setDeathReason(Player victim, DamageHandler handler)
        {
            DeathReason reason = new DeathReason()
            {
                team = GetTeam(victim),
                handler = handler
            };
            if (!deathReason.ContainsKey(victim))
            {
                deathReason.Add(victim, reason);
            }
            else
            {
                deathReason[victim] = reason;
            }
            Log.Debug("Set death reason for " + victim.DisplayNickname + "\nDamagetype: " + handler.Type.ToString());
        }
        private DeathReason getDeathReason(Player victim)
        {
            if (!deathReason.ContainsKey(victim))
            {
                return null;
            }
            return deathReason[victim];
        }

        private void JankSetRole(Player pl, RoleTypeId role)
        {
            Server.ExecuteCommand("forceclass " + pl.Id + " " + role.ToString() + " 0", new RemoteAdmin.PlayerCommandSender(ReferenceHub.HostHub));
        }
        public void Spawn(Player pl, SpawnReason reason = SpawnReason.ForceClass, RoleTypeId spawnPoint = RoleTypeId.None, bool dontRespawn=false)
        {
            var plTeam = GetTeam(pl);
            GiveLoadout(pl);

            if (!pl.IsNPC)
            {
                Log.Debug("Spawning player " + pl.DisplayNickname);
            }
            RoleTypeId role = config.teamsConfig.TeamRole[GetTeam(pl)];


            if (pl.Role.Type == role)
            {
                return;
            }

            //pl.RoleManager.ServerSetRole(role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
           // JankSetRole(pl, role);
            pl.Role.Set(role, RoleSpawnFlags.None);

            if (spawnPoint != RoleTypeId.None && !dontRespawn)
            {
                pl.Teleport(spawnPoint.GetRandomSpawnLocation());
            }
            if (!pl.IsNPC)
            {
                Log.Debug("Set role and teleported!");
            }

            if (pl.IsNPC) // NPC Jank
            {
                pl.Health = 100;
            }
      
            setSpawnTime(pl);
            if (!pl.IsNPC)
            {
                Log.Debug("Spawned " + pl.DisplayNickname);
            }

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

                foreach (Player pl in Player.List) // Set player models, using Roles
                {
                    try
                    {
                        // Player should already be spawned with correct role
                        Spawn(pl, dontRespawn:true); 
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

            // Hint HUD now displays winner, no need to broadcast it.
            /*string winText = config.teamsConfig.TeamWinText[winningTeam].Replace("{TeamColor}", config.teamsConfig.TeamColor[winningTeam]);
            foreach (Player pl in Player.List)
            {
                if (pl.IsNPC)
                {
                    continue;
                }
                pl.Broadcast((ushort)config.roundConfig.PostRoundDuration, winText, shouldClearPrevious:true);
            }*/

            Log.Info(config.teamsConfig.TeamName[winningTeam] + " won! Starting new round!");
            SetRoundState(RoundState.Finished);
        }
        public PlayerStatsSystem.DamageHandlerBase OnSpawnedCorpse(Player player, PlayerStatsSystem.DamageHandlerBase damageHandler, string deathReason)
        {

            PlayerStatsSystem.DamageHandlerBase baseHandler = new PlayerStatsSystem.CustomReasonDamageHandler(TTTCorpse.GetCorpseInfo(config, player, previousTeams[player], damageHandler, deathReason));
            return baseHandler;

            // Todo: Add "They were a traitor! etc". I forgot how to do this!
            // Also need to probably fakesendvar it for detectives having extra info etc... Yay...
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
        DateTime nextHudShow = DateTime.Now;

        private void hudUpdate() // Todo: Optimise this
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

                // Is round finished, check if it's time to start the new round
                if (currentRoundState == RoundState.Finished)
                {
                    if (DateTime.Now > NextRoundState)
                    {
                        StartNextRound();
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

        private List<ItemType> GetLoadout(Team team)
        {
            if (!config.teamsConfig.TeamLoadout.ContainsKey(team))
            {
                return new List<ItemType> { };
            }
            return config.teamsConfig.TeamLoadout[team];
        }
        private void GiveLoadoutItems(Player pl, List<ItemType> items)
        {
            foreach (var i in items)
            {
                if (pl == null)
                {
                    Log.Debug("Player became null? (Loadout  Error)");
                    break;
                }
                if (i == null)
                {
                    Log.Debug("Attempting to give null item to a player, abort");
                    continue;
                }
                if (i.IsAmmo())
                {
                    pl.AddAmmo(i.GetAmmoType(), 100);
                    continue;
                }
                pl.AddItem(i);
            }
        }
        public void GiveLoadout(Player pl, bool clearInventory=false)
        {
            if (pl == null)
            {
                return;
            }
            if (!Player.List.Contains(pl))
            {
                return;
            }
            if (clearInventory)
            {
                pl.ClearInventory(true);
            }
            Team team = GetTeam(pl);
            if (team == Team.Spectator)
            {
                return;
            }
            GiveLoadoutItems(pl, GetLoadout(team));
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

        public void SetKarma(Player pl, int karma)
        {
            if (!Karma.ContainsKey(pl)) { Karma.Add(pl, karma); return; }
            Karma[pl] = karma;
        }
        public int GetKarma(Player pl)
        {
            if (!Karma.ContainsKey(pl))
            {
                SetKarma(pl, 0);
                return 0;
            }
            return Karma[pl];
        }
        public int GetOldKarma(Player pl)
        {
            if (!OldKarma.ContainsKey(pl))
            {
                OldKarma.Add(pl, GetKarma(pl));
            }
            return OldKarma[pl];
        }
        private void UpdateOldKarma(Player pl)
        {
            Log.Debug("Updating " + pl.DisplayNickname + "'s old karma!");
            if (!OldKarma.ContainsKey(pl))
            {
                OldKarma.Add(pl, GetKarma(pl));
                return;
            }
            OldKarma[pl] = GetKarma(pl);
        }
        public void AddKarma(Player pl, int amount)
        {
            if (!Karma.ContainsKey(pl))
            {
                SetKarma(pl, amount);
                return;
            }
            SetKarma(pl, GetKarma(pl) + amount);
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
        




        private int GetTraitorCount(int numberOfPlayers) // From gmod function
        {
            int traitorCount = (int)Math.Floor(numberOfPlayers * config.teamsConfig.TraitorPercentage);
            traitorCount = Math.Min(traitorCount, config.teamsConfig.TraitorMax);
            return traitorCount;
        }
        private int GetDetectiveCount(int numberOfPlayers) // From gmod function
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
