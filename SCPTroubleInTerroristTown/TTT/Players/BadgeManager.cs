using System;
using System.Collections.Generic;
using Exiled.API.Extensions;
using PluginAPI.Core;

namespace SCPTroubleInTerroristTown.TTT.Players
{
    public class BadgeManager
    {
        Round.Round round;
        public BadgeManager(Round.Round playermanager) 
        {
            round = playermanager;
        }
        public List<PluginAPI.Core.Player> badgeOptOuted = new List<PluginAPI.Core.Player>();

        public void SendFakeBadge(PluginAPI.Core.Player player, PluginAPI.Core.Player targetToTrick, string text, string color)
        {
            if (!targetToTrick.IsReady)
            {
                return;
            }
            if (player.ReferenceHub.serverRoles.HasGlobalBadge)
            {
                return;
            }
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

        public void SyncPlayer(PluginAPI.Core.Player targetToTrick, PluginAPI.Core.Player pl)
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
        public void SyncPlayer(PluginAPI.Core.Player pl)
        {
            foreach(PluginAPI.Core.Player other in PluginAPI.Core.Player.GetPlayers())
            {
                SyncPlayer(pl, other);
                SyncPlayer(other, pl);
            }
        }
        public void Resync()
        {
            foreach(PluginAPI.Core.Player pl in PluginAPI.Core.Player.GetPlayers())
            {
                SyncPlayer(pl);
            }
        }

        public void Cleanup()
        {
            badgeOptOuted.Clear();
        }
    }
}
