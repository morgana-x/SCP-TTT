﻿using PlayerRoles.FirstPersonControl.Spawnpoints;
using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCPTroubleInTerroristTown.TTT.PlayerManager
{
    public class PlayerManager
    {
        Round round;

        public BadgeManager badgeManager;
        public PlayerManager(Round round)
        {
            this.round = round;
            badgeManager = new BadgeManager(round);
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

            if (spawnPointRole != RoleTypeId.None) // Teleport to spawnpoint
            {
                ISpawnpointHandler spawnpoint = null;
                Vector3 spawnpointPos = Vector3.zero;
                float rot = 0f;
                RoleSpawnpointManager.TryGetSpawnpointForRole(spawnPointRole, out spawnpoint);
                if (!RoleSpawnpointManager.TryGetSpawnpointForRole(spawnPointRole, out spawnpoint))
                {
                    return;
                }
                if (!spawnpoint.TryGetSpawnpoint(out spawnpointPos, out rot))
                {
                    return;
                }
                pl.Position = spawnpointPos;


            }
        }
        public void Spawn(Player pl, RoleTypeId spawnPoint = RoleTypeId.None)
        {
            teamSetRole(pl, spawnPoint);
            round.teamManager.loadoutManager.GiveLoadout(pl);
            setSpawnTime(pl);
        }
        public void Cleanup()
        {
            spawnTimes.Clear();
            badgeManager.Cleanup();
        }
       
    }
}