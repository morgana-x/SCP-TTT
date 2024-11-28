using PlayerRoles.Spectating;
using PlayerRoles;
using PluginAPI.Core;
using System;
using UnityEngine;

namespace SCPTroubleInTerroristTown.TTT.Hud
{
    public class Hud
    {
        public Round.Round round;
        public Hud(Round.Round round)
        {

            this.round = round;
        }
        private PluginAPI.Core.Player getSpectatingPlayer(PluginAPI.Core.Player player)
        {

            uint spectatedId = (player.ReferenceHub.roleManager.CurrentRole as SpectatorRole).SyncedSpectatedNetId;

            foreach (PluginAPI.Core.Player spectated in PluginAPI.Core.Player.GetPlayers())
            {
                if (spectated == player)
                {
                    continue;
                }
                if (spectated.ReferenceHub.netId == spectatedId)
                {
                    return spectated;
                }
            }

            return null;
        }
        DateTime nextHudShow = DateTime.Now;

        public void hudUpdate() // Todo: Optimise this
        {
            if (DateTime.Now < nextHudShow)
            {
                return;
            }
            nextHudShow = DateTime.Now.AddSeconds(0.6f);

            foreach (PluginAPI.Core.Player pl in PluginAPI.Core.Player.GetPlayers())
            {
                if (pl == null)
                {
                    continue;
                }
                if ((!round.teamManager.playerTeams.ContainsKey(pl)))
                {
                    continue;
                }
                //float ping = (pl.Ping) + 0.1f;
                //Math.Min(ping, 1.9f); // 1.8 is the max I get on satelitte on AU servers, so I sure hope noone gets worse than this!
                ShowHud(pl, DateTime.Now.Subtract(round.playerManager.getSpawnTime(pl)).TotalSeconds < round.config.hudConfig.ShowCustomSpawnMessageDuration, 1.3f);
            }
        }
        private Team.Team getPlayerTeam(PluginAPI.Core.Player target,bool isSpectating)
        {
            if (isSpectating)
            {
                return (round.teamManager.GetTeam(target) == Team.Team.Traitor) ? Team.Team.Innocent : round.teamManager.GetTeam(target);
            }
            return round.teamManager.GetTeam(target);
        }

        private string getRoundEndWidget(string hud)
        {
            string winText = round.config.teamsConfig.TeamWinText[round.winner].Replace("{TeamColor}", round.config.teamsConfig.TeamColor[round.winner]);
            hud = hud.Replace("{winner}", winText);
            hud = hud.Replace("{awardsTitle}", round.config.awardConfig.AwardTitle);
            
            for (int i=0; i < 3; i++)
            {
                if (round.awardManager.finalAwards.Count <= i)
                {
                    hud = hud.Replace("{award" + (i + 1).ToString() + "}", "\n");
                    continue;
                }
                hud = hud.Replace("{award" + (i + 1).ToString() + "}", round.awardManager.finalAwards[i]);
            }
            return hud;
        }
        private string GetHud(PluginAPI.Core.Player player, bool ShowSpawnMsg, bool spectating=false)
        {
            if (player.Role == PlayerRoles.RoleTypeId.Spectator) // Such simple spectating logic, yay :tear:
            {
                var spectatedPlayer = getSpectatingPlayer(player);
                if (spectatedPlayer != null && spectatedPlayer.Role != RoleTypeId.Spectator) // make sure spectated player is somehow not spectator, other wise we get the matrix in the bad way! No escape from the loop!
                {
                    return GetHud(spectatedPlayer, false, true);
                }
            }
            //Log.Debug("Getting hud for " + player.Nickname);
            Team.Team playerTeam = getPlayerTeam(player, spectating);/*round.teamManager.GetTeam(player);*/
            string hud = round.config.hudConfig.Hud;
            hud = hud.Replace("{role}",
               round.config.hudConfig.RoleWidget.Replace("{TeamColor}",
               round.config.teamsConfig.TeamColor[playerTeam]).Replace("{TeamName}",
               round.config.teamsConfig.TeamName[playerTeam])
               );
           
            
          
            if (ShowSpawnMsg)
            {
                hud = hud.Replace("{spawn}", round.config.teamsConfig.TeamSpawnText[playerTeam].Replace("{TeamColor}", round.config.teamsConfig.TeamColor[playerTeam]));
            }
            else
            {
                hud = hud.Replace("{spawn}", "\n");
            }
            if (round.currentRoundState == Round.Round.RoundState.Finished)
            {
                hud = getRoundEndWidget(hud);
            }
            else
            {
                hud = hud.Replace("{winner}", "");
                hud = hud.Replace("{awardsTitle}", "");
                hud = hud.Replace("{award1}", "\n");
                hud = hud.Replace("{award2}", "\n");
                hud = hud.Replace("{award3}", "\n");
               // hud = hud.Replace("{award4}", "\n");
            }

            string lookingAtInfo = "\n\n";

            PluginAPI.Core.Player lookingAt = getLookingAtPlayer(player);

            if (lookingAt != null && lookingAt.IsAlive)
            {
                lookingAtInfo = GetInfoOfTarget(player, lookingAt);
            }

            hud = hud.Replace("{lookingAtInfo}", $"<size=25>{lookingAtInfo}</size>");


            TimeSpan timeLeft = round.NextRoundState.Subtract(DateTime.Now);
            string min = (timeLeft.Minutes <= 9 ? "0" : "") + timeLeft.Minutes.ToString();
            string sec = (timeLeft.Seconds <= 9 ? "0" : "") + timeLeft.Seconds.ToString();
            hud = hud.Replace("{time}", round.config.hudConfig.TimeWidget.Replace("{TimeLeft}", min + ":" + sec));

            if (round.config.hudConfig.ShowKarmaWidget)
            {
                string[] karmaStatus = GetKarmaStatus(player);
                string karmaWidget = round.config.hudConfig.KarmaWidget.Replace("{KarmaColor}", karmaStatus[0]).Replace("{Karma}", karmaStatus[1]) + $"({round.karmaManager.GetKarma(player)})";
                hud = hud.Replace("{karma}", karmaWidget);
            }
            else
            {
                hud = hud.Replace("{karma}", "");
            }

            hud = round.playerManager.notificationManager.getNotificationWidget(hud, player);
            return hud;
        }
        public void RemovePlayer(PluginAPI.Core.Player player)
        {
        }
        public void ShowHud(PluginAPI.Core.Player player, bool ShowSpawnMsg, float duration = 0.7f)
        {
            // if (player.IsNPC || !player.IsVerified) { return; } // Forgot about this!
            string hud = GetHud(player, ShowSpawnMsg);
           // Log.Debug("Showing " + hud + " to " + player.DisplayNickname);
            player.ReceiveHint(hud, duration);
        }

