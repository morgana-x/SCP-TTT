using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.Handlers;
using System.Linq;

namespace SCP_SL_Trouble_In_Terrorist_Town
{
    public sealed class Plugin : Plugin<Config>
    {
        public override string Author => "morgana";

        public override string Name => "Trouble in Terrorist Town";

        public override string Prefix => Name;

        public static Plugin Instance;

        private EventHandlers _handlers;

        public TTTRound tttRound;
        public override void OnEnabled()
        {
            Instance = this;
            tttRound = new TTTRound(this.Config.TTTConfig);
            Exiled.API.Features.Server.IsHeavilyModded= true;
           
            RegisterEvents();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();

            Instance = null;

            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            _handlers = new EventHandlers(this);
            Exiled.Events.Handlers.Server.WaitingForPlayers += _handlers.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += _handlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RestartingRound += _handlers.OnRestartingRound;
            Exiled.Events.Handlers.Map.Generated += _handlers.OnMapLoaded;

            Exiled.Events.Handlers.Player.Verified += _handlers.OnVerified;
            Exiled.Events.Handlers.Player.Left += _handlers.OnLeave;
            Exiled.Events.Handlers.Player.Spawned += _handlers.OnPlayerSpawned;
            Exiled.Events.Handlers.Player.Died += _handlers.OnPlayerKilled;
            Exiled.Events.Handlers.Player.Hurt += _handlers.OnPlayerDamaged;

            Exiled.Events.Handlers.Player.SpawnedRagdoll += _handlers.OnSpawnedCorpse;

            Exiled.Events.Handlers.Server.RespawningTeam += _handlers.OnRespawningTeam;

            Exiled.Events.Handlers.Server.ReloadedConfigs += _handlers.OnConfigReloaded;

            Exiled.Events.Handlers.Player.ChangingRole += _handlers.OnChangingRole;

        }

        private void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= _handlers.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= _handlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RestartingRound -= _handlers.OnRestartingRound;
            Exiled.Events.Handlers.Map.Generated -= _handlers.OnMapLoaded;

            Exiled.Events.Handlers.Player.Verified -= _handlers.OnVerified;
            Exiled.Events.Handlers.Player.Left -= _handlers.OnLeave;
            Exiled.Events.Handlers.Player.Spawned -= _handlers.OnPlayerSpawned;
            Exiled.Events.Handlers.Player.Died -= _handlers.OnPlayerKilled;
            Exiled.Events.Handlers.Player.Hurt -= _handlers.OnPlayerDamaged;

            Exiled.Events.Handlers.Player.SpawnedRagdoll -= _handlers.OnSpawnedCorpse;

            Exiled.Events.Handlers.Server.RespawningTeam -= _handlers.OnRespawningTeam;

            Exiled.Events.Handlers.Server.ReloadedConfigs -= _handlers.OnConfigReloaded;

            Exiled.Events.Handlers.Player.ChangingRole -= _handlers.OnChangingRole;
            _handlers = null;
        }
    }
}