using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPTroubleInTerroristTown.TTT.Team
{
    public class TeamConfig
    {


        public float TraitorPercentage { get; set; } = 0.25f; // Percentage of players to be traitor
        public float DetectivePercentage { get; set; } = 0.13f; // Percentage of players to be detective
        public int TraitorMax { get; set; } = 32; // Max num of traitor
        public int DetectiveMax { get; set; } = 32; // Max num of detectives
        public int DetectiveMin { get; set; } = 8; // Min number of players for Detective to exist

        public int DetectiveMinKarma { get; set; } = 0; // Min Karma needed for player to become detective

        public Dictionary<Team, string> TeamColor { get; set; } = new Dictionary<Team, string>()
        {
            [Team.Innocent] = "#6eeb34",
            [Team.Traitor] = "#eb3a34",
            [Team.Detective] = "#3489eb",
            [Team.Spectator] = "#696969", // Actual coincidence I picked this when using the color picker
            [Team.Undecided] = "#696969"
        };
        public Dictionary<Team, string> TeamColorSimplified { get; set; } = new Dictionary<Team, string>()
        {
            [Team.Innocent] = "light_green",
            [Team.Traitor] = "red",
            [Team.Detective] = "cyan",
            [Team.Spectator] = "nickel", // Actual coincidence I picked this when using the color picker
            [Team.Undecided] = "nickel"
        };
        public Dictionary<Team, string> TeamName { get; set; } = new Dictionary<Team, string>()
        {
            [Team.Innocent] = "Innocent",
            [Team.Traitor] = "Traitor",
            [Team.Detective] = "Detective",
            [Team.Spectator] = "Spectator", // Actual coincidence I picked this when using the color picker
            [Team.Undecided] = "Undecided"
        };
        public Dictionary<Team, RoleTypeId> TeamRole { get; set; } = new Dictionary<Team, RoleTypeId>()
        {
            [Team.Innocent] = RoleTypeId.ChaosMarauder,
            [Team.Traitor] = RoleTypeId.ChaosMarauder,
            [Team.Detective] = RoleTypeId.NtfSergeant,
            [Team.Spectator] = RoleTypeId.Spectator,
            [Team.Undecided] = RoleTypeId.ChaosMarauder
        };
        public Dictionary<Team, string> TeamWinText { get; set; } = new Dictionary<Team, string>()
        {
            [Team.Innocent] = "<color={TeamColor}>Innocents Win!</color>",
            [Team.Traitor] = "<color={TeamColor}>Traitors Win!</color>",
            [Team.Detective] = "This is a bug if you see this!\nIgnore it!",
            [Team.Spectator] = "<color={TeamColor}>Stalemate!</color>", // Actual coincidence I picked this when using the color picker
            [Team.Undecided] = "This is a bug if you see this!\nIgnore it!",
        };

        public Dictionary<Team, string> TeamSpawnText { get; set; } = new Dictionary<Team, string>()
        {
            [Team.Innocent] = "<color={TeamColor}>Innocent</color>\nYou are an Innocent Terrorist! Find and kill the Traitors!",
            [Team.Traitor] = "<color={TeamColor}>Traitor</color>\nYou are a Traitor! Kill Innocents and Detectives to win!",
            [Team.Detective] = "<color={TeamColor}>Detective</color>\nYou are a Detective! Kill Traitors and protect Innocents to win!",
            [Team.Spectator] = "<color={TeamColor}>Spectator</color>\nYou are a Spectator! (Don't) Have fun not existing!", // Actual coincidence I picked this when using the color picker
            [Team.Undecided] = "<color={TeamColor}>Undecided</color>\nPrepare for the round by picking up weapons!",
        };

        public Dictionary<Team, List<ItemType>> TeamLoadout { get; set; } = new Dictionary<Team, List<ItemType>>()
        {
            [Team.Undecided] = new List<ItemType>() // Given to all players at start of round
            {
                ItemType.KeycardMTFPrivate,
                ItemType.Radio,
            },
            [Team.Traitor] = new List<ItemType>()
            {
                ItemType.KeycardO5
            },
            [Team.Detective] = new List<ItemType>()
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
}
