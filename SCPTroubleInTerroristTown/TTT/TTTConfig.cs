
using MapGeneration;
using PlayerRoles;
using PluginAPI.Core.Zones;
using SCPTroubleInTerroristTown.TTT.Corpse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.NonAllocLINQ;

namespace SCPTroubleInTerroristTown.TTT
{

    public class TeamsConfig
    {
        

        public float TraitorPercentage { get; set; } = 0.25f; // Percentage of players to be traitor
        public float DetectivePercentage { get; set; } = 0.13f; // Percentage of players to be detective
        public int TraitorMax { get; set; } = 32; // Max num of traitor
        public int DetectiveMax { get; set; } = 32; // Max num of detectives
        public int DetectiveMin { get; set; } = 8; // Min number of players for Detective to exist

        public int DetectiveMinKarma { get; set; } = 0; // Min Karma needed for player to become detective

        public Dictionary<TTTRound.Team, string> TeamColor { get; set; } = new Dictionary<TTTRound.Team, string>()
        {
            [TTTRound.Team.Innocent] = "#6eeb34",
            [TTTRound.Team.Traitor] = "#eb3a34",
            [TTTRound.Team.Detective] = "#3489eb",
            [TTTRound.Team.Spectator] = "#696969", // Actual coincidence I picked this when using the color picker
            [TTTRound.Team.Undecided] = "#696969"
        };
        public Dictionary<TTTRound.Team, string> TeamColorSimplified { get; set; } = new Dictionary<TTTRound.Team, string>()
        {
            [TTTRound.Team.Innocent] = "green",
            [TTTRound.Team.Traitor] = "red",
            [TTTRound.Team.Detective] = "blue",
            [TTTRound.Team.Spectator] = "gray", // Actual coincidence I picked this when using the color picker
            [TTTRound.Team.Undecided] = "gray"
        };
        public Dictionary<TTTRound.Team, string> TeamName { get; set; } = new Dictionary<TTTRound.Team, string>()
        {
            [TTTRound.Team.Innocent] = "Innocent",
            [TTTRound.Team.Traitor] = "Traitor",
            [TTTRound.Team.Detective] = "Detective",
            [TTTRound.Team.Spectator] = "Spectator", // Actual coincidence I picked this when using the color picker
            [TTTRound.Team.Undecided] = "Undecided"
        };
        public Dictionary<TTTRound.Team, RoleTypeId> TeamRole { get; set; } = new Dictionary<TTTRound.Team, RoleTypeId>()
        {
            [TTTRound.Team.Innocent] = RoleTypeId.ChaosMarauder,
            [TTTRound.Team.Traitor] = RoleTypeId.ChaosMarauder,
            [TTTRound.Team.Detective] = RoleTypeId.NtfSergeant,
            [TTTRound.Team.Spectator] = RoleTypeId.Spectator, 
            [TTTRound.Team.Undecided] = RoleTypeId.ChaosMarauder
        };
        public Dictionary<TTTRound.Team, string> TeamWinText { get; set; } = new Dictionary<TTTRound.Team, string>()
        {
            [TTTRound.Team.Innocent] = "<color={TeamColor}>Innocents Win!</color>",
            [TTTRound.Team.Traitor] = "<color={TeamColor}>Traitors Win!</color>",
            [TTTRound.Team.Detective] = "This is a bug if you see this!\nIgnore it!",
            [TTTRound.Team.Spectator] = "<color={TeamColor}>Stalemate!</color>", // Actual coincidence I picked this when using the color picker
            [TTTRound.Team.Undecided] = "This is a bug if you see this!\nIgnore it!",
        };

        public Dictionary<TTTRound.Team, string> TeamSpawnText { get; set; } = new Dictionary<TTTRound.Team, string>()
        {
            [TTTRound.Team.Innocent] = "<color={TeamColor}>Innocent</color>\nYou are an Innocent Terrorist! Find and kill the Traitors!",
            [TTTRound.Team.Traitor] = "<color={TeamColor}>Traitor</color>\nYou are a Traitor! Kill Innocents and Detectives to win!",
            [TTTRound.Team.Detective] = "<color={TeamColor}>Detective</color>\nYou are a Detective! Kill Traitors and protect Innocents to win!",
            [TTTRound.Team.Spectator] = "<color={TeamColor}>Spectator</color>\nYou are a Spectator! (Don't) Have fun not existing!", // Actual coincidence I picked this when using the color picker
            [TTTRound.Team.Undecided] = "<color={TeamColor}>Undecided</color>\nPrepare for the round by picking up weapons!",
        };

