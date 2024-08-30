﻿
namespace SCPTroubleInTerroristTown
{
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.Ragdolls;
    using PlayerStatsSystem;
    using PluginAPI.Core;
    using PluginAPI.Core.Attributes;
    using PluginAPI.Core.Factories;
    using PluginAPI.Core.Zones;
    using PluginAPI.Core.Zones.Light.Rooms;
    using PluginAPI.Enums;
    using PluginAPI.Events;
    using Respawning;
    using Scp914;
    using SCPTroubleInTerroristTown.TTT;
    using System.Linq;
    using UnityEngine;
    using YamlDotNet.Core.Tokens;
    public class MainClass
    {

        public static MainClass Singleton { get; private set; }
        public TTT.Round tttRound { get; private set; }

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("Trouble in Terrorist Town", "1.0.0", "The garry's mod gamemode, ported to SCP SL", "morgana")]
        void LoadPlugin()
        {
            Singleton = this;

            Log.Info("Loading TTT...");

            EventManager.RegisterEvents(this);


            Log.Info($"Registered events, register factory...");


            var handler = PluginHandler.Get(this);

            Log.Info(handler.PluginName);
            Log.Info(handler.PluginFilePath);
            Log.Info(handler.PluginDirectoryPath);

            tttRound = new TTT.Round(config.tttConfig);

            RagdollManager.ServerOnRagdollCreated += OnRagdollSpawn;

           

        }
        [PluginConfig]
        public Config config;

        [PluginEvent(ServerEventType.PlayerDeath)]
        void OnPlayerDied(Player player, Player attacker, DamageHandlerBase damageHandler)
        {
            tttRound.OnPlayerDeath(player, attacker);

        }
        [PluginEvent(ServerEventType.PlayerLeft)]
        void OnPlayerLeave(Player player)
        {
            tttRound.On_Player_Leave(player);
            Log.Info($"Player &6{player.UserId}&r left this server");

        }

        [PluginEvent(ServerEventType.PlayerChangeRole)]
        void OnPlayerChangeRole(Player player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason) 
        {
            tttRound.OnPlayerChangeRole(player, newRole, reason);
        }
        [PluginEvent(ServerEventType.PlayerJoined)]
        void OnPlayerJoin(Player player)
        {
            tttRound.On_Player_Joined(player);
        }
       
        [PluginEvent(ServerEventType.PlayerDamage)]
        void OnPlayerDamage(Player player, Player target, DamageHandlerBase damageHandler)
        {
            tttRound.OnPlayerHurt(target, player, damageHandler);
        }
        void OnRagdollSpawn(ReferenceHub hub, BasicRagdoll Ragdoll)
        {
            Player plr = Player.Get(hub);
            var damageHandler = Ragdoll.Info.Handler;
            NetworkServer.UnSpawn(Ragdoll.gameObject);
            var newDamageHandler = tttRound.OnSpawnedCorpse(plr, damageHandler, damageHandler.ServerLogsText);
            Ragdoll.NetworkInfo = new RagdollData(Ragdoll.NetworkInfo.OwnerHub, newDamageHandler, Ragdoll.NetworkInfo.RoleType, Ragdoll.NetworkInfo.StartPosition, Ragdoll.NetworkInfo.StartRotation, Ragdoll.NetworkInfo.Nickname, Ragdoll.NetworkInfo.CreationTime);
            NetworkServer.Spawn(Ragdoll.gameObject);
           
        }

        [PluginEvent(ServerEventType.RoundRestart)]
        void OnRestart()
        {
            tttRound.On_Round_Restarting();
            Log.Info($"Round restarting");

        }
        [PluginEvent(ServerEventType.WaitingForPlayers)]
        void WaitingForPlayers()
        {
            tttRound.On_Waiting_For_Players();
            Log.Info($"Waiting for players...");

        }

        [PluginEvent(ServerEventType.TeamRespawnSelected)]
        public bool OnRespawn(SpawnableTeamType team)
        {
            Log.Debug("Cancelling respawn!");
            return false;
        }

        [PluginEvent(ServerEventType.MapGenerated)]
        public void MapGenerated()
        {
            tttRound.On_Map_Loaded();
        }

        [PluginEvent(ServerEventType.RoundStart)]
        public void RoundStart()
        {
            tttRound.On_NewRound();
        }

        [PluginEvent(ServerEventType.Scp914ProcessPlayer)]
        public void Scp914ProcessPlayer(Player hub, Scp914KnobSetting setting, Vector3 outPosition )
        {
            Log.Debug("Processing " + hub.DisplayNickname);
            tttRound.Scp914ProcessPlayer(hub);
        }

        [PluginEvent(ServerEventType.Scp914Activate)]
        public bool Scp914Activate(Player hub, Scp914KnobSetting setting)
        {

            Log.Debug("SCP914 activated by " + hub.DisplayNickname);
            return tttRound.Scp914Activated(hub);
        }

    }
}