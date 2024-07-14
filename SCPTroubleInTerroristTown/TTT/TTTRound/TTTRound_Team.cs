using PlayerRoles;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Core;

namespace SCPTroubleInTerroristTown.TTT
{
    public partial class TTTRound
    {
        public Dictionary<Player, Team> playerTeams = new Dictionary<Player, Team>();
        public Dictionary<Player, Team> previousTeams = new Dictionary<Player, Team>();
        public enum Team
        {
            Innocent,
            Traitor,
            Detective,
            Spectator,
            Undecided
        }
      
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
                    previousTeams.Add(pl, team);
                }
                playerTeams.Add(pl, team);
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
        }
        public List<Player> GetTeamPlayers(Team team)
        {
            return playerTeams.Keys.Where((x) => (GetTeam(x) == team)).ToList();
        }
        private void AssignRoles() // Semi-Port of the original GMOD function
        {
            Log.Debug("Assigning roles!");
            List<Player> remainingPlayers = Player.GetPlayers().Where((x) => x.Role != RoleTypeId.Spectator).ToList();

            foreach (Player pl in remainingPlayers)
            {
                SetTeam(pl, Team.Innocent, true);
            }

            if (remainingPlayers.Count == 0)
            {
                return;
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
                if (GetKarma(pick) > config.teamsConfig.DetectiveMinKarma) // Karma and player choices later
                {
                    SetTeam(pick, Team.Detective, true);
                    numDetectives++;
                }
                remainingPlayers.Remove(pick);
            }


            foreach (Player pl in Player.GetPlayers())
            {
                setSpawnTime(pl); // Make sure their spawn text is shown!
            }

            Log.Debug("Finished Assigned roles");
        }

        private int GetTraitorCount(int numberOfPlayers) // From gmod function
        {
            int traitorCount = (int)Math.Floor(numberOfPlayers * config.teamsConfig.TraitorPercentage);
            traitorCount = Math.Min(traitorCount, config.teamsConfig.TraitorMax);
            return traitorCount;
        }
        private int GetDetectiveCount(int numberOfPlayers) // From gmod function
        {
            if (numberOfPlayers < config.teamsConfig.DetectiveMin)
            {
                return 0;
            }
            int detectiveCount = (int)Math.Floor(numberOfPlayers * config.teamsConfig.DetectivePercentage);
            detectiveCount = Math.Min(detectiveCount, config.teamsConfig.DetectiveMax);
            return detectiveCount;
        }
    }
}
