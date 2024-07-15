using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Core;
using PlayerRoles;
using UnityEngine;
using PlayerRoles.Spectating;

namespace SCPTroubleInTerroristTown.TTT
{
    public class TTTHud
    {
        public TTTRound round;
        public TTTHud(TTTRound round)
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
        private string GetHud(Player player, bool ShowSpawnMsg)
        {
            if (player.Role == PlayerRoles.RoleTypeId.Spectator) // Such simple spectating logic, yay :tear:
            {
                var spectatedPlayer = getSpectatingPlayer(player);
                if (spectatedPlayer != null && spectatedPlayer.Role != RoleTypeId.Spectator) // make sure spectated player is somehow not spectator, other wise we get the matrix in the bad way! No escape from the loop!
                {
                    return GetHud(spectatedPlayer, false);
                }
            }
            TTTRound.Team playerTeam = round.GetTeam(player);
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
            if (round.currentRoundState == TTTRound.RoundState.Finished)
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

            hud = hud.Replace("{lookingAtInfo}", $"{lookingAtInfo}");


            TimeSpan timeLeft = round.NextRoundState.Subtract(DateTime.Now);
            string min = (timeLeft.Minutes <= 9 ? "0" : "") +  timeLeft.Minutes.ToString();
            string sec = (timeLeft.Seconds <= 9 ? "0" : "") + timeLeft.Seconds.ToString();
            hud = hud.Replace("{time}", round.config.hudConfig.TimeWidget.Replace("{TimeLeft}", min + ":" + sec ));
            string[] karmaStatus = GetKarmaStatus(player);

            if (round.config.hudConfig.ShowKarmaWidget)
            {
                string karmaWidget = round.config.hudConfig.KarmaWidget.Replace("{KarmaColor}", karmaStatus[0]).Replace("{Karma}", karmaStatus[1]) + $"({round.GetKarma(player)})";
                hud = hud.Replace("{karma}", karmaWidget);
            }
            else
            {
                hud = hud.Replace("{karma}", "");
            }
            return hud;
        }
        private Dictionary<Player, string> lastHint = new Dictionary<Player, string>();

        private string getlastHint(Player pl)
        {
            if (!lastHint.ContainsKey(pl))
            {
                lastHint.Add(pl, "");
            }
            return lastHint[pl];
        }
        public void RemovePlayer(Player player)
        {
            if (lastHint.ContainsKey(player))
            {
                lastHint.Remove(player);
            }
        }
        public void ShowHud(Player player, bool ShowSpawnMsg, float duration = 0.7f)
        {
           // if (player.IsNPC || !player.IsVerified) { return; } // Forgot about this!
            string hud = GetHud(player, ShowSpawnMsg);
            /*if (getlastHint(player) == hud)
            {
                return;
            }
            lastHint[player] = hud;*/
            player.ReceiveHint(hud, duration);
        }

        private string[] GetHealthStatus(Player player)
        {
            string[] status = { "red", "ERROR GETTING HEALTH STATUS" };
            foreach (int key in round.config.hudConfig.HealthStatus.Keys.OrderBy( (i) => i)) 
            {
                if (player.Health <= key)
                {
                    status = round.config.hudConfig.HealthStatus[key];
                    break;
                }
            }
            return status;
        }
        private string[] GetOldKarmaStatus(Player player) // Todo: reference old karma
        {
            string[] status = { "red", "ERROR GETTING OLD KARMA STATUS" };
            foreach (int key in round.config.hudConfig.KarmaStatus.Keys.OrderBy((i) => i))
            {
                if (round.GetOldKarma(player) <= key)
                {
                    status = round.config.hudConfig.KarmaStatus[key];
                    break;
                }
            }
            return status;
        }
        private string[] GetKarmaStatus(Player player)
        {
            string[] status = { "red", "ERROR GETTING KARMA STATUS" };
            foreach (int key in round.config.hudConfig.KarmaStatus.Keys.OrderBy((i) => i))
            {
                if (round.GetKarma(player) <= key)
                {
                    status = round.config.hudConfig.KarmaStatus[key];
                    break;
                }
            }
            return status;
        }
        private string GetCustomInfo(Player player, TTTRound.Team playerTeam)
        {
            string tem = round.config.hudConfig.CustomInfoTemplate;
            tem = tem.Replace("{TeamColor}",
                round.config.teamsConfig.TeamColor[playerTeam]);
            tem = tem.Replace("{TeamName}",
                round.config.teamsConfig.TeamName[playerTeam]);

            //{HealthColor}>{HealthStatus}
            string[] status = GetHealthStatus(player);
            tem = tem.Replace("{HealthColor}", status[0]).Replace("{HealthStatus}", status[1]);

            string[] karmaStatus = GetOldKarmaStatus(player);
            tem = tem.Replace("{KarmaColor}", karmaStatus[0]).Replace("{KarmaStatus}", karmaStatus[1]);

            return tem;
        }

        private string GetInfoOfTarget(Player player, Player target)
        {
            return GetCustomInfo(target, (round.GetTeam(target) == TTTRound.Team.Traitor && round.GetTeam(player) != TTTRound.Team.Traitor) ? TTTRound.Team.Innocent : round.GetTeam(target));
        }

        private Player getLookingAtPlayer(Player pl)
        {
 
            Ray ray = new Ray(pl.Camera.position + (pl.Camera.forward * 0.16f), pl.Camera.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, 45))
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
