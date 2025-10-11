using System;
using Mirror;
using PlayerRoles;
using UnityEngine;
using LabApi.Features.Wrappers;
using NetworkManagerUtils.Dummies;
using LabApi.Features.Console;

namespace SCPTroubleInTerroristTown.TTT.Util
{
    public class FakeConnection : NetworkConnectionToClient
    {
        public override string address => "localhost";

        public FakeConnection(int networkConnectionId)
            : base(networkConnectionId)
        {
        }

        public override void Send(ArraySegment<byte> segment, int channelId = 0)
        {
        }
    }
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
