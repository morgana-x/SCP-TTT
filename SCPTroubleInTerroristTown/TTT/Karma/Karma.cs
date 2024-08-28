﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decals;
using PluginAPI.Core;
using SCPTroubleInTerroristTown.TTT.Karma;
using SCPTroubleInTerroristTown.TTT.Team;

namespace SCPTroubleInTerroristTown.TTT
{
    public class KarmaManager
    {
        Round round;
        public KarmaManager(Round tttround) 
        { 
            round = tttround;
        }
        public Dictionary<Player, int> Karma = new Dictionary<Player, int>();
        public Dictionary<Player, int> OldKarma = new Dictionary<Player, int>();

        public void SetupKarma(Player pl)
        {
            if (Karma.ContainsKey(pl))
            {
                return;
            }
            SetKarma(pl, round.config.karmaConfig.karma_starting_karma);
        }
        public void SetKarma(Player pl, int karma)
        {
            if (karma > round.config.karmaConfig.karma_max)
            {
                karma = round.config.karmaConfig.karma_max;
            }
            if (!Karma.ContainsKey(pl)) { Karma.Add(pl, karma); return; }
            Karma[pl] = karma;
        }
        public int GetKarma(Player pl)
        {
            if (!Karma.ContainsKey(pl))
            {
                SetKarma(pl, round.config.karmaConfig.karma_starting_karma);
                return round.config.karmaConfig.karma_starting_karma;
            }
            return Karma[pl];
        }
        public int GetOldKarma(Player pl)
        {
            if (!OldKarma.ContainsKey(pl))
            {
                OldKarma.Add(pl, GetKarma(pl));
            }
            return OldKarma[pl];
        }
        public void UpdateOldKarma(Player pl)
        {
            if (!OldKarma.ContainsKey(pl))
            {
                OldKarma.Add(pl, GetKarma(pl));
                return;
            }
            OldKarma[pl] = GetKarma(pl);
        }
        public void AddKarma(Player pl, int amount)
        {
            if (!Karma.ContainsKey(pl))
            {
                SetKarma(pl, amount);
                return;
            }
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
            foreach(Player player in Player.GetPlayers()) {
                AddKarma(player, round.config.karmaConfig.karma_round_increment);
            };
        }
        public bool AllowedSpawnKarmaCheck(Player pl)
        {
            if (!round.config.karmaConfig.karma_low_round_suspension)
            {
                return true;
            }
            if (GetKarma(pl) < round.config.karmaConfig.karma_low_round_suspension_amount )
            {
                // ({karma}<{minkarma})
                pl.SendBroadcast(round.config.karmaConfig.karma_low_round_suspension_message.Replace("{karma}", GetKarma(pl).ToString()).Replace("{minkarma}", round.config.karmaConfig.karma_low_round_suspension_amount.ToString()), 10);
                return false;
            }
            return true;
        }
        public bool KarmaKick(Player player)
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
            player.Kick(round.config.karmaConfig.karma_low_kick_message);
            return true;
        }
        public void KarmaPunishCheck(Player victim, Player attacker)
        {
            if (round.currentRoundState != Round.RoundState.Running)
            {
                return;
            }
            if (KarmaKick(attacker))
            {
                return;
            }
            Team.Team victimTeam = round.teamManager.GetTeam(victim);
            Team.Team attackerTeam = round.teamManager.GetTeam(attacker);
            if (victimTeam == attackerTeam || (victimTeam == Team.Team.Innocent && attackerTeam == Team.Team.Detective) || (victimTeam == Team.Team.Detective && attackerTeam == Team.Team.Innocent))
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
