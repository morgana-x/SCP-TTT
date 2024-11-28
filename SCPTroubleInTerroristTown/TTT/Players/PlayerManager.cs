using PlayerRoles.FirstPersonControl.Spawnpoints;
using PlayerRoles;
using System;
using System.Collections.Generic;
using UnityEngine;
using MapGeneration;

namespace SCPTroubleInTerroristTown.TTT.Players
{
    public class PlayerManager
    {
        Round.Round round;

        public BadgeManager badgeManager;
        public NotificationManager notificationManager;
        public PlayerManager(Round.Round round)
        {
            this.round = round;
            badgeManager = new BadgeManager(round);
            notificationManager = new NotificationManager(round);
        }
        public Dictionary<PluginAPI.Core.Player, DateTime> spawnTimes = new Dictionary<PluginAPI.Core.Player, DateTime>();
        public DateTime getSpawnTime(PluginAPI.Core.Player pl)
        {
            if (!spawnTimes.ContainsKey(pl))
            {
                spawnTimes.Add(pl, DateTime.Now);
            }
            return spawnTimes[pl];
        }
        public void setSpawnTime(PluginAPI.Core.Player pl)
        {
            if (!spawnTimes.ContainsKey(pl))
            {
                spawnTimes.Add(pl, DateTime.Now);
                return;
            }
            spawnTimes[pl] = DateTime.Now;
        }

       
        public void teamSetRole(PluginAPI.Core.Player pl, RoleTypeId spawnPointRole = RoleTypeId.None)
        {
            var plTeam = round.teamManager.GetTeam(pl);

            RoleTypeId role = round.config.teamsConfig.TeamRole[plTeam];

            if (pl.Role == role) // Don't need to set the same role twice
            {
                return;
            }

            pl.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.Respawn, RoleSpawnFlags.None);

            Util.Util.gotoRoleSpawn(pl, spawnPointRole);
        }
        public void Spawn(PluginAPI.Core.Player pl, RoomName spawnPoint = RoomName.Unnamed)
        {
            teamSetRole(pl);
            Util.Util.gotoRoom(pl, spawnPoint);
            round.teamManager.loadoutManager.GiveLoadout(pl);
            round.creditManager.GiveStartingCredits(pl);
            setSpawnTime(pl);
        }
        public void Cleanup()
        {
            spawnTimes.Clear();
            badgeManager.Cleanup();
            notificationManager.Cleanup();
        }
       
    }
}
