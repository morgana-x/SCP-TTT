using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPTroubleInTerroristTown.TTT.Hud
{
    public enum HealthStatus
    { 
        health_max,
        health_high,
        health_med,
        health_low,
        health_min
    }

    public class HUDConfig
    {
        public int ShowCustomSpawnMessageDuration { get; set; } = 5;

        public string CustomInfoTemplate { get; set; } = "<color={KarmaColor}>{KarmaStatus}</color>\n<color={HealthColor}>{HealthStatus}</color>\n<color={TeamColor}>{TeamName}</color>";

        public Dictionary<HealthStatus, string[]> HudHealthStatus { get; set; } = new Dictionary<HealthStatus, string[]>()
        {
            [HealthStatus.health_max] = new string[] {
                "green",
                "Healthy"
            },
            [HealthStatus.health_high] = new string[] {
                "yellow",
                "Injured"
            },
            [HealthStatus.health_med] = new string[] {
                "orange",
                "Badly Injured"
            },
            [HealthStatus.health_low] = new string[] {
                "red",
                "Extremely Injured"
            },
            [HealthStatus.health_min] = new string[] {
                "red",
                "Near Death"
            },
        };
        public string Hud { get; set; } = "\n\n<align=\"center\">{notify1}\n{notify2}\n{notify3}\n<align=\"center\">{winner}\n{spawn}\n{lookingAtInfo}\n{awardsTitle}\n{award1}\n{award2}\n{award3}\n\n<align=\"left\">      {role} {time} {karma}";
        public string RoleWidget { get; set; } = "<color={TeamColor}>{TeamName}</color>";
        public string TimeWidget { get; set; } = "<color=#616161>{TimeLeft}</color>";

        public string KarmaWidget { get; set; } = "<color={KarmaColor}>{Karma}</color>";

        public bool ShowKarmaWidget { get; set; } = false;
    }

}
