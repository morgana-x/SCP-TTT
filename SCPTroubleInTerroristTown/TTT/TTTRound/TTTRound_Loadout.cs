﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Items.Firearms.Ammo;
using PluginAPI.Core;

namespace SCPTroubleInTerroristTown.TTT
{
    public partial class TTTRound
    {
        private List<ItemType> GetLoadout(Team team)
        {
            if (!config.teamsConfig.TeamLoadout.ContainsKey(team))
            {
                return new List<ItemType> { };
            }
            return config.teamsConfig.TeamLoadout[team];
        }
        private void GiveLoadoutItems(Player pl, List<ItemType> items)
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
        public void GiveLoadout(Player pl, bool clearInventory = false)
        {
            if (pl == null)
            {
                return;
            }
            if (!Player.GetPlayers().Contains(pl))
            {
                return;
            }
            if (clearInventory)
            {
                pl.ClearInventory(true);
            }
            Team team = GetTeam(pl);
            if (team == Team.Spectator)
            {
                return;
            }
            GiveLoadoutItems(pl, GetLoadout(team));
        }


    }
}
