
namespace SCPTroubleInTerroristTown
{
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.Ragdolls;
    using PlayerStatsSystem;
    using PluginAPI.Core;
    using PluginAPI.Core.Attributes;
    using PluginAPI.Core.Factories;
    using PluginAPI.Enums;
    using PluginAPI.Events;
    using Respawning;
    using SCPTroubleInTerroristTown.TTT;
    using YamlDotNet.Core.Tokens;
    public class MainClass
    {

        public static MainClass Singleton { get; private set; }
        public TTTRound tttRound { get; private set; }

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("Trouble in Terrorist Town", "1.0.0", "The garry's mod gamemode, ported to SCP SL", "morgana")]
        void LoadPlugin()
        {
            Singleton = this;

            Log.Info("Loading TTT...");

            // I am so confused
            EventManager.RegisterEvents(this);
            //EventManager.RegisterEvents<EventHandlers>(this);


            Log.Info($"Registered events, register factory...");

            // Don't need factory?
            //FactoryManager.RegisterPlayerFactory(this, new MyPlayerFactory());

            var handler = PluginHandler.Get(this);

            Log.Info(handler.PluginName);
            Log.Info(handler.PluginFilePath);
            Log.Info(handler.PluginDirectoryPath);


            // Todo: Add config!

            TTTConfig tttConfig = new TTTConfig()
            { 
                spawnDebugNPCS = true
            };
            tttRound = new TTTRound(tttConfig);

            RagdollManager.ServerOnRagdollCreated += OnRagdollSpawn;

        }
        [PluginEvent(ServerEventType.PlayerDeath)]
        void OnPlayerDied(Player player, Player attacker, DamageHandlerBase damageHandler)
        {
            tttRound.OnPlayerDeath(player, attacker);
            if (attacker == null)
                Log.Info($"Player &6{player.Nickname}&r (&6{player.UserId}&r) died, cause {damageHandler}");
            else
                Log.Info($"Player &6{attacker.Nickname}&r (&6{attacker.UserId}&r) killed &6{player.Nickname}&r (&6{player.UserId}&r), cause {damageHandler}");


        }
        [PluginEvent(ServerEventType.PlayerLeft)]
        void OnPlayerLeave(Player player)
        {
            tttRound.On_Player_Leave(player);
            Log.Info($"Player &6{player.UserId}&r left this server");

        }
        [PluginEvent(ServerEventType.PlayerJoined)]
        void OnPlayerJoin(Player player)
        {
            tttRound.On_Player_Joined(player);
            Log.Info($"Player &6{player.UserId}&r joined this server");
        }
       
        [PluginEvent(ServerEventType.PlayerDamage)]
        void OnPlayerDamage(Player player, Player target, DamageHandlerBase damageHandler)
        {
            tttRound.OnPlayerHurt(target, player, damageHandler);
            if (player == null)
                Log.Info($"Player &6{target.Nickname}&r (&6{target.UserId}&r) got damaged, cause {damageHandler}.");
            else
                Log.Info($"Player &6{target.Nickname}&r (&6{target.UserId}&r) received damage from &6{player.Nickname}&r (&6{player.UserId}&r), cause {damageHandler}.");
        }
        void OnRagdollSpawn(ReferenceHub hub, BasicRagdoll Ragdoll)
        {
            Player plr = Player.Get(hub);
            var damageHandler = Ragdoll.Info.Handler;
            Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) spawned ragdoll &6{Ragdoll}&r, reason &6{damageHandler}&r");

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
    }
}