using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using PlayerRoles;
using PluginAPI.Roles;
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



        public static List<Pickup> SpawnWeapons(List<WeaponSpawnPoint> weapons)
        {
            List<Pickup> spawned = new List<Pickup>();
            foreach( var w in weapons) 
            {

                var i = Pickup.CreateAndSpawn(w.Item, w.Location, Quaternion.Euler(0, 0, 0));
                spawned.Add(i);
            }
            Exiled.API.Features.Log.Debug("Spawned " + spawned.Count + " weapons!");
            return spawned;
        }
        private static List<Pickup> safeSpawnPickup(ItemType type, Vector3 position)
        {
            List<Pickup> pickups = new List<Pickup>();
            if (position == null)
            {
                Log.Debug("Warning position is null!");
                return pickups;
            }
            if (type == ItemType.None)
            {
                return pickups;
            }
            var p = Pickup.CreateAndSpawn(type, position, Quaternion.Euler(0, 0, 0));
            if (type.IsAmmo())
            {
                p.As<AmmoPickup>().Ammo = 100;
            }
            pickups.Add(p);
            if (p.Type.IsWeapon())
            {
                var ammoItem = p.Type.GetFirearmType().GetWeaponAmmoType().GetItemType();
                if (ammoItem != ItemType.None)
                {
                    pickups.AddRange(safeSpawnPickup(ammoItem, position));
                }
            }
            return pickups;
        }
        public static List<Pickup> SpawnRandomWeapons(ZoneType zone) // Awful temporary code! Forgive me my sins
        {
            List<Pickup> spawned = new List<Pickup>();
            foreach (Room room in Room.Get(zone))
            {
                /*if (room.Type == RoomType.LczArmory || room.Type == RoomType.Lcz173 || room.Type == RoomType.Lcz914 || room.Type == RoomType.HczArmory)
                {
                    try
                    {
                        List<Pickup> toSpawn = new List<Pickup>();
                        foreach (Pickup ogpickup in room.Pickups)
                        {
                            toSpawn.Add(ogpickup);
                        }
                        foreach(var ogpickup in toSpawn)
                        { 
                            if (ogpickup.Type.IsWeapon())
                            {
                                for (int i = 0; i < 1; i++)
                                {
                                    var gunType = randomGuns.RandomItem();
                                    var pickup = safeSpawnPickup(gunType, ogpickup.Position + Vector3.up);
                                    spawned.AddRange(pickup);
                                }
                            }
                            else if (!ogpickup.Type.IsScp() && !ogpickup.Type.IsAmmo()) ;
                            {
                                for (int i = 0; i < 1; i++)
                                {
                                    var pickup = safeSpawnPickup(randomEtc.RandomItem(), ogpickup.Position + Vector3.up);
                                    spawned.AddRange(pickup);
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Log.Debug(e);
                    }
                }
                else */if (room.RoomShape == MapGeneration.RoomShape.XShape || room.RoomShape == MapGeneration.RoomShape.TShape)
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
                else if (room.Type == RoomType.LczClassDSpawn)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        Vector3 center = RoleTypeId.ClassD.GetRandomSpawnLocation().Position; // Wow what can go wrong!
                        var pickup = safeSpawnPickup(randomGuns.RandomItem(), center + Vector3.up);
                        spawned.AddRange(pickup);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        Vector3 center = RoleTypeId.ClassD.GetRandomSpawnLocation().Position; // Wow what can go wrong!
                        center = center + Vector3.right + Vector3.up;
                        var pickup = safeSpawnPickup(randomEtc.RandomItem(), center);
                        spawned.AddRange(pickup);
                    }
                }
            }
            Exiled.API.Features.Log.Debug("Spawned " + spawned.Count + " weapons!");
            return spawned;
        }
    }
}
