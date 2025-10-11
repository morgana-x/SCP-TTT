using LabApi.Features.Wrappers;
using MapGeneration;
using Mirror;
using Scp914;
using System.Linq;
using UnityEngine;

namespace SCPTroubleInTerroristTown.TTT.TraitorTester
{
    public class TraitorTester
    {
        public bool traitorDetected = false;
       
        public Room scp914Room = null;
        public LightSourceToy lightSource = null;
        /// <summary>
        /// A primitive base
        /// </summary>
        public LightSourceToy Base;

        public Round.Round round;

        private LightSourceToy ToyPrefab
        {
            get
            {
                if (Base == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                        if (gameObject.TryGetComponent<LightSourceToy>(out var component))
                            Base = component;
                }

                return Base;
            }
        }
        public void Init()
        {
            getScp914Room();
            SpawnLight();
        }

        public TraitorTester(Round.Round round)
        {
            this.round = round;
        }
        private void SpawnLight()
        {
            if (ToyPrefab == null)
            {
                LabApi.Features.Console.Logger.Error("[TRAITOR TESTOR] Couldn't instantiate light prefab, Was NULL!\nWill try to use room light color instead.");
                return;
            }
            lightSource = LightSourceToy.Create();
            lightSource.Position = scp914Room.Position + (Vector3.up * 1f); 
            lightSource.Intensity = 20f;
            lightSource.Range = 15f;
            
            lightSource.Color = UnityEngine.Color.white;
            lightSource.Spawn();
       //     NetworkServer.Spawn(lightSource);
        }
        public void Reset()
        {
            traitorDetected = false;
            SetLightColor(UnityEngine.Color.white);
        }

        private void getScp914Room()
        {
            RoomIdentifier ident = null;

            var result = Room.Get(RoomName.Lcz914).ToList();
            if (result.Count == 0)
            {
                return;
            }
            scp914Room = result.First();
        }
        private void broadcastToPlayersInRoom(string msg)
        {
            foreach (Player player in Player.GetAll())
            {
                if (player == null)
                    continue;
                if (!player.IsAlive)
                    continue;
                if (player.Room == null)
                    continue;   
                if (player.Room.Name != RoomName.Lcz914)
                    continue;
                round.playerManager.notificationManager.PlayerNotify(player, msg);
            }
        }
        private void SetLightColor(UnityEngine.Color color)
        {
            if (lightSource != null)
            {
                lightSource.Color = color;
                return;
            }
            if (scp914Room == null)
                return;

        }

        private int getPlayersInChamber()
        {
            int numOfPlayers = 0;


            Vector3 leftChamberPos = Scp914Controller.Singleton.IntakeChamber.position;
            //Log.Debug(leftChamberPos.ToString());
            foreach(Player player in Player.GetAll())
            {
                if (Vector3.Distance(player.Position, leftChamberPos) < 1.2f)
                {
                    numOfPlayers++;
                }
            }
            return numOfPlayers;

        }
        public bool shouldActivate(Round.Round round, Player player)
        {
            if (round.teamManager.GetTeam(player) != Team.Team.Detective && !round.config.traitorTesterConfig.AllowNonDetective)
            {
                player.SendBroadcast(round.config.traitorTesterConfig.CantUseBroadcast, 5);
                return false;
            }
            if (getPlayersInChamber() < round.config.traitorTesterConfig.MinimumPlayers)
            {
                player.SendBroadcast(round.config.traitorTesterConfig.NeedXPlayers.Replace("{x}", round.config.traitorTesterConfig.MinimumPlayers.ToString()), 5);
                return false;
            }
            Reset();
            return true;
        }
        public void ProcessPlayer(Round.Round round, Player player)
        {
            if (traitorDetected)
            {
                return;
            }

            if (round.teamManager.GetTeam(player) == Team.Team.Traitor)
            {
                traitorDetected = true;
                SetLightColor(UnityEngine.Color.red);
                broadcastToPlayersInRoom("<color=red>Traitor Detected!</color>");
                return;
            }
            SetLightColor(UnityEngine.Color.green);
        }
    }
}
