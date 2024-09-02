using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using System;

namespace SCPTroubleInTerroristTown.TTT
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
                TTTNPC.SpawnNpcs(12);
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
            catch(Exception e)
            {
                Log.Debug(e.ToString());    
            }
        }
        public void OnPlayerHurt(PluginAPI.Core.Player victim, PluginAPI.Core.Player attacker, DamageHandlerBase damageType)
        {
        }
        public void OnPlayerDeath(PluginAPI.Core.Player victim, PluginAPI.Core.Player attacker)
        {
            teamManager.SetTeam(victim, Team.Team.Spectator, false);
            playerManager.badgeManager.SyncPlayer(victim);
            if (attacker != null)
            {
                karmaManager.KarmaPunishCheck(victim, attacker);
                awardManager.OnPlayerKill(victim.DisplayNickname, teamManager.GetTeam(victim), attacker.DisplayNickname, teamManager.GetTeam(attacker));
            }
            else
            {
                awardManager.OnPlayerKill(victim.DisplayNickname, teamManager.GetTeam(victim), victim.DisplayNickname, teamManager.GetTeam(victim));
            }
        }
        public void OnPlayerChangeRole(PluginAPI.Core.Player victim, RoleTypeId newrole, RoleChangeReason reason)
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
        public bool Scp914Activated(PluginAPI.Core.Player player)
        {
            return traitorTester.shouldActivate(this, player);
        }
        public void Scp914ProcessPlayer(PluginAPI.Core.Player player)
        {
            traitorTester.ProcessPlayer(this, player);
        }
        public PlayerStatsSystem.DamageHandlerBase OnSpawnedCorpse(PluginAPI.Core.Player player, PlayerStatsSystem.DamageHandlerBase damageHandler, string deathReason)
        {
            PlayerStatsSystem.DamageHandlerBase baseHandler = new PlayerStatsSystem.CustomReasonDamageHandler(Corpse.Corpse.GetCorpseInfo(config, player, teamManager.previousTeams[player], damageHandler, deathReason));
            return baseHandler;
        }
    }
}
