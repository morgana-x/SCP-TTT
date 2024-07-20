using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PluginAPI.Core;
using PlayerRoles.FirstPersonControl.Spawnpoints;
using UnityEngine;

namespace SCPTroubleInTerroristTown.TTT
{
    public partial class TTTRound
    {
        public Dictionary<Player, DateTime> spawnTimes = new Dictionary<Player, DateTime>();
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
        private void teamSetRole(Player pl, RoleTypeId spawnPointRole = RoleTypeId.None)
        {
            var plTeam = GetTeam(pl);

            /*if (plTeam == Team.Detective && pl.IsNPC) // Testing cause of crash
            {
                return;
            }*/

            RoleTypeId role = config.teamsConfig.TeamRole[plTeam];

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
            GiveLoadout(pl);
            setSpawnTime(pl);



        }

        private void SpawnPlayers()
        {
            foreach (Player pl in Player.GetPlayers()) // Set player models, using Roles
            {
                if (!pl.IsAlive)
                {
                    SetTeam(pl, Team.Spectator);
                    continue;
                }
                if (!config.teamsConfig.TeamRole.ContainsKey(GetTeam(pl)))
                {
                    Log.Debug($"Player's team {GetTeam(pl)} does not have an ingame role set!");
                    continue;
                }
                // Player should already be spawned with correct role
                Spawn(pl);
            }
        }
    }
}