        public Dictionary<TTTRound.Team, List<ItemType>> TeamLoadout { get; set; } = new Dictionary<TTTRound.Team, List<ItemType>>()
        {
            [TTTRound.Team.Undecided] = new List<ItemType>() // Given to all players at start of round
            {
                ItemType.KeycardMTFPrivate,
                ItemType.Radio,
            },
            [TTTRound.Team.Traitor] = new List<ItemType>()
            {
                ItemType.KeycardO5
            },
            [TTTRound.Team.Detective] = new List<ItemType>()
            {
                ItemType.KeycardMTFCaptain,
                ItemType.GunRevolver,
                ItemType.Ammo44cal,
                ItemType.Ammo762x39,
                ItemType.Medkit,
                ItemType.ArmorCombat
            }
        };
    }

    public class RoundConfig
    {
        public int PreRoundDuration { get; set; } = 30; // Preperation to get weapons etc
        public int PostRoundDuration { get; set; } = 30; // Display who won etc 
        public int RoundDuration { get; set; } = 600; // How long round goes for;
    }

    public class HUDConfig
    {
        public int ShowCustomSpawnMessageDuration { get; set; } = 5;

        public string CustomInfoTemplate { get; set; } = "<color={TeamColor}>{TeamName}</color>\n<color={KarmaColor}>{KarmaStatus}</color>\n<color={HealthColor}>{HealthStatus}</color>";

        public Dictionary<int, string[]> HealthStatus { get; set; } = new Dictionary<int, string[]>()
        {
            [100] = new string[] {
                "green",
                "Healthy"
            },
            [75] = new string[] {
                "yellow",
                "Injured"
            },
            [50] = new string[] {
                "orange",
                "Badly Injured"
            },
            [25] = new string[] {
                "red",
                "Extremely Injured"
            },
            [10] = new string[] {
                "red",
                "Near Death"
            },
        };
        public Dictionary<int, string[]> KarmaStatus { get; set; } = new Dictionary<int, string[]>()
        {
            [50] = new string[] {
                "#f5e056",
                "Saint"
            },
            [25] = new string[] {
                "#63abb8",
                "Kind"
            },
            [10] = new string[] {
                "green",
                "Friendly"
            },
            [0] = new string[] {
                "#757575",
                "Neutral"
            },
            [-10] = new string[] {
                "orange",
                "Trigger Happy"
            },
            [-25] = new string[] {
                "red",
                "Serial Killer"
            },
            [-50] = new string[] {
                "red",
                "Evil Incarnate"
            },
        };
        public string Hud { get; set; } = @"
        





<align=""center"">{winner}\n{spawn}\n{lookingAtInfo}





        <align=""left"">{karma}
        <align=""left"">{role} {time}
        ";
        public string RoleWidget { get; set; } = "<color={TeamColor}>{TeamName}</color>";
        public string TimeWidget { get; set; } = "<color=#616161>{TimeLeft}</color>";

        public string KarmaWidget { get; set; } = "<color={KarmaColor}>{Karma}</color>";

        public bool ShowKarmaWidget { get; set; } = false;
    }




    public class TTTConfig
    {
        public TeamsConfig teamsConfig { get; set; } = new TeamsConfig();
        public RoundConfig roundConfig { get; set; } = new RoundConfig();

        public HUDConfig hudConfig { get; set; } = new HUDConfig();

        public CorpseInfoTranslationConfig corpseConfig { get; set; } = new CorpseInfoTranslationConfig();

        public RoleTypeId spawnPoint { get; set; } = RoleTypeId.ClassD;
        public MapGeneration.FacilityZone spawnZone { get; set; } = MapGeneration.FacilityZone.LightContainment;

        public bool lockDownSpawnZone { get; set; } = true;

        public bool spawnDebugNPCS { get; set; } = false;
    }
}
