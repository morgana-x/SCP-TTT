using MapGeneration;
using PlayerRoles;
using PluginAPI.Core.Zones;

namespace SCPTroubleInTerroristTown.TTT.Map
{
    public class MapConfig
    {
        public RoomName spawnPoint { get; set; } = RoomName.HczWarhead;

        public bool lockDownSpawnZone { get; set; } = false;
    }
}
