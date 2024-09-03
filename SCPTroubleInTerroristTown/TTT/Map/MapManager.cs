using LightContainmentZoneDecontamination;
using SCPTroubleInTerroristTown.TTT.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPTroubleInTerroristTown.TTT.Map
{
    public class MapManager
    {
        private Round tttRound;

        public MapManager(Round round) 
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
            // Disable CASSIE
            //Cassie.Announcer.enabled = false;
            // Disable Decontamination
        }
        public void onMapLoaded()
        {
            TTTWeaponSpawner.SpawnRandomWeapons(tttRound.config.mapConfig.spawnZone);
        }

    }
}
