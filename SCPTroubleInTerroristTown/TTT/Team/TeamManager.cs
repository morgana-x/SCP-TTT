using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPTroubleInTerroristTown.TTT.Team
{
    public enum Team
    {
        Innocent,
        Traitor,
        Detective,
        Spectator,
        Undecided
    }
    public class TeamManager
    {
        public TeamManager(Round.Round round)
        {
            tttRound = round;
            loadoutManager = new LoadoutManager(round);
        }
        private Round.Round tttRound;

        public Dictionary<PluginAPI.Core.Player, Team> playerTeams = new Dictionary<PluginAPI.Core.Player, Team>();
        public Dictionary<PluginAPI.Core.Player, Team> previousTeams = new Dictionary<PluginAPI.Core.Player, Team>();

        public LoadoutManager loadoutManager;
        public void SetTeam(PluginAPI.Core.Player pl, Team team)
        {
            SetTeam(pl, team, false);
        }
        public Team GetTeam(PluginAPI.Core.Player pl)
        {
            if (!playerTeams.ContainsKey(pl))
            {
                SetTeam(pl, Team.Spectator);
            }
            return playerTeams[pl];
        }
        public Team GetPreviousTeam(PluginAPI.Core.Player pl)
        {
            if (!previousTeams.ContainsKey(pl))
            {
                previousTeams.Add(pl, Team.Spectator);
            }
            return previousTeams[pl];
        }
        public Team GetVisibleTeam(PluginAPI.Core.Player pl)
        {
            if (!playerTeams.ContainsKey(pl))
            {
                SetTeam(pl, Team.Spectator);
            }
            if (playerTeams[pl] == Team.Traitor)
            {
                return Team.Innocent;
            }
            return playerTeams[pl];
        }
        public void SetTeam(PluginAPI.Core.Player pl, Team team, bool SetPreviousTeam = true)
        {
            if (!playerTeams.ContainsKey(pl))
            {
                if (!previousTeams.ContainsKey(pl))
                {
                    previousTeams.Add(pl, Team.Undecided);
                }
                playerTeams.Add(pl, Team.Undecided);
                SetTeam(pl, team, SetPreviousTeam);
                return;
            }
            if (SetPreviousTeam)
            {
                if (!previousTeams.ContainsKey(pl))
                {
                    previousTeams.Add(pl, team);
                }
                else
                {
                    previousTeams[pl] = team;
                }
            }
            playerTeams[pl] = team;
            UpdatePlayerTag(pl);
        }
        public List<PluginAPI.Core.Player> GetTeamPlayers(Team team)
        {
            return playerTeams.Keys.Where((x) => (GetTeam(x) == team)).ToList();
        }

        private void UpdatePlayerTag(Player pl)
        {
            pl.PlayerInfo.IsRoleHidden = true;
            pl.PlayerInfo.IsUnitNameHidden = true;
            pl.PlayerInfo.IsPowerStatusHidden = true;
            tttRound.playerManager.badgeManager.SyncPlayer(pl);
        }
        private System.Random randomGenerator = new System.Random();
        public void AssignRoles() // Semi-Port of the original GMOD function
        {
            Log.Debug("Assigning roles!");
            List<Player> filterPlayers = Player.GetPlayers().Where((x) => x.Role != RoleTypeId.Spectator).ToList();
            List<PluginAPI.Core.Player> remainingPlayers = new List<PluginAPI.Core.Player>();
            foreach (PluginAPI.Core.Player pl in filterPlayers)
            {
                if (tttRound.karmaManager.AllowedSpawnKarmaCheck(pl))
                {
                    remainingPlayers.Add(pl);
                }
            }

            if (remainingPlayers.Count == 0)
            {
                return;
            }
            foreach (PluginAPI.Core.Player pl in remainingPlayers)
            {
                SetTeam(pl, Team.Innocent, true);
            }

            // Get target numbers of Traitors/Detectives
            int targetNumOfTraitors = GetTraitorCount(remainingPlayers.Count);
            int targetNumofDetectives = GetDetectiveCount(remainingPlayers.Count);

            int numTerrorists = 0;
            int numDetectives = 0;



            // Assign Traitors
            while (numTerrorists < targetNumOfTraitors)
            {
                PluginAPI.Core.Player pick = remainingPlayers.RandomItem(); // No need to shuffle when you can do this!
                if ((!(previousTeams.ContainsKey(pick) && previousTeams[pick] == Team.Traitor) || (randomGenerator.Next(3) == 2)))
                {
                    remainingPlayers.Remove(pick);
                    SetTeam(pick, Team.Traitor, true);
                    numTerrorists++;
                }
            }

            // Assign Detectives REMOVE NPC CHECKS LATER
            while (numDetectives < targetNumofDetectives)
            {
                if (remainingPlayers.Count <= targetNumofDetectives - numDetectives)
                {
                    foreach (PluginAPI.Core.Player pl in remainingPlayers)
                    {
                        SetTeam(pl, Team.Detective, true);
                    }
                    break;
                }
                PluginAPI.Core.Player pick = remainingPlayers.RandomItem();
                if (tttRound.karmaManager.GetKarma(pick) > tttRound.config.teamsConfig.DetectiveMinKarma) // Karma and player choices later
                {
                    SetTeam(pick, Team.Detective, true);
                    numDetectives++;
                }
                remainingPlayers.Remove(pick);
            }


            foreach (PluginAPI.Core.Player pl in PluginAPI.Core.Player.GetPlayers())
            {
                tttRound.playerManager.setSpawnTime(pl); // Make sure their spawn text is shown!
            }
            tttRound.playerManager.badgeManager.Resync();
            Log.Debug("Finished Assigned roles");
        }

        public void Cleanup()
        {
            previousTeams.Clear();
            playerTeams.Clear();
        }
        private int GetTraitorCount(int numberOfPlayers) // From gmod function
        {
            int traitorCount = (int)Math.Floor(numberOfPlayers * tttRound.config.teamsConfig.TraitorPercentage);
            traitorCount = Math.Min(traitorCount, tttRound.config.teamsConfig.TraitorMax);
            if (traitorCount < 1)
            {
                return 1;
            }
            return traitorCount;
        }
        private int GetDetectiveCount(int numberOfPlayers) // From gmod function
        {
            if (numberOfPlayers < tttRound.config.teamsConfig.DetectiveMin)
            {
                return 0;
            }
            int detectiveCount = (int)Math.Floor(numberOfPlayers * tttRound.config.teamsConfig.DetectivePercentage);
            detectiveCount = Math.Min(detectiveCount, tttRound.config.teamsConfig.DetectiveMax);
            return detectiveCount;
        }
    }
}
