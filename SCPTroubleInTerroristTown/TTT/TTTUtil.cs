using CentralAuth;
using CommandSystem.Commands.RemoteAdmin;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Doors;
using PluginAPI.Core.Zones;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCPTroubleInTerroristTown.TTT
{
    public class TTTUtil
    {
        private static System.Random rng = new System.Random();

        public static void RandomShuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void LockdownZones()
        {
            foreach (var door in DoorVariant.AllDoors.Where((x) => x is ElevatorDoor))//.Where((x) => x.Zone == zone))
            {
                door.ServerChangeLock(Interactables.Interobjects.DoorUtils.DoorLockReason.AdminCommand, true);
               // Log.Debug("Locking door!");
            }
        }

        public static void RestartServer()
        {

            if (!ServerStatic.IsDedicated)
            {
                return;
            }

            //Round.Restart()
            RoundRestart.InitiateRoundRestart();
            //ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
            //RoundRestart.
            //RoundRestart.ChangeLevel(noShutdownMessage: true);
        }
      
    }
}
