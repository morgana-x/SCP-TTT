
using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using MapGeneration;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Items;
using PluginAPI.Core.Zones;
using PluginAPI.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCPTroubleInTerroristTown.TTT
{
    public class WeaponSpawnPoint
    {
        public Vector3 Location { get; set; }
        public ItemType Item { get; set; }

    }
    public class TTTWeaponSpawner
    {
        private static List<ItemType> randomGuns = new List<ItemType>() {

            ItemType.GunA7,
            ItemType.GunAK,
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.GunCrossvec,
            ItemType.GunE11SR,
            ItemType.GunFSP9,
            ItemType.GunFRMG0,// Maybe gun doesn't exist?
            ItemType.GunLogicer,
            ItemType.GunRevolver,
            ItemType.GunShotgun,
        };


        private static List<ItemType> randomEtc = new List<ItemType>()
        {
            ItemType.ArmorCombat,
            ItemType.ArmorHeavy,
            ItemType.ArmorLight,
            ItemType.Coin,
            ItemType.Flashlight,
            ItemType.GrenadeFlash,
            ItemType.GrenadeHE,
        };



        public static List<ItemPickup> SpawnWeapons(List<WeaponSpawnPoint> weapons)
        {
            List<ItemPickup> spawned = new List<ItemPickup>();
            foreach( var w in weapons) 
            {

                var i = ItemPickup.Create(w.Item, w.Location, Quaternion.Euler(0, 0, 0));
                i.Spawn();
                spawned.Add(i);
            }
            Log.Debug("Spawned " + spawned.Count + " weapons!");
            return spawned;
        }
        private static ItemType GetWeaponAmmoType(ItemType type)
        {
            switch (type)
            {
                case ItemType.GunCOM15:
                case ItemType.GunCOM18:
                case ItemType.GunCrossvec:
                case ItemType.GunFSP9:
                case ItemType.GunCom45:
                    return ItemType.Ammo9x19;
                case ItemType.GunE11SR:
                case ItemType.GunFRMG0:
                    return ItemType.Ammo556x45;
                case ItemType.GunLogicer:
                case ItemType.GunAK:
                case ItemType.GunA7:
                    return ItemType.Ammo762x39;
                case ItemType.GunRevolver:
                    return ItemType.Ammo44cal;
                case ItemType.GunShotgun:
                    return ItemType.Ammo12gauge;
                default:
                    return ItemType.None;
            }
        }
        private static List<ItemPickup> safeSpawnPickup(ItemType type, Vector3 position)
        {
            List<ItemPickup> pickups = new List<ItemPickup>();
            if (position == null)
            {
                Log.Debug("Warning position is null!");
                return pickups;
            }
            if (type == ItemType.None)
            {
                return pickups;
            }
         
            var p = ItemPickup.Create(type, position, Quaternion.Euler(0, 0, 0));
            
            if (type.ToString().StartsWith("Ammo"))
            {
                
            }
            p.Spawn();
            pickups.Add(p);
         
            if (p.Type.ToString().StartsWith("Gun"))
            {
                var ammoItem = GetWeaponAmmoType(p.Type);//p.Type //p.Type.GetFirearmType().GetWeaponAmmoType().GetItemType();
                if (ammoItem != ItemType.None)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        pickups.AddRange(safeSpawnPickup(ammoItem, position));
                    }
                }
            }
            return pickups;
        }
        public static List<ItemPickup> SpawnRandomWeapons(MapGeneration.FacilityZone zone) // Awful temporary code! Forgive me my sins
        {
            
            List<ItemPickup> spawned = new List<ItemPickup>();
            foreach (FacilityRoom room in Facility.Rooms.Where((x) => x.Zone.ZoneType == zone ))
            {
                if (room.Identifier.Shape == MapGeneration.RoomShape.XShape || room.Identifier.Shape == MapGeneration.RoomShape.TShape)
                {
                    Vector3 center = room.Position + (Vector3.up * 2); // ::pray::
                    for (int i = 0; i < 2; i++)
                    {
                        var pickup = safeSpawnPickup(randomGuns.RandomItem(), center + Vector3.up);
                        spawned.AddRange(pickup);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        var pickup = safeSpawnPickup(randomEtc.RandomItem(), center + Vector3.right + Vector3.up);
                        spawned.AddRange(pickup);
                    }
                }
              
            }
            Log.Debug("Spawned " + spawned.Count + " weapons!");
            return spawned;
        }
    }
}
