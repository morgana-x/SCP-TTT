
using MapGeneration;
using PlayerRoles;
using PluginAPI.Core.Zones;
using SCPTroubleInTerroristTown.TTT.Corpse;
using SCPTroubleInTerroristTown.TTT.Karma;
using SCPTroubleInTerroristTown.TTT.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.NonAllocLINQ;

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
        public TeamConfig teamsConfig { get; set; } = new TeamConfig();
        public RoundConfig roundConfig { get; set; } = new RoundConfig();

        public KarmaConfig karmaConfig { get; set; } = new KarmaConfig();

        public Hud.HUDConfig hudConfig { get; set; } = new Hud.HUDConfig();

        public CorpseConfig corpseConfig { get; set; } = new CorpseConfig();

        public TraitorTester.TraitorTesterConfig traitorTesterConfig { get; set; } = new TraitorTester.TraitorTesterConfig();

        public RoleTypeId spawnPoint { get; set; } = RoleTypeId.ClassD;
        public MapGeneration.FacilityZone spawnZone { get; set; } = MapGeneration.FacilityZone.LightContainment;

        public bool lockDownSpawnZone { get; set; } = true;

        public bool spawnDebugNPCS { get; set; } = false;
    }
}
