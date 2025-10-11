using LabApi.Features.Wrappers;
using LightContainmentZoneDecontamination;
using System.Linq;

namespace SCPTroubleInTerroristTown.TTT.Map
{
    public class MapManager
    {
        private Round.Round tttRound;

        public MapManager(Round.Round round) 
        {
            this.tttRound = round;
        }

        public void InitMap()
        {
            DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
            if (tttRound.config.mapConfig.lockDownSpawnZone) // Lock down elevators to decrease play area, game gets boring if map is too large (probably) 
            {
                Util.Util.LockdownZones();
            }
            Util.Util.LockdownEntrance();
            // Disable CASSIE
            //Cassie.Announcer.enabled = false;
            // Disable Decontamination
        }
        public void onMapLoaded()
        {
            TTTWeaponSpawner.SpawnRandomWeapons(Room.Get(tttRound.config.mapConfig.spawnPoint).First().Zone);
        }

    }
}
