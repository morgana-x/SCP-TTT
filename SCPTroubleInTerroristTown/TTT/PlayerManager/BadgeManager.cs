using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using PluginAPI.Core;
using SCPTroubleInTerroristTown.TTT.Team;
using Mirror;
using YamlDotNet.Core;

namespace SCPTroubleInTerroristTown.TTT.PlayerManager
{
    public class BadgeManager
    {
        Round round;
        public BadgeManager(Round playermanager) 
        {
            round = playermanager;
        }
        public List<Player> badgeOptOuted = new List<Player>();

        public void SendFakeBadge(Player player, Player targetToTrick, string text, string color)
        {
            if (player.ReferenceHub.serverRoles.Network_myText != "" && player.ReferenceHub.serverRoles.Network_myText != null)
                text = player.ReferenceHub.serverRoles.Network_myText + " | " + text;
            try
            {
                targetToTrick.SendFakeSyncVar(player.ReferenceHub.serverRoles.netIdentity, typeof(ServerRoles), "Network_myText", text);
                targetToTrick.SendFakeSyncVar(player.ReferenceHub.serverRoles.netIdentity, typeof(ServerRoles), "Network_myColor", color);
            }
            catch(Exception e) 
            {
                Log.Debug(e.ToString());
            }
        }

        public void SyncPlayer(Player targetToTrick, Player pl)
        {
            if (pl.ReferenceHub.serverRoles.HasGlobalBadge)
            {
                badgeOptOuted.Add(pl);
                return;
            }
            if (badgeOptOuted.Contains(pl)) { badgeOptOuted.Remove(pl); }

            Team.Team team = round.teamManager.GetTeam(pl);

            if (team == Team.Team.Traitor && team != round.teamManager.GetTeam(targetToTrick))
            {
                SendFakeBadge(pl, targetToTrick, round.config.teamsConfig.TeamName[Team.Team.Innocent], round.config.teamsConfig.TeamColorSimplified[Team.Team.Innocent]);
                return;
            }
            SendFakeBadge(pl, targetToTrick, round.config.teamsConfig.TeamName[team], round.config.teamsConfig.TeamColorSimplified[team]);
        }
        public void SyncPlayer(Player pl)
        {
            foreach(Player other in Player.GetPlayers())
            {
                SyncPlayer(pl, other);
                SyncPlayer(other, pl);
            }
        }
        public void Resync()
        {
            foreach(Player pl in Player.GetPlayers())
            {
                SyncPlayer(pl);
            }
        }
    }
}
