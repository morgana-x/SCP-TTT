using PlayerRoles.Spectating;
using PlayerRoles;
using PluginAPI.Core;
using System;
using UnityEngine;

namespace SCPTroubleInTerroristTown.TTT.Hud
{
    public class Hud
    {
        public Round round;
        public Hud(Round round)
        {

            this.round = round;
        }
        private Player getSpectatingPlayer(Player player)
        {

            uint spectatedId = (player.ReferenceHub.roleManager.CurrentRole as SpectatorRole).SyncedSpectatedNetId;

            foreach (Player spectated in Player.GetPlayers())
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

            foreach (Player pl in Player.GetPlayers())
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
        private Team.Team getPlayerTeam(Player target,bool isSpectating)
        {
            if (isSpectating)
            {
                return (round.teamManager.GetTeam(target) == Team.Team.Traitor) ? Team.Team.Innocent : round.teamManager.GetTeam(target);
            }
            return round.teamManager.GetTeam(target);
        }
        private string GetHud(Player player, bool ShowSpawnMsg, bool spectating=false)
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
            if (round.currentRoundState == Round.RoundState.Finished)
            {
                string winText = round.config.teamsConfig.TeamWinText[round.winner].Replace("{TeamColor}", round.config.teamsConfig.TeamColor[round.winner]);
                hud = hud.Replace("{winner}", winText);
            }
            else
            {
                hud = hud.Replace("{winner}", "");
            }

            string lookingAtInfo = "\n\n";

            Player lookingAt = getLookingAtPlayer(player);

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
            return hud;
        }
        public void RemovePlayer(Player player)
        {
        }
        public void ShowHud(Player player, bool ShowSpawnMsg, float duration = 0.7f)
        {
            // if (player.IsNPC || !player.IsVerified) { return; } // Forgot about this!
            string hud = GetHud(player, ShowSpawnMsg);
            player.ReceiveHint(hud, duration);
        }

        private string[] GetHealthStatus(Player player)
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
        private string[] GetOldKarmaStatus(Player player) // Todo: reference old karma
        {
            return round.karmaManager.KarmaToString(round.karmaManager.GetOldKarma(player)) ;
        }
        private string[] GetKarmaStatus(Player player)
        {

            return round.karmaManager.KarmaToString(round.karmaManager.GetKarma(player));
        }
        private string GetCustomInfo(Player player, Team.Team playerTeam)
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

        private string GetInfoOfTarget(Player player, Player target)
        {
            return GetCustomInfo(target, (round.teamManager.GetTeam(target) == Team.Team.Traitor && round.teamManager.GetTeam(player) != Team.Team.Traitor) ? Team.Team.Innocent : round.teamManager.GetTeam(target));
        }

        private Player getLookingAtPlayer(Player pl)
        {

            Ray ray = new Ray(pl.Camera.position + (pl.Camera.forward * 0.16f), pl.Camera.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                return null;
            }
            /* Exiled
                    public static Player Get(Collider collider)
                    {
                        return Get(collider.transform.root.gameObject);
                    }
             */
            var found = Player.Get(hit.collider.transform.root.gameObject);
            if (found == pl)
            {
                return null;
            }
            return found;
        }

    }
}
