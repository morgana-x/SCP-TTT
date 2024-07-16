using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Configs;
using LightContainmentZoneDecontamination;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Enums;

namespace SCPTroubleInTerroristTown.TTT
{
    public partial class TTTRound
    {
        public void On_Map_Loaded() // Spawning weapons logic, needs updating
        {
            Cleanup_Round();
            TTTWeaponSpawner.SpawnRandomWeapons(config.spawnZone);
        }
        public void On_Player_Leave(Player player)
        {
            if (playerTeams.ContainsKey(player))
                playerTeams.Remove(player);
            //if (previousTeams.ContainsKey(player)) // Needed for corpse info currently
            //    previousTeams.Remove(player);
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

        public void On_NewRound()
        {

            // Make sure old round checking logic is gone 
            Round.IsLocked = true;
            // Disable friendly fire (Until the round actually starts, nice easy way to stop people killing before round starts)
            Server.FriendlyFire = false;
            // Disable CASSIE
            //Cassie.Announcer.enabled = false;
            // Disable Decontamination
            DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;

            winner = Team.Undecided;
            Log.Debug("Setting up round!");

            if (config.lockDownSpawnZone) // Lock down elevators to decrease play area, game gets boring if map is too large (probably) 
            {
                TTTUtil.LockdownZones();
            }


            SetRoundState(RoundState.Preperation);

            foreach (Player pl in Player.GetPlayers())
            {
                SetTeam(pl, Team.Undecided);
                Spawn(pl, spawnPoint: config.spawnPoint);
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
            playerTeams.Clear();
            spawnTimes.Clear();
            deathReason.Clear();
            SetRoundState(RoundState.WaitingForPlayers);
            Round.IsLocked = true;
            Server.FriendlyFire = false; // Disable friendly fire to avoid killing before round start
            InventoryLimits.StandardCategoryLimits[ItemCategory.Firearm] = 2;
            if (config.spawnDebugNPCS)
            {
                int numOfNpcs = 12;
                for (int i = 0; i < numOfNpcs; i++)
                {
                    // Npc spawning doesn't work on Exiled 9, Version 10
                    var n = TTTNPC.Spawn("Bob", RoleTypeId.Spectator);

                }
                Log.Info($"Spawned {numOfNpcs} npcs!");
            }
        }
        public void OnPlayerSpawned(Player pl)
        {
            UpdateOldKarma(pl);
        }
        public void OnPlayerHurt(Player victim, Player attacker, DamageHandlerBase damageType)
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
        public void OnPlayerDeath(Player victim, Player attacker)
        {

            if (attacker != null) // Karma checks
            {
                Team victimTeam = GetTeam(victim);
                Team attackerTeam = GetTeam(attacker);
                if (victimTeam == GetTeam(attacker) || (victimTeam == Team.Innocent && attackerTeam == Team.Detective) || (victimTeam == Team.Detective && attackerTeam == Team.Innocent))
                {
                    AddKarma(attacker, -4); // Lose karma for killing same team
                }
                else if (victimTeam == Team.Traitor)
                {
                    AddKarma(attacker, 2); // Gain less karma for killing correct team as traitor as that's easy
                }
                else
                {
                    AddKarma(attacker, 4); // Gain karma for killing correct team
                }
            }
            SetTeam(victim, Team.Spectator, false);

            //setDeathReason(victim, handler);
        }
        public PlayerStatsSystem.DamageHandlerBase OnSpawnedCorpse(Player player, PlayerStatsSystem.DamageHandlerBase damageHandler, string deathReason)
        {
            PlayerStatsSystem.DamageHandlerBase baseHandler = new PlayerStatsSystem.CustomReasonDamageHandler(Corpse.TTTCorpse.GetCorpseInfo(config, player, previousTeams[player], damageHandler, deathReason));
            return baseHandler;

            // Todo: Add "They were a traitor! etc". I forgot how to do this!
            // Also need to probably fakesendvar it for detectives having extra info etc... Yay...
            //ragdoll.DamageHandler. += TTTCorpse.GetCorpseInfo(config, player, previousTeams[player]);
        }
    }
}
