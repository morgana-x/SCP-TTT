using System.Collections.Generic;
using PluginAPI.Core;

namespace SCPTroubleInTerroristTown.TTT.Karma
{
    public class KarmaManager
    {
        Round round;
        public KarmaManager(Round tttround) 
        { 
            round = tttround;
        }
        public Dictionary<string, int> Karma = new Dictionary<string, int>();
        public Dictionary<string, int> OldKarma = new Dictionary<string, int>();

        public void SetupKarma(PluginAPI.Core.Player pl)
        {
            if (Karma.ContainsKey(pl.UserId))
            {
                return;
            }
            SetKarma(pl, round.config.karmaConfig.karma_starting_karma);
            UpdateOldKarma(pl);
        }
        public void SetKarma(PluginAPI.Core.Player pl, int karma)
        {
            if (karma > round.config.karmaConfig.karma_max)
            {
                karma = round.config.karmaConfig.karma_max;
            }
            if (!Karma.ContainsKey(pl.UserId)) { Karma.Add(pl.UserId, karma); return; }
            Karma[pl.UserId] = karma;
        }
        public int GetKarma(PluginAPI.Core.Player pl)
        {
            if (!Karma.ContainsKey(pl.UserId))
            {
                return round.config.karmaConfig.karma_starting_karma;
            }
            return Karma[pl.UserId];
        }
        public int GetOldKarma(PluginAPI.Core.Player pl)
        {
            if (!OldKarma.ContainsKey(pl.UserId))
            {
                OldKarma.Add(pl.UserId, GetKarma(pl));
            }
            return OldKarma[pl.UserId];
        }
        public void UpdateOldKarma(PluginAPI.Core.Player pl)
        {
            if (!OldKarma.ContainsKey(pl.UserId))
            {
                OldKarma.Add(pl.UserId, GetKarma(pl));
                return;
            }
            OldKarma[pl.UserId] = GetKarma(pl);
        }
        public void AddKarma(PluginAPI.Core.Player pl, int amount)
        {
            SetKarma(pl, GetKarma(pl) + amount);
        }

        public string[] KarmaToString(int karma)
        {
            float maxkarma = round.config.karmaConfig.karma_max;

            if (karma > maxkarma * 0.89f)
                return round.config.karmaConfig.KarmaStatus[KarmaState.karma_max];

            if (karma > maxkarma * 0.8f)
                return round.config.karmaConfig.KarmaStatus[KarmaState.karma_high];

            if (karma > maxkarma * 0.65f)
                return round.config.karmaConfig.KarmaStatus[KarmaState.karma_med];

            if (karma > maxkarma * 0.5f)
                return round.config.karmaConfig.KarmaStatus[KarmaState.karma_low];

            return round.config.karmaConfig.KarmaStatus[KarmaState.karma_min];
        }

        public void ReplenishKarma()
        {
            foreach(PluginAPI.Core.Player player in PluginAPI.Core.Player.GetPlayers()) {
                AddKarma(player, round.config.karmaConfig.karma_round_increment);
            };
        }
        public bool AllowedSpawnKarmaCheck(PluginAPI.Core.Player pl)
        {
            if (!round.config.karmaConfig.karma_low_round_suspension)
            {
                return true;
            }
            if (GetKarma(pl) < round.config.karmaConfig.karma_low_round_suspension_amount )
            {
                pl.SendBroadcast(round.config.karmaConfig.karma_low_round_suspension_message.Replace("{karma}", GetKarma(pl).ToString()).Replace("{minkarma}", round.config.karmaConfig.karma_low_round_suspension_amount.ToString()), 20);
                return false;
            }
            return true;
        }
        public bool KarmaKick(PluginAPI.Core.Player player)
        {
            if (!round.config.karmaConfig.karma_low_round_kick)
            {
                return false;
            }
            if (GetKarma(player) > round.config.karmaConfig.karma_low_round_kick_amount)
            {
                return false;
            }
            SetKarma(player, round.config.karmaConfig.karma_low_round_kick_amount + 1);
            player.Kick(round.config.karmaConfig.karma_low_kick_message.Replace("{karma}", GetKarma(player).ToString()).Replace("{minkarma}", round.config.karmaConfig.karma_low_round_kick_amount.ToString()));
            Log.Info($"Kicked {player.Nickname}({player.UserId}) for having too low karma when team killing!");
            return true;
        }
        public void KarmaPunishCheck(PluginAPI.Core.Player victim, PluginAPI.Core.Player attacker)
        {
            if (round.currentRoundState != Round.RoundState.Running)
            {
                return;
            }

            Log.Info($"{attacker.Nickname}({attacker.UserId}) team killed {victim.Nickname}({victim.UserId})!");

            if (KarmaKick(attacker))
            {
                return;
            }

            Team.Team victimTeam = round.teamManager.previousTeams.ContainsKey(victim) ? round.teamManager.previousTeams[victim] : round.teamManager.GetTeam(victim);
            Team.Team attackerTeam = round.teamManager.GetTeam(attacker);

            if (victimTeam == attackerTeam || (victimTeam == Team.Team.Innocent && attackerTeam == Team.Team.Detective) || (attackerTeam == Team.Team.Innocent && victimTeam == Team.Team.Detective))
            {
                AddKarma(attacker, round.config.karmaConfig.karma_kill_penalty); // Lose karma for killing same team
                return;
            }
            if (victimTeam == Team.Team.Traitor)
            {
                AddKarma(attacker, round.config.karmaConfig.karma_traitorkill_bonus); // Gain less karma for killing correct team as traitor as that's easy
            }
            AddKarma(attacker, round.config.karmaConfig.karma_clean_bonus); // Gain karma for killing correct team
        }
    }
}