        private string[] GetHealthStatus(PluginAPI.Core.Player player)
        {
            float maxhealth = player.MaxHealth;
            float health = player.Health;
            if (health > maxhealth * 0.89f)
                return round.config.hudConfig.HudHealthStatus[HealthStatus.health_max];

            if (health > maxhealth * 0.8f)
                return round.config.hudConfig.HudHealthStatus[HealthStatus.health_high];

            if (health > maxhealth * 0.65f)
                return round.config.hudConfig.HudHealthStatus[HealthStatus.health_med];

            if (health > maxhealth * 0.5f)
                return round.config.hudConfig.HudHealthStatus[HealthStatus.health_low];

            return round.config.hudConfig.HudHealthStatus[HealthStatus.health_min];
        }
        private string[] GetOldKarmaStatus(PluginAPI.Core.Player player)
        {
            return round.karmaManager.KarmaToString(round.karmaManager.GetOldKarma(player)) ;
        }
        private string[] GetKarmaStatus(PluginAPI.Core.Player player)
        {

            return round.karmaManager.KarmaToString(round.karmaManager.GetKarma(player));
        }
        private string GetCustomInfo(PluginAPI.Core.Player player, Team.Team playerTeam)
        {
            string tem = round.config.hudConfig.CustomInfoTemplate;

            if (round.playerManager.badgeManager.badgeOptOuted.Contains(player))
            {
                tem = tem.Replace("{TeamColor}",
                    round.config.teamsConfig.TeamColor[playerTeam]);
                tem = tem.Replace("{TeamName}",
                    round.config.teamsConfig.TeamName[playerTeam]);
            }
            else
            {
                tem = tem.Replace("<color={TeamColor}>{TeamName}</color>",
                    "");
                tem = tem.Replace("{TeamName}",
                    "");
            }

            //{HealthColor}>{HealthStatus}
            string[] status = GetHealthStatus(player);
            tem = tem.Replace("{HealthColor}", status[0]).Replace("{HealthStatus}", status[1]);

            string[] karmaStatus = GetOldKarmaStatus(player);
            tem = tem.Replace("{KarmaColor}", karmaStatus[0]).Replace("{KarmaStatus}", karmaStatus[1]);

            return tem;
        }

        private string GetInfoOfTarget(PluginAPI.Core.Player player, PluginAPI.Core.Player target)
        {
            return GetCustomInfo(target, (round.teamManager.GetTeam(target) == Team.Team.Traitor && round.teamManager.GetTeam(player) != Team.Team.Traitor) ? Team.Team.Innocent : round.teamManager.GetTeam(target));
        }
        private PluginAPI.Core.Player getLookingAtPlayerCheapWorkaround(PluginAPI.Core.Player pl)
        {
            Vector3 startPos = pl.Camera.position + (pl.Camera.forward * 0.16f);
            for (int i = 0; i < 10; i++)
            {
                foreach (Player v in Player.GetPlayers())
                {
                    if (!v.IsAlive) continue;
                    if (v == pl) continue;
                    if (Vector3.Distance(v.Position + Vector3.up, startPos) > 1f) continue;
                    return v;
                }
                startPos += pl.Camera.forward;
            }
            return null;
        }
        private PluginAPI.Core.Player getLookingAtPlayer(PluginAPI.Core.Player pl)
        {
            return getLookingAtPlayerCheapWorkaround(pl); // Temp while I figure out how to raycasting for players in 14.0
            Ray ray = new Ray(pl.Camera.position + (pl.Camera.forward * 0.16f), pl.Camera.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                return null;
            }
            var found = Player.Get(hit.collider.gameObject.transform.root.gameObject);
            if (found == pl)
            {
                return null;
            }
           
            return found;
        }

    }
}
