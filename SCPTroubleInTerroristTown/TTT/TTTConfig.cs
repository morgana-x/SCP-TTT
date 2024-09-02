using SCPTroubleInTerroristTown.TTT.Corpse;
using SCPTroubleInTerroristTown.TTT.Karma;
using SCPTroubleInTerroristTown.TTT.Map;
using SCPTroubleInTerroristTown.TTT.Team;
using SCPTroubleInTerroristTown.TTT.Hud;
using SCPTroubleInTerroristTown.TTT.TraitorTester;
using SCPTroubleInTerroristTown.TTT.Award;

namespace SCPTroubleInTerroristTown.TTT
{
    public class RoundConfig
    {
        public int PreRoundDuration { get; set; } = 30; // Preperation to get weapons etc
        public int PostRoundDuration { get; set; } = 30; // Display who won etc 
        public int RoundDuration { get; set; } = 600; // How long round goes for;
    }

    public class TTTConfig
    {

        public bool spawnDebugNPCS { get; set; } = false;

        public TeamConfig teamsConfig { get; set; } = new TeamConfig();
        public RoundConfig roundConfig { get; set; } = new RoundConfig();

        public KarmaConfig karmaConfig { get; set; } = new KarmaConfig();

        public HUDConfig hudConfig { get; set; } = new Hud.HUDConfig();

        public CorpseConfig corpseConfig { get; set; } = new CorpseConfig();

        public MapConfig mapConfig { get; set; } = new MapConfig();

        public TraitorTesterConfig traitorTesterConfig { get; set; } = new TraitorTesterConfig();

        public AwardConfig awardConfig { get; set; } = new AwardConfig();
    }
}
