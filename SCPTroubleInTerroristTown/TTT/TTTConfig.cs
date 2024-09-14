using SCPTroubleInTerroristTown.TTT.Corpse;
using SCPTroubleInTerroristTown.TTT.Karma;
using SCPTroubleInTerroristTown.TTT.Map;
using SCPTroubleInTerroristTown.TTT.Team;
using SCPTroubleInTerroristTown.TTT.Hud;
using SCPTroubleInTerroristTown.TTT.TraitorTester;
using SCPTroubleInTerroristTown.TTT.Award;
using SCPTroubleInTerroristTown.TTT.Credit;
using SCPTroubleInTerroristTown.TTT.Round;
namespace SCPTroubleInTerroristTown.TTT
{

    public class TTTConfig
    {

        public bool spawnDebugNPCS { get; set; } = false;

        public RoundConfig roundConfig { get; set; } = new RoundConfig();

        public TraitorTesterConfig traitorTesterConfig { get; set; } = new TraitorTesterConfig();

        public MapConfig mapConfig { get; set; } = new MapConfig();

        public TeamConfig teamsConfig { get; set; } = new TeamConfig();

        public KarmaConfig karmaConfig { get; set; } = new KarmaConfig();

        public HUDConfig hudConfig { get; set; } = new Hud.HUDConfig();

        public CorpseConfig corpseConfig { get; set; } = new CorpseConfig();

        public AwardConfig awardConfig { get; set; } = new AwardConfig();

        public CreditConfig creditConfig { get; set; } = new CreditConfig();
    }
}
