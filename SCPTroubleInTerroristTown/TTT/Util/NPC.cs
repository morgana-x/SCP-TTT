using PlayerRoles;
using LabApi.Features.Wrappers;
using NetworkManagerUtils.Dummies;

namespace SCPTroubleInTerroristTown.TTT.Util
{
    internal class NPC
    {
        public static Player Spawn(string name, RoleTypeId role)
        {
            var refhub = DummyUtils.SpawnDummy(name);
            refhub.roleManager.ServerSetRole(role, RoleChangeReason.RoundStart);
            return Player.Get(refhub);
        }

        public static void SpawnNpcs(int numOfNpcs)
        {
            for (int i = 0; i < numOfNpcs; i++)
            {
                var n = Spawn("Bob", RoleTypeId.Spectator);
            }
            LabApi.Features.Console.Logger.Info($"Spawned {numOfNpcs} npcs!");
        }
    }
}
