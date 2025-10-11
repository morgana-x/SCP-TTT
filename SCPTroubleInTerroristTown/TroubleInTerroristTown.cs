
namespace SCPTroubleInTerroristTown
{
    using HarmonyLib;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Arguments.Scp914Events;
    using LabApi.Events.Arguments.ServerEvents;
    using LabApi.Features;
    using LabApi.Features.Console;
    using LabApi.Loader.Features.Plugins;
    using System;

    public enum DamageType
    {
        Firearm,
        Explosion,
        Jailbird,
        Poisoned,
        PocketDecay,
        Falldown,
        Hypothermia,
        Scp207,
        Scp173,
        Asphyxiated,
        Universal,
        PlayerLeft,
        Tesla,
        Bleeding,
        CardiacArrest,
        Checkpoint,
        ForcedDeath,
        Hemorrhage,
        MolecularDisruptor,
        GrenadeExplosion

    }

    public class TroubleInTerroristTown : Plugin<Config>
    {
        public override string Name { get; } = "Trouble in Terrorist Town";

        // The description of the plugin
        public override string Description { get; } = "The garry's mod gamemode, ported to SCP SL";
        // The author of the plugin
        public override string Author { get; } = "morgana";

        public override Version Version => new Version(1, 0);

        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

        public static TroubleInTerroristTown Singleton { get; private set; }

        private static readonly Harmony HarmonyPatcher = new("github.com/morgana-x/SCP-TTT");
        public TTT.Round.Round tttRound { get; private set; }

        public override void Enable()
        {
            Singleton = this;

            Logger.Info("Loading TTT...");

            LabApi.Events.Handlers.PlayerEvents.Death += OnPlayerDied;
            LabApi.Events.Handlers.PlayerEvents.Left += OnPlayerLeave;
            LabApi.Events.Handlers.PlayerEvents.ChangedRole += OnPlayerChangeRole;
            LabApi.Events.Handlers.PlayerEvents.Joined += OnPlayerJoin;
            LabApi.Events.Handlers.PlayerEvents.Hurt += OnPlayerDamage;
            LabApi.Events.Handlers.ServerEvents.RoundRestarted += RoundRestart;
            LabApi.Events.Handlers.ServerEvents.RoundStarted += RoundStart;
            LabApi.Events.Handlers.ServerEvents.MapGenerated += MapGenerated;
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers += WaitingForPlayers;
            LabApi.Events.Handlers.Scp914Events.Activating += Scp914Activate;
            LabApi.Events.Handlers.Scp914Events.ProcessedPlayer += Scp914ProcessPlayer;
            LabApi.Events.Handlers.PlayerEvents.SpawnedRagdoll += OnRagdollSpawn;

            PatchEvents.onPlayerTogglingNoclip += OnPlayerNoclip;
            HarmonyPatcher.PatchAll();

            tttRound = new TTT.Round.Round(Config.tttConfig);


        }

        public override void Disable()
        {
            LabApi.Events.Handlers.PlayerEvents.Death -= OnPlayerDied;
            LabApi.Events.Handlers.PlayerEvents.Left -= OnPlayerLeave;
            LabApi.Events.Handlers.PlayerEvents.ChangedRole -= OnPlayerChangeRole;
            LabApi.Events.Handlers.PlayerEvents.Joined -= OnPlayerJoin;
            LabApi.Events.Handlers.PlayerEvents.Hurt -= OnPlayerDamage;
            LabApi.Events.Handlers.ServerEvents.RoundRestarted -= RoundRestart;
            LabApi.Events.Handlers.ServerEvents.RoundStarted -= RoundStart;
            LabApi.Events.Handlers.ServerEvents.MapGenerated -= MapGenerated;
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers -= WaitingForPlayers;
            LabApi.Events.Handlers.Scp914Events.Activating -= Scp914Activate;
            LabApi.Events.Handlers.Scp914Events.ProcessedPlayer -= Scp914ProcessPlayer;
            LabApi.Events.Handlers.PlayerEvents.SpawnedRagdoll -= OnRagdollSpawn;
        }


        void OnPlayerDied(PlayerDeathEventArgs ev)
        {
            tttRound.OnPlayerDeath(ev.Player, ev.Attacker, ev.DamageHandler);
        }


        void OnPlayerLeave(PlayerLeftEventArgs ev)
        {
            tttRound.On_Player_Leave(ev.Player);
        }


        void OnPlayerChangeRole(PlayerChangedRoleEventArgs ev)// Player player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason) 
        {
            tttRound.OnPlayerChangeRole(ev.Player, ev.NewRole.RoleTypeId, ev.ChangeReason);
        }

        void OnPlayerJoin(PlayerJoinedEventArgs ev)
        {
            tttRound.On_Player_Joined(ev.Player);
        }
       
      
        void OnPlayerDamage(PlayerHurtEventArgs ev)
        {
            tttRound.OnPlayerHurt(ev.Player, ev.Attacker, ev.DamageHandler);
        }


        void MapGenerated(MapGeneratedEventArgs ev)
        {
            tttRound.On_Map_Loaded();
        }


        private void WaitingForPlayers()
        {
            tttRound.On_Waiting_For_Players();
        }

        void Scp914ProcessPlayer(Scp914ProcessedPlayerEventArgs ev)
        {
            tttRound.Scp914ProcessPlayer(ev.Player);
        }


        void Scp914Activate(Scp914ActivatingEventArgs ev)
        {
            ev.IsAllowed = tttRound.Scp914Activated(ev.Player);
        }
        void OnRagdollSpawn(PlayerSpawnedRagdollEventArgs ev)
        {
            tttRound.corpseManager.OnCorpseSpawn(ev.Player, ev.Ragdoll);
        }

        void OnPlayerNoclip(object sender, PatchEvents.ToggleNoclipArgs ev)
        {
            tttRound.OnPlayerToggleNoclip(ev.referenceHub);
        }

        void RoundStart()
        {
           tttRound.On_NewRound();
        }

        void RoundRestart()
        {
            tttRound.On_Round_Restarting();
        }


    }
}