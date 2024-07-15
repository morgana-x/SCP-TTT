﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CentralAuth;

using Mirror;

using PlayerRoles;

using UnityEngine;
using PluginAPI.Core;

namespace SCPTroubleInTerroristTown.TTT
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
    internal class TTTNPC
    {
        public static Player Spawn(string name, RoleTypeId role, int id = 0, string userId = "")
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
   
            Player npc = new Player(ReferenceHub.GetHub(gameObject));
            try
            {
                npc.ReferenceHub.roleManager.InitializeNewRole(RoleTypeId.None, RoleChangeReason.None);
            }
            catch (Exception arg)
            {
               // Log.Debug($"Ignore: {arg}");
            }


            int freeId = 0;
            /*
            if (!RecyclablePlayerId.FreeIds.Contains(id) && RecyclablePlayerId._autoIncrement >= id)
            {
                Log.Warn(Assembly.GetCallingAssembly().GetName().Name + " tried to spawn an NPC with a duplicate PlayerID. Using auto-incremented ID instead to avoid issues.");
                id = new RecyclablePlayerId(useMinQueue: false).Value;
            }*/
            for (int i = 16; i < 100; i++)
            {
                try
                {
                    if (Player.Get(i) != null)
                    {
                        continue;
                    }
                }
                catch(Exception ex) 
                {
                    Log.Debug(ex.ToString());
                    continue;
                }
                freeId = i;
                break;
            }
            id = freeId;

            NetworkServer.AddPlayerForConnection(new FakeConnection(id), gameObject);
            try
            {
                npc.ReferenceHub.authManager.UserId = ((userId == string.Empty) ? "Dummy@localhost" : userId);
            }
            catch (Exception arg3)
            {
                Log.Debug($"Ignore: {arg3}");
            }

            npc.ReferenceHub.nicknameSync.Network_myNickSync = name;

            /*Timing.CallDelayed(0.5f, delegate
            {
                npc.Role.Set(role, SpawnReason.RoundStart, RoleSpawnFlags.All);
            });*/


            return npc;
        }
    }
}