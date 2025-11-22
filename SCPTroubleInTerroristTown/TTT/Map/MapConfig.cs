using MapGeneration;
using System.Collections.Generic;

namespace SCPTroubleInTerroristTown.TTT.Map
{
    public class MapConfig
    {
        public RoomName spawnPoint { get; set; } = RoomName.HczWarhead;

        public bool lockDownSpawnZone { get; set; } = false;


        public List<ItemType> randomGunItemSpawns { get; set; } = new List<ItemType>() {

           // ItemType.GunA7, messes up death reason
            ItemType.GunAK,
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.GunCrossvec,
            ItemType.GunE11SR,
            ItemType.GunFSP9,
           // ItemType.GunFRMG0, messes up death reason
            ItemType.GunLogicer,
            ItemType.GunRevolver,
            ItemType.GunShotgun,
        };


        public List<ItemType> randomOtherItemSpawns { get; set; } = new List<ItemType>()
        {
            ItemType.ArmorLight,
            ItemType.Coin,
            ItemType.Flashlight,
            ItemType.GrenadeFlash,
            ItemType.GrenadeHE,
           // ItemType.Jailbird,
            ItemType.Lantern,
        };

    }
}
