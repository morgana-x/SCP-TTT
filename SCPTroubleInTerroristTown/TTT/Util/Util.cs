using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using PlayerRoles.FirstPersonControl.Spawnpoints;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Enums;
using RoundRestarting;
using System;
using System.Linq;
using UnityEngine;
using MapGeneration;

namespace SCPTroubleInTerroristTown.TTT.Util
{
    public class Util
    {
        public static void LockdownZones()
        {
            foreach (var door in DoorVariant.AllDoors.Where((x) => x is ElevatorDoor))
            {
                door.ServerChangeLock(DoorLockReason.AdminCommand, true);
            }
        }
        public static void LockdownEntrance()
        {
            foreach (var door in DoorVariant.AllDoors)
            {
                if (!door.IsInZone(FacilityZone.Entrance))
                    continue;
                door.ServerChangeLock(DoorLockReason.AdminCommand, true);
            }
        }
        public static void RestartServer()
        {
            if (!ServerStatic.IsDedicated)
            {
                return;
            }
            RoundRestart.InitiateRoundRestart();
        }
        public static DamageType getDamageTypeFromHandler(DamageHandlerBase dmgbase)
        {
            string logs = dmgbase.ServerLogsText;
            if (dmgbase is FirearmDamageHandler)
            {
                return DamageType.Firearm;
            }
            if (dmgbase is ExplosionDamageHandler)
            {
                return DamageType.Explosion;
            }
            if (dmgbase is JailbirdDamageHandler)
            {
                return DamageType.Jailbird;
            }
            logs = logs.ToLower();
            if (logs.Contains("poison"))
            {
                return DamageType.Poisoned;
            }
            if (logs.Contains("pocket"))
            {
                return DamageType.PocketDecay;
            }
            if (logs.Contains("fall") || logs.Contains("fell"))
            {
                return DamageType.Falldown;
            }
            if (logs.Contains("hypothermia"))
            {
                return DamageType.Hypothermia;
            }
            if (logs.Contains("207"))
            {
                return DamageType.Scp207;
            }
            if (logs.Contains("173"))
            {
                return DamageType.Scp173;
            }
            if (logs.Contains("shot"))
            {
                return DamageType.Firearm;
            }
            if (logs.Contains("jailbird"))
            {
                return DamageType.Jailbird;
            }
            if (logs.Contains("asphyxiated") || logs.Contains("strangle"))
            {
                return DamageType.Asphyxiated;
            }
            return DamageType.Universal;

        }
        public static ItemType getAmmoType(ItemType type)
        {
            switch (type)
            {
                case ItemType.GunCOM15:
                case ItemType.GunCOM18:
                case ItemType.GunCom45:
                case ItemType.GunCrossvec:
                case ItemType.GunFSP9:
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
        public static bool isHeadshot(string serverLogsText)
        {
            return serverLogsText.Contains("Headshot");
        }
        public static DamageType getDamageType(string serverLogsText)
        {
            var dmgType = DamageType.PlayerLeft;
            foreach (DamageType type in Enum.GetValues(typeof(DamageType)).Cast<DamageType>())
            {
                if (serverLogsText.ToLower().Contains(type.ToString().ToLower()))
                {
                    dmgType = type;
                    break;
                }

            }
            return dmgType;
        }

        public static ItemType getItemType(DamageType dmg)
        {
            var dmgType = ItemType.None;

            foreach (ItemType type in Enum.GetValues(typeof(ItemType)).Cast<ItemType>())
            {
                if ("Gun" + dmg.ToString() == type.ToString())
                {
                    dmgType = type;
                    break;
                }

            }
            return dmgType;
        }

        public static ItemType getFirearmType(DamageType dmg)
        {
            var dmgType = ItemType.None;
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)).Cast<ItemType>())
            {
                if (dmg.ToString().ToLower() == type.ToString().ToLower())
                {
                    dmgType = type;
                    break;
                }

            }
            return dmgType;
        }
        public static void gotoRoleSpawn(PluginAPI.Core.Player pl, RoleTypeId spawnPointRole = RoleTypeId.None)
        {
            if (spawnPointRole != RoleTypeId.None) // Teleport to spawnpoint
            {
                ISpawnpointHandler spawnpoint = null;
                Vector3 spawnpointPos = Vector3.zero;
                float rot = 0f;
                RoleSpawnpointManager.TryGetSpawnpointForRole(spawnPointRole, out spawnpoint);
                if (!RoleSpawnpointManager.TryGetSpawnpointForRole(spawnPointRole, out spawnpoint))
                {
                    return;
                }
                if (!spawnpoint.TryGetSpawnpoint(out spawnpointPos, out rot))
                {
                    return;
                }
                pl.Position = spawnpointPos;
            }
        }
        static System.Random rnd = new System.Random(); 
        public static void gotoRoom(PluginAPI.Core.Player pl, RoomName roomName)
        {
            if (roomName == RoomName.Unnamed)
                return;
            RoomIdentifier ident = null;

            bool success = RoomIdUtils.TryFindRoom(roomName, MapGeneration.FacilityZone.HeavyContainment, RoomShape.Undefined, out ident);
            if (!success)
            {
              
                return;
            }
            float negativeX = rnd.Next(10) > 5 ? 1f : -1f;
            float negativeY = rnd.Next(10) > 5 ? 1f : -1f;
            float magnitude = 1.5f;
            Vector3 randomOffset = new Vector3((float)rnd.NextDouble() * magnitude * negativeX, (float)0, (float)rnd.NextDouble() * magnitude * negativeY);
            pl.Position = ident.ApiRoom.Position + (Vector3.up * 1.5f) + randomOffset;
        }
    }
}
