using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPTroubleInTerroristTown.TTT.Karma
{
    public enum KarmaState
    { 
        karma_max,
        karma_high,
        karma_med,
        karma_low,
        karma_min
    }

    public class KarmaConfig
    {
        public int karma_starting_karma { get; set; } = 1000;
        public int karma_max { get; set; } = 1000;

        public int karma_kill_penalty { get; set; } = -15;

        public int karma_round_increment { get; set; } = 15;

        public int karma_clean_bonus { get; set; } = 10;

        public int karma_traitorkill_bonus { get; set; } = 5;

        public bool karma_low_round_suspension { get; set; }  = true;
        public int karma_low_round_suspension_amount { get; set; } = 450;

        public bool karma_low_round_kick { get; set; } = true;
        public int karma_low_round_kick_amount { get; set; } = 250;

        public string karma_low_round_suspension_message { get; set; } = "<color=red>Your Karma is too low!</color>\n{karma}<color=red><</color>{minkarma}\n<color=red>You will be left out of rounds until it replenishes to an acceptable amount!</color>";
        public string karma_low_kick_message { get; set; } = "You have been kicked due to having too low karma!\n{karma}<{minkarma}";
        public Dictionary<KarmaState, string[]> KarmaStatus { get; set; } = new Dictionary<KarmaState, string[]>()
        {
            [KarmaState.karma_max] = new string[2]
            {
                "#53bf3d", 
                "Reputable"
            },
            [KarmaState.karma_high] = new string[2]
            {
                "#acbf3d",
                "Crude"
            },
            [KarmaState.karma_med] = new string[2]
            {
                "#f08400",
                "Trigger Happy"
            },
            [KarmaState.karma_low] = new string[2]
            {
                "#f04400",
                "Dangerous"
            },
            [KarmaState.karma_min] = new string[2]
            {
                "#f00000",
                "Liability"
            }
        };

    }
}
