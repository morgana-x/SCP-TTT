using PlayerRoles.FirstPersonControl.Spawnpoints;
using PlayerRoles;
using System;
using System.Collections.Generic;
using UnityEngine;
using MapGeneration;
using LabApi.Features.Wrappers;
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
        public Dictionary<Player, DateTime> spawnTimes = new Dictionary<Player, DateTime>();
        public DateTime getSpawnTime(Player pl)
        {
            if (!spawnTimes.ContainsKey(pl))
            {
                spawnTimes.Add(pl, DateTime.Now);
            }
            return spawnTimes[pl];
        }
        public void setSpawnTime(Player pl)
        {
            if (!spawnTimes.ContainsKey(pl))
            {
                spawnTimes.Add(pl, DateTime.Now);
                return;
            }
            spawnTimes[pl] = DateTime.Now;
        }

       
        public void teamSetRole(Player pl, RoleTypeId spawnPointRole = RoleTypeId.None)
        {
            var plTeam = round.teamManager.GetTeam(pl);

            RoleTypeId role = round.config.teamsConfig.TeamRole[plTeam];

            if (pl.Role == role) // Don't need to set the same role twice
            {
                return;
            }

            pl.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.Respawn, RoleSpawnFlags.None);


            //Util.Util.gotoRoleSpawn(pl, spawnPointRole);
        }
        public void Spawn(Player pl, RoomName spawnPoint = RoomName.Unnamed)
        {
            Util.Util.gotoRoom(pl, spawnPoint);
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
