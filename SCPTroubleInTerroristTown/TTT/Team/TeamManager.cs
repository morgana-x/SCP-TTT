using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public TeamManager(Round round)
        {
            tttRound = round;
        }
        private Round tttRound;
        public Dictionary<Player, Team> playerTeams = new Dictionary<Player, Team>();
        public Dictionary<Player, Team> previousTeams = new Dictionary<Player, Team>();

        public void SetTeam(Player pl, Team team)
        {
            SetTeam(pl, team, false);
        }
        public Team GetTeam(Player pl)
        {
            if (!playerTeams.ContainsKey(pl))
            {
                SetTeam(pl, Team.Spectator);
            }
            return playerTeams[pl];
        }
        public void SetTeam(Player pl, Team team, bool SetPreviousTeam = true)
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
            UpdatePlayerTag(pl, team);
        }
        public List<Player> GetTeamPlayers(Team team)
        {
            return playerTeams.Keys.Where((x) => (GetTeam(x) == team)).ToList();
        }

        private void UpdatePlayerTag(Player pl, Team team)
        {
            if (pl.ReferenceHub.serverRoles.HasGlobalBadge)
            {
                // I love Northwoods badge system (I'm glad its getting an overhaul!!!)
                return;
            }
            if (team == Team.Traitor)
            {
                team = Team.Innocent;
            }
            try
            {
                pl.ReferenceHub.serverRoles.SetText(tttRound.config.teamsConfig.TeamName[team]);
                pl.ReferenceHub.serverRoles.SetColor(tttRound.config.teamsConfig.TeamColorSimplified[team]);
            }
            catch(Exception e)
            {
                Log.Error("Error occured while trying to set tag text for " + pl.DisplayNickname + "\n" + e.ToString());
            }
        }
        private System.Random randomGenerator = new System.Random();
        public void AssignRoles() // Semi-Port of the original GMOD function
        {
            Log.Debug("Assigning roles!");
            List<Player> remainingPlayers = Player.GetPlayers().Where((x) => x.Role != RoleTypeId.Spectator).ToList();

            foreach (Player pl in remainingPlayers)
            {
                if (!tttRound.karmaManager.AllowedSpawnKarmaCheck(pl))
                {
                    SetTeam(pl, Team.Spectator, true);
                }
            }
            remainingPlayers = Player.GetPlayers().Where((x) => x.Role != RoleTypeId.Spectator).ToList();

            if (remainingPlayers.Count == 0)
            {
                return;
            }
            foreach (Player pl in remainingPlayers)
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
                Player pick = remainingPlayers.RandomItem(); // No need to shuffle when you can do this!
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
                    foreach (Player pl in remainingPlayers)
                    {
                        SetTeam(pl, Team.Detective, true);
                    }
                    break;
                }
                Player pick = remainingPlayers.RandomItem();
                if (tttRound.karmaManager.GetKarma(pick) > tttRound.config.teamsConfig.DetectiveMinKarma) // Karma and player choices later
                {
                    SetTeam(pick, Team.Detective, true);
                    numDetectives++;
                }
                remainingPlayers.Remove(pick);
            }


            foreach (Player pl in Player.GetPlayers())
            {
                tttRound.playerManager.setSpawnTime(pl); // Make sure their spawn text is shown!
            }

            Log.Debug("Finished Assigned roles");
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
