﻿using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPTroubleInTerroristTown.TTT.Team
{
    public class LoadoutManager
    {
        public Round.Round round;

        public LoadoutManager(Round.Round round)
        {
            this.round = round;
        }
        private List<ItemType> GetLoadout(Team team)
        {
            if (!round.config.teamsConfig.TeamLoadout.ContainsKey(team))
            {
                return new List<ItemType> { };
            }
            return round.config.teamsConfig.TeamLoadout[team];
        }
        private void GiveLoadoutItems(PluginAPI.Core.Player pl, List<ItemType> items)
        {
            foreach (var i in items)
            {
                if (pl == null)
                {
                    break;
                }
                if (i == null)
                {
                    continue;
                }
                if (i.ToString().StartsWith("Ammo"))
                {
                    pl.AddAmmo(i, 100);
                    continue;
                }
                pl.AddItem(i);
            }
        }
        public void GiveLoadout(PluginAPI.Core.Player pl, bool clearInventory = false)
        {
            if (pl == null)
            {
                return;
            }
            if (!PluginAPI.Core.Player.GetPlayers().Contains(pl))
            {
                return;
            }
            if (clearInventory)
            {
                pl.ClearInventory(true);
            }
            Team team = round.teamManager.GetTeam(pl);
            if (team == Team.Spectator)
            {
                return;
            }
            GiveLoadoutItems(pl, GetLoadout(team));
        }
    }
}
