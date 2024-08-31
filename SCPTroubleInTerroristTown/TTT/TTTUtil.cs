using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using RoundRestarting;
using System.Linq;

namespace SCPTroubleInTerroristTown.TTT
{
    public class TTTUtil
    {
        public static void LockdownZones()
        {
            foreach (var door in DoorVariant.AllDoors.Where((x) => x is ElevatorDoor))
            {
                door.ServerChangeLock(Interactables.Interobjects.DoorUtils.DoorLockReason.AdminCommand, true);
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
    }
}
