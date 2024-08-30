using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Configs;
using JetBrains.Annotations;
using LightContainmentZoneDecontamination;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Enums;

namespace SCPTroubleInTerroristTown.TTT
{
    public partial class Round
    {
        TraitorTester.TraitorTester traitorTester = new TraitorTester.TraitorTester();
        public void On_Map_Loaded() // Spawning weapons logic, needs updating
        {
            Cleanup_Round();
            TTTWeaponSpawner.SpawnRandomWeapons(config.spawnZone);
            traitorTester.Init();
        }
        public void On_Player_Leave(Player player)
        {
            if (teamManager.playerTeams.ContainsKey(player))
                teamManager.playerTeams.Remove(player);
            //if (previousTeams.ContainsKey(player)) // Needed for corpse info currently
            //    previousTeams.Remove(player);
            if (playerManager.spawnTimes.ContainsKey(player))
                playerManager.spawnTimes.Remove(player);
            if (deathReason.ContainsKey(player))
                deathReason.Remove(player);

            if (playerManager.badgeManager.badgeOptOuted.Contains(player))
                playerManager.badgeManager.badgeOptOuted.Remove(player);

            hudManager.RemovePlayer(player);
        }
        public void On_Player_Joined(Player player)
        {
            karmaManager.SetupKarma(player);
            if (currentRoundState == RoundState.Preperation)
            {
                teamManager.SetTeam(player, Team.Team.Undecided);
                playerManager.Spawn(player, spawnPoint: config.spawnPoint);
                return;
            }
            if (currentRoundState != RoundState.WaitingForPlayers)
            {
                teamManager.SetTeam(player, Team.Team.Spectator);
                playerManager.Spawn(player, spawnPoint: config.spawnPoint);
                return;
            }
        }

        public void On_NewRound()
        {

            // Make sure old round checking logic is gone 
            PluginAPI.Core.Round.IsLocked = true;
            // Disable friendly fire (Until the round actually starts, nice easy way to stop people killing before round starts)
            Server.FriendlyFire = false;
            // Disable CASSIE
            //Cassie.Announcer.enabled = false;
            // Disable Decontamination
            DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;

            winner = Team.Team.Undecided;
            Log.Debug("Setting up round!");

            if (config.lockDownSpawnZone) // Lock down elevators to decrease play area, game gets boring if map is too large (probably) 
            {
                TTTUtil.LockdownZones();
            }


            SetRoundState(RoundState.Preperation);

            foreach (Player pl in Player.GetPlayers())
            {
                teamManager.SetTeam(pl, Team.Team.Undecided);
                playerManager.Spawn(pl, spawnPoint: config.spawnPoint);
            }

            Cleanup_Coroutines();
            think_task = Timing.RunCoroutine(Think());
        }
        public void On_Round_Restarting()
        {
            Log.Debug("Restarting round!");


            SetRoundState(RoundState.Reset);
            Cleanup_Round();

        }
        public void On_Waiting_For_Players()
        {
            Cleanup_Round();
            teamManager.playerTeams.Clear();
            playerManager.spawnTimes.Clear();
            deathReason.Clear();
            SetRoundState(RoundState.WaitingForPlayers);
            PluginAPI.Core.Round.IsLocked = true;
            Server.FriendlyFire = false; // Disable friendly fire to avoid killing before round start
            InventoryLimits.StandardCategoryLimits[ItemCategory.Firearm] = 2;
            if (config.spawnDebugNPCS)
            {
                int numOfNpcs = 12;
                for (int i = 0; i < numOfNpcs; i++)
                {
                    var n = TTTNPC.Spawn("Bob", RoleTypeId.Spectator);
                }
                Log.Info($"Spawned {numOfNpcs} npcs!");
            }
        }
        public void OnPlayerSpawned(Player pl)
        {
            karmaManager.UpdateOldKarma(pl);
            playerManager.badgeManager.SyncPlayer(pl);
        }
        public void OnPlayerHurt(Player victim, Player attacker, DamageHandlerBase damageType)
        {
            if (attacker != null) // Karma checks
            {
                Team.Team vicTeam = teamManager.GetTeam(victim);
                Team.Team atackTeam = teamManager.GetTeam(attacker);
                if (vicTeam == atackTeam || 
                    (vicTeam == Team.Team.Innocent && atackTeam == Team.Team.Detective) || 
                    (vicTeam == Team.Team.Detective && atackTeam == Team.Team.Innocent))
                {
                    karmaManager.AddKarma(attacker, -1); // Lose karma for killing same team
                }
            }
        }
        public void OnPlayerDeath(Player victim, Player attacker)
        {
            teamManager.SetTeam(victim, Team.Team.Spectator, false);
            playerManager.badgeManager.SyncPlayer(victim);
            if (attacker != null) // Karma checks
            {
                karmaManager.KarmaPunishCheck(victim, attacker);
            }
        }
        public void OnPlayerChangeRole(Player victim, RoleTypeId newrole, RoleChangeReason reason)
        {
            playerManager.badgeManager.SyncPlayer(victim);
            if (reason == RoleChangeReason.Respawn || reason == RoleChangeReason.RoundStart)
            {
                return;
            }
            if ( (newrole == RoleTypeId.Spectator || newrole == RoleTypeId.Tutorial) && reason == RoleChangeReason.RemoteAdmin)
            {
                teamManager.SetTeam(victim, Team.Team.Spectator, false);
                return;
            }
            /*
            if (reason == RoleChangeReason.RemoteAdmin && config.teamsConfig.TeamRole[Team.Team.Innocent] == newrole)
            {
                teamManager.SetTeam(victim, Team.Team.Innocent, true);
                playerManager.badgeManager.SyncPlayer(victim);
                return;
            }*/
        }
        public bool Scp914Activated(Player player)
        {
            return traitorTester.shouldActivate(this, player);
        }
        public void Scp914ProcessPlayer(Player player)
        {
            traitorTester.ProcessPlayer(this, player);
        }
        public PlayerStatsSystem.DamageHandlerBase OnSpawnedCorpse(Player player, PlayerStatsSystem.DamageHandlerBase damageHandler, string deathReason)
        {
            PlayerStatsSystem.DamageHandlerBase baseHandler = new PlayerStatsSystem.CustomReasonDamageHandler(Corpse.Corpse.GetCorpseInfo(config, player, teamManager.previousTeams[player], damageHandler, deathReason));
            return baseHandler;
        }
    }
}
