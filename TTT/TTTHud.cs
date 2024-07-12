using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using PlayerRoles;
using UnityEngine;

namespace SCP_SL_Trouble_In_Terrorist_Town.TTT
{
    public class TTTHud
    {
        public TTTRound round;
        public TTTHud(TTTRound round)
        {

            this.round = round;
        }

        private string GetHud(Player player, bool ShowSpawnMsg)
        {
            if (player.Role.Type == PlayerRoles.RoleTypeId.Spectator) // Such simple spectating logic, yay :tear:
            {
                var spectatedPlayer = player.Role.As<SpectatorRole>().SpectatedPlayer;
                if (spectatedPlayer != null && spectatedPlayer.Role.Type != RoleTypeId.Spectator) // make sure spectated player is somehow not spectator, other wise we get the matrix in the bad way! No escape from the loop!
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

            hud = hud.Replace("{lookingAtInfo}", lookingAtInfo);


            TimeSpan timeLeft = round.NextRoundState.Subtract(DateTime.Now);
            string min = (timeLeft.Minutes <= 9 ? "0" : "") +  timeLeft.Minutes.ToString();
            string sec = (timeLeft.Seconds <= 9 ? "0" : "") + timeLeft.Seconds.ToString();
            hud = hud.Replace("{time}", round.config.hudConfig.TimeWidget.Replace("{TimeLeft}", min + ":" + sec ));
           
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
        public void ShowHud(Exiled.API.Features.Player player, bool ShowSpawnMsg, float duration = 1.15f)
        {
            if (player.IsNPC || !player.IsVerified) { return; } // Forgot about this!
            string hud = GetHud(player, ShowSpawnMsg);
            /*if (getlastHint(player) == hud)
            {
                return;
            }
            lastHint[player] = hud;*/
            player.ShowHint(hud, duration);
        }

        private string[] GetHealthStatus(Exiled.API.Features.Player player)
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
        private string GetCustomInfo(Exiled.API.Features.Player player, TTTRound.Team playerTeam)
        {
            string tem = round.config.hudConfig.CustomInfoTemplate;
            tem = tem.Replace("{TeamColor}",
                round.config.teamsConfig.TeamColor[playerTeam]);
            tem = tem.Replace("{TeamName}",
                round.config.teamsConfig.TeamName[playerTeam]);

            //{HealthColor}>{HealthStatus}
            string[] status = GetHealthStatus(player);
            tem = tem.Replace("{HealthColor}", status[0]).Replace("{HealthStatus}", status[1]);

            return tem;
        }

        private string GetInfoOfTarget(Exiled.API.Features.Player player, Exiled.API.Features.Player target)
        {
            return GetCustomInfo(target, (round.GetTeam(target) == TTTRound.Team.Traitor && round.GetTeam(player) != TTTRound.Team.Traitor) ? TTTRound.Team.Innocent : round.GetTeam(target));
        }

        private Exiled.API.Features.Player getLookingAtPlayer(Player pl)
        {
            var ray = new Ray(pl.CameraTransform.position + (pl.CameraTransform.forward * 0.16f), pl.CameraTransform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, 45))
            {
                return null;
            }
            var found = Player.Get(hit.collider);
            if (found == pl)
            {
                return null;
            }
            return found;
        }

    }
}
