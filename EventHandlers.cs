using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

namespace SCP_SL_Trouble_In_Terrorist_Town
{
    public class EventHandlers
    {
        public Plugin pluginRef;
        public EventHandlers(Plugin plugin)
        {
            this.pluginRef = plugin;
        }
        public void OnWaitingForPlayers()
        {
            pluginRef.tttRound.On_Waiting_For_Players();
        }
        public void OnRoundStarted()
        {
            
            pluginRef.tttRound.On_NewRound();
        }
        public void OnRestartingRound() 
        {
            pluginRef.tttRound.On_Round_Restarting();
        }
        public void OnVerified(VerifiedEventArgs ev)
        {
            pluginRef.tttRound.On_Player_Joined(ev.Player);
        }
        public void OnLeave(LeftEventArgs ev)
        {
            pluginRef.tttRound.On_Player_Leave(ev.Player);
        }
        public void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            pluginRef.tttRound.OnPlayerSpawned(ev.Player);
        }
        public void OnPlayerDamaged(HurtEventArgs ev)
        {
            pluginRef.tttRound.OnPlayerHurt(ev.Player, ev.Attacker, ev.DamageHandler.Type);
        }
        public void OnPlayerKilled(DiedEventArgs ev)
        {
            pluginRef.tttRound.OnPlayerDeath(ev.Player, ev.Attacker);
        }
        public void OnSpawnedCorpse(SpawnedRagdollEventArgs ev)
        {
            pluginRef.tttRound.OnSpawnedCorpse(ev.Player, ev.Ragdoll, ev.Info);
        }
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Reason == Exiled.API.Enums.SpawnReason.RoundStart) // TTT doesn't use this flag... yet
            {
                Log.Debug("Stopped original class change at start of round!");
                ev.IsAllowed = false;
            }
        }
        public void OnMapLoaded()
        {
            pluginRef.tttRound.On_Map_Loaded();
        }
        public void OnConfigReloaded()
        {
            pluginRef.tttRound.config = pluginRef.Config.TTTConfig;
            Log.Debug("Updated config!");
        }
        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}