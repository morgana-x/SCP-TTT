﻿using AdminToys;
using MapGeneration;
using Mirror;
using PluginAPI.Core;
using PluginAPI.Core.Zones;
using Scp914;
using UnityEngine;

namespace SCPTroubleInTerroristTown.TTT.TraitorTester
{
    public class TraitorTester
    {
        public bool traitorDetected = false;
       
        public FacilityRoom scp914Room = null;
        public LightSourceToy lightSource = null;
        /// <summary>
        /// A primitive base
        /// </summary>
        public LightSourceToy Base;

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
        private void SpawnLight()
        {
            lightSource = UnityEngine.Object.Instantiate(ToyPrefab);
            lightSource.NetworkLightIntensity = 20f;
            lightSource.NetworkLightRange = 15f;
            lightSource.NetworkLightShadows = true;
            setLightPos();
            lightSource.NetworkLightColor = UnityEngine.Color.white;
            NetworkServer.Spawn(lightSource.gameObject);
        }
        public void Reset()
        {
            traitorDetected = false;
            SetLightColor(UnityEngine.Color.white);
        }
        private void setLightPos()
        {
            Vector3 newPos = scp914Room.Position + (Vector3.up * 1f);
            lightSource.Position = newPos;
            lightSource.NetworkPosition = newPos;
            lightSource.transform.position = newPos;
        }
        private void getScp914Room()
        {
            RoomIdentifier ident = null;

            bool success = RoomIdUtils.TryFindRoom(RoomName.Lcz914, MapGeneration.FacilityZone.LightContainment, RoomShape.Endroom, out ident);
            if (!success)
            {
                return;
            }
            scp914Room = ident.ApiRoom;
        }
        private void SetLightColor(UnityEngine.Color color)
        {
            lightSource.NetworkLightColor = color;
        }

        private int getPlayersInChamber()
        {
            int numOfPlayers = 0;


            Vector3 leftChamberPos = Scp914Controller.Singleton.IntakeChamber.position;
            //Log.Debug(leftChamberPos.ToString());
            foreach(PluginAPI.Core.Player player in PluginAPI.Core.Player.GetPlayers())
            {
                if (Vector3.Distance(player.Position, leftChamberPos) < 1.2f)
                {
                    numOfPlayers++;
                }
            }
            return numOfPlayers;

        }
        public bool shouldActivate(Round round, PluginAPI.Core.Player player)
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
        public void ProcessPlayer(Round round, PluginAPI.Core.Player player)
        {
            if (traitorDetected)
            {
                return;
            }

            if (round.teamManager.GetTeam(player) == Team.Team.Traitor)
            {
                traitorDetected = true;
                SetLightColor(UnityEngine.Color.red);
                return;
            }
            SetLightColor(UnityEngine.Color.green);
        }
    }
}
