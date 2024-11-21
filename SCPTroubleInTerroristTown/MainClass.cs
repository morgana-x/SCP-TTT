
namespace SCPTroubleInTerroristTown
{
    using HarmonyLib;
    using PlayerRoles;
    using PlayerRoles.Ragdolls;
    using PlayerStatsSystem;
    using PluginAPI.Core;
    using PluginAPI.Core.Attributes;
    using PluginAPI.Enums;
    using PluginAPI.Events;
    using Respawning;
    using Scp914;
    using UnityEngine;
    public class MainClass
    {
        public static MainClass Singleton { get; private set; }
        private static readonly Harmony HarmonyPatcher = new("github.com/morgana-x/SCP-TTT");
        public TTT.Round.Round tttRound { get; private set; }

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("Trouble in Terrorist Town", "1.0.0", "The garry's mod gamemode, ported to SCP SL", "morgana")]
        void LoadPlugin()
        {
            Singleton = this;

            Log.Info("Loading TTT...");

            EventManager.RegisterEvents(this);

            HarmonyPatcher.PatchAll();
            var handler = PluginHandler.Get(this);

            Log.Info(handler.PluginName);
            Log.Info(handler.PluginFilePath);
            Log.Info(handler.PluginDirectoryPath);

            tttRound = new TTT.Round.Round(config.tttConfig);

            RagdollManager.ServerOnRagdollCreated += OnRagdollSpawn;
            PatchEvents.onPlayerTogglingNoclip += OnPlayerNoclip;
        }
        [PluginConfig]
        public Config config;

        [PluginEvent(ServerEventType.PlayerDeath)]
        void OnPlayerDied(Player player, Player attacker, DamageHandlerBase damageHandler)
        {
            tttRound.OnPlayerDeath(player, attacker, damageHandler);
        }
        [PluginEvent(ServerEventType.PlayerLeft)]
        void OnPlayerLeave(Player player)
        {
            tttRound.On_Player_Leave(player);
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
        [PluginEvent(ServerEventType.RoundRestart)]
        void OnRestart()
        {
            tttRound.On_Round_Restarting();
        }
        [PluginEvent(ServerEventType.WaitingForPlayers)]
        void WaitingForPlayers()
        {
            tttRound.On_Waiting_For_Players();
        }

        [PluginEvent(ServerEventType.TeamRespawnSelected)]
        bool OnRespawn(SpawnableTeamType team)
        {
            return false;
        }

        [PluginEvent(ServerEventType.MapGenerated)]
        void MapGenerated()
        {
            tttRound.On_Map_Loaded();
        }

        [PluginEvent(ServerEventType.RoundStart)]
        void RoundStart()
        {
            tttRound.On_NewRound();
        }

        [PluginEvent(ServerEventType.Scp914ProcessPlayer)]
        void Scp914ProcessPlayer(Player hub, Scp914KnobSetting setting, Vector3 outPosition )
        {
            tttRound.Scp914ProcessPlayer(hub);
        }

        [PluginEvent(ServerEventType.Scp914Activate)]
        bool Scp914Activate(Player hub, Scp914KnobSetting setting)
        {
            return tttRound.Scp914Activated(hub);
        }
        void OnRagdollSpawn(ReferenceHub hub, BasicRagdoll Ragdoll)
        {
            tttRound.corpseManager.OnCorpseSpawn(hub, Ragdoll);
        }

        void OnPlayerNoclip(object sender, PatchEvents.ToggleNoclipArgs ev)
        {
            tttRound.OnPlayerToggleNoclip(ev.referenceHub);
        }
    }
}