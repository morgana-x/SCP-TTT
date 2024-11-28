using PlayerRoles;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using PluginAPI.Core;
using SCPTroubleInTerroristTown.TTT.Util;
using System;

namespace SCPTroubleInTerroristTown.TTT.Round
{

    public partial class Round
    {
        public void On_Map_Loaded() // Spawning weapons logic, needs updating
        {
            Cleanup_Round();
            mapManager.onMapLoaded();
            traitorTester.Init();
        }
        public void On_Player_Leave(PluginAPI.Core.Player player)
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
        public void On_Player_Joined(PluginAPI.Core.Player player)
        {
            karmaManager.SetupKarma(player);
            if (currentRoundState == RoundState.Preperation)
            {
                if (!karmaManager.AllowedSpawnKarmaCheck(player))
                {
                    teamManager.SetTeam(player, Team.Team.Spectator);
                    playerManager.Spawn(player, spawnPoint: config.mapConfig.spawnPoint);
                    return;
                }
                teamManager.SetTeam(player, Team.Team.Undecided);
                playerManager.Spawn(player, spawnPoint: config.mapConfig.spawnPoint);
                return;
            }
            if (currentRoundState != RoundState.WaitingForPlayers)
            {
                teamManager.SetTeam(player, Team.Team.Spectator);
                playerManager.Spawn(player, spawnPoint: config.mapConfig.spawnPoint);
                return;
            }
        }

        public void On_NewRound()
        {
            SetRoundState(RoundState.Preperation);
        }
        public void On_Round_Restarting()
        {
            SetRoundState(RoundState.Reset);
        }
        public void On_Waiting_For_Players()
        {
            SetRoundState(RoundState.WaitingForPlayers);

            if (config.spawnDebugNPCS)
                NPC.SpawnNpcs(12);
        }
        public void OnPlayerSpawned(PluginAPI.Core.Player pl)
        {
            if (pl == null)
                return;
            try
            {
                karmaManager.UpdateOldKarma(pl);
                playerManager.badgeManager.SyncPlayer(pl);
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
            }
        }
        public void OnPlayerHurt(PluginAPI.Core.Player victim, PluginAPI.Core.Player attacker, DamageHandlerBase damageType)
        {
        }
        public void OnPlayerDeath(PluginAPI.Core.Player victim, PluginAPI.Core.Player attacker, DamageHandlerBase damageBase)
        {
            teamManager.SetTeam(victim, Team.Team.Spectator, false, true);
            //playerManager.badgeManager.SyncPlayer(victim);
            if (attacker != null)
            {

                if (attacker != victim)
                {
                   // creditManager.AddCredits(attacker, config.creditConfig.KillAwardCredit);
                    karmaManager.KarmaPunishCheck(victim, attacker);
                }
                awardManager.OnPlayerKill(victim.DisplayNickname, teamManager.GetPreviousTeam(victim), attacker.DisplayNickname, teamManager.GetPreviousTeam(attacker), Util.Util.getDamageTypeFromHandler(damageBase));
            }
            else
            {
                awardManager.OnPlayerKill(victim.DisplayNickname, teamManager.GetPreviousTeam(victim), victim.DisplayNickname, teamManager.GetPreviousTeam(victim), Util.Util.getDamageTypeFromHandler(damageBase));
            }
        }
        public void OnPlayerChangeRole(PluginAPI.Core.Player victim, RoleTypeId newrole, RoleChangeReason reason)
        {
            playerManager.badgeManager.SyncPlayer(victim);
            if (reason == RoleChangeReason.Respawn || reason == RoleChangeReason.RoundStart)
            {
                return;
            }
            if ((newrole == RoleTypeId.Spectator || newrole == RoleTypeId.Tutorial) && reason == RoleChangeReason.RemoteAdmin)
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
        public bool Scp914Activated(PluginAPI.Core.Player player)
        {
            return traitorTester.shouldActivate(this, player);
        }
        public void Scp914ProcessPlayer(PluginAPI.Core.Player player)
        {
            traitorTester.ProcessPlayer(this, player);
        }
        public void OnSpawnedCorpse(ReferenceHub hub, BasicRagdoll ragdoll)
        {
            corpseManager.OnCorpseSpawn(hub, ragdoll);
        }

        public void OnPlayerToggleNoclip(ReferenceHub hub)
        {
            corpseManager.OnCorpseDiscoverHotKey(Player.Get(hub));
        }
    }
}