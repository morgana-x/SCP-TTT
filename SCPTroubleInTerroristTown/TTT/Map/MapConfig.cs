using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPTroubleInTerroristTown.TTT.Map
{
    public class MapConfig
    {
        public RoleTypeId spawnPoint { get; set; } = RoleTypeId.ClassD;
        public MapGeneration.FacilityZone spawnZone { get; set; } = MapGeneration.FacilityZone.LightContainment;

        public bool lockDownSpawnZone { get; set; } = true;
    }
}
