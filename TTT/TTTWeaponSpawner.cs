using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCP_SL_Trouble_In_Terrorist_Town.TTT
{
    public class WeaponSpawnPoint
    {
        public Vector3 Location { get; set; }
        public ItemType Item { get; set; }

        public RoomType Room { get; set; } // Todo: Room stuff
    }
    public class TTTWeaponSpawner
    {
        public static List<WeaponSpawnPoint> tempWeaponList = new List<WeaponSpawnPoint>()
        {
            new WeaponSpawnPoint(){
                Item = ItemType.GunE11SR,
                Location = new Vector3(121,996,-41)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo556x45,
                Location = new Vector3(123,996,-41)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo12gauge,
                Location = new Vector3(122,996,-41)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunAK,
                Location = new Vector3(114,996,-39)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo762x39,
                Location = new Vector3(121,996,-41)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunCOM18,
                Location = new Vector3(130,996,-59)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GrenadeHE,
                Location = new Vector3(130,996,-59)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo9x19,
                Location = new Vector3(121,996,-41)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunFRMG0,
                Location = new Vector3(140,996,-50)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunShotgun,
                Location = new Vector3(89,996,-36)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo12gauge,
                Location = new Vector3(89,996,-36)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunRevolver,
                Location = new Vector3(64,996,-36)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo762x39,
                Location = new Vector3(64,996,-36)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunE11SR,
                Location = new Vector3(62,992,-50)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo9x19,
                Location = new Vector3(64,996,-36)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo556x45,
                Location = new Vector3(64,996,-36)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunE11SR,
                Location = new Vector3(74,992,-44)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunShotgun,
                Location = new Vector3(91,993,-43)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo12gauge,
                Location = new Vector3(91,993,-43)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunCOM15,
                Location = new Vector3(136,996,-20)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo9x19,
                Location = new Vector3(136,996,-20)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunE11SR,
                Location = new Vector3(136,994,-1)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo556x45,
                Location = new Vector3(136,994,-1)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunE11SR,
                Location = new Vector3(130,989,20)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo556x45,
                Location = new Vector3(130,989,20)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GrenadeHE,
                Location = new Vector3(130,989,20)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunCOM15,
                Location = new Vector3(29,992,-26)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.ArmorHeavy,
                Location = new Vector3(29,992,-26)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.ArmorLight,
                Location = new Vector3(0,1001,-41)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunLogicer,
                Location = new Vector3(0,1001,-41)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo762x39,
                Location = new Vector3(0,1001,-41)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo9x19,
                Location = new Vector3(0,1001,1)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunCrossvec,
                Location = new Vector3(0,1001,1)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunRevolver,
                Location = new Vector3(-13,1001,0.5f)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo44cal,
                Location = new Vector3(-13,1001,0.5f)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Flashlight,
                Location = new Vector3(-13,1001,0.5f)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunFSP9,
                Location = new Vector3(-11,992,-35f)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunFSP9,
                Location = new Vector3(140,996,-35f)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo9x19,
                Location = new Vector3(140,996,-35f)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.ArmorLight,
                Location = new Vector3(140,996,-35f)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunCrossvec,
                Location = new Vector3(118,996,-34f)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo9x19,
                Location = new Vector3(118,996,-34f)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.ArmorLight,
                Location = new Vector3(118,996,-34f)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GunRevolver,
                Location = new Vector3(113,996,-34f)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.Ammo44cal,
                Location = new Vector3(113,996,-34f)
            },
            new WeaponSpawnPoint(){
                Item = ItemType.GrenadeFlash,
                Location = new Vector3(113,996,-34f)
            },
        };

        public static List<Pickup> SpawnWeapons(List<WeaponSpawnPoint> weapons)
        {
            List<Pickup> spawned = new List<Pickup>();
            foreach( var w in weapons) 
            {

                var i = Pickup.CreateAndSpawn(w.Item, w.Location, Quaternion.Euler(0, 0, 0));
                spawned.Add(i);
            }
            Exiled.API.Features.Log.Debug("Spawned " + weapons.Count + " weapons!");
            return spawned;
        }
    }
}
