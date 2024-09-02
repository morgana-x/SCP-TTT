using PluginAPI.Core;
using System.Collections.Generic;
using System.Linq;

namespace SCPTroubleInTerroristTown.TTT.Award
{

    class AwardEntry
    {
        public string name;
        public int num;
        public AwardEntry(string name, int num)
        {
            this.name = name;
            this.num = num;
        }
        public AwardEntry(string name)
        {
            this.name = name;
            this.num = 0;
        }
    }

    public class AwardManager
    {
        private Round round;
        private Dictionary<AwardType, AwardEntry> awards = new Dictionary<AwardType, AwardEntry>();
        private int suicideCount = 0;
        private Dictionary<string, int> teamKills= new Dictionary<string, int>();
        private int teamKillCount = 0;
        private int killCount = 0;

        public List<string> finalAwards = new List<string>();
        public AwardManager(Round round)
        {
            this.round = round;
        }
        public void Cleanup()
        {
            suicideCount = 0;
            teamKillCount = 0;
            killCount = 0;
            awards.Clear();
            teamKills.Clear();
            finalAwards.Clear();
        }

        public void AddAward(AwardType type, string name, int count = 0)
        {
            if (!awards.ContainsKey(type))
            {
                awards.Add(type, new AwardEntry(name, count));
                return;
            }
            awards[type] = new AwardEntry(name, count);
        }
        public void RemoveAward(AwardType type) 
        {
            if (!awards.ContainsKey(type))
            {
                return;
            }
            awards.Remove(type);
        }
        public void OnPlayerKill(string vicName, Team.Team vicTeam, string attackName, Team.Team attackTeam)
        {
            bool teamKill = (vicTeam == attackTeam) || (vicTeam == Team.Team.Detective && attackTeam == Team.Team.Innocent) || (attackTeam == Team.Team.Detective && vicTeam == Team.Team.Innocent);
            bool suicide = vicName == attackName;
            killCount++;
            bool firstBlood = killCount == 1;
            if (suicide)
            {
                suicideCount++;
                if (suicideCount == 1)
                {
                    AddAward(AwardType.Suicide_Lonely, vicName);
                    return;
                }
                RemoveAward(AwardType.Suicide_Lonely);
                AddAward(AwardType.Suicide_Cult, vicName, suicideCount-1);
                return;
            }
            if (teamKill)
            {
                if (!teamKills.ContainsKey(attackName))
                {
                    teamKills.Add(attackName, 0);
                }
                teamKills[attackName]++;
            }
            if (firstBlood)
            {
                if (teamKill && attackTeam == Team.Team.Innocent)
                {
                    AddAward(AwardType.FirstBlood_Blooper, attackName);
                    return;
                }
                if (teamKill && attackTeam == Team.Team.Traitor)
                {
                    AddAward(AwardType.FirstBlood_Stupid, attackName);
                    return;
                }
                if (attackTeam == Team.Team.Traitor)
                {
                    AddAward(AwardType.FirstBlood, attackName);
                    return;
                }
                AddAward(AwardType.FirstBlood_Blow, attackName);
                return;
            }
            if (teamKill)
            {
                if (teamKillCount > teamKills[attackName])
                    return;
                switch (teamKills[attackName])
                {
                    case 1:
                        AddAward(AwardType.Teamkill_1, attackName);
                        break;
                    case 2:
                        RemoveAward(AwardType.Teamkill_1);
                        AddAward(AwardType.Teamkill_2, attackName);
                        break;
                    case 3:
                        RemoveAward(AwardType.Teamkill_1);
                        RemoveAward(AwardType.Teamkill_2);
                        AddAward(AwardType.Teamkill_3, attackName);
                        break;
                    default:
                        RemoveAward(AwardType.Teamkill_1);
                        RemoveAward(AwardType.Teamkill_2);
                        RemoveAward(AwardType.Teamkill_3);
                        AddAward(AwardType.Teamkill_moron, attackName); // Todo: add the rest of teamkill stuff
                        break;
                }
                return;
            }

            if (attackTeam == Team.Team.Traitor) 
            {

            }
        }

        public void getFinalisedAwards()
        {
            finalAwards.Clear();
            int count = 0;
            foreach (var award in awards) 
            {
                count++;
                if (count > 2)
                    break;
                AwardTranslation translation = round.config.awardConfig.AwardTranslation[award.Key];
                string awardstr = $"<color=yellow>{translation.Title}</color>\n{award.Value.name} {translation.Description}".Replace("{num}", $"<color=blue>{award.Value.num.ToString()}</color>");
                Log.Debug("Added " + awardstr + " to award final!");
                finalAwards.Add(awardstr);
            }
        }
    }

}
