using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Extensions;

namespace SCP_SL_Trouble_In_Terrorist_Town
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
        private void teamSetRole(Player pl, RoleTypeId spawnPoint = RoleTypeId.None)
        {
            var plTeam = GetTeam(pl);

            /*if (plTeam == Team.Detective && pl.IsNPC) // Testing cause of crash
            {
                return;
            }*/

            RoleTypeId role = config.teamsConfig.TeamRole[plTeam];

            if (pl.Role.Type == role) // Don't need to set the same role twice
            {
                return;
            }

            // pl.Role.Set(role, RoleSpawnFlags.None); // This crashes server somehow
            pl.RoleManager.InitializeNewRole(role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);

            if (spawnPoint != RoleTypeId.None) // Teleport to spawnpoint
            {
                pl.Teleport(spawnPoint.GetRandomSpawnLocation());
            }

            if (pl.IsNPC) // NPC Jank
            {
                pl.Health = 100;
            }
        }
        public void Spawn(Player pl, RoleTypeId spawnPoint = RoleTypeId.None)
        {


            if (!pl.IsNPC)
            {
                Log.Debug("Spawning player " + pl.DisplayNickname);
            }

            teamSetRole(pl, spawnPoint);
            GiveLoadout(pl);
            setSpawnTime(pl);

            if (!pl.IsNPC)
            {
                Log.Debug("Spawned " + pl.DisplayNickname);
            }

        }

        private void SpawnPlayers()
        {
            foreach (Player pl in Player.List) // Set player models, using Roles
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
