using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
namespace SCP_SL_Trouble_In_Terrorist_Town
{
    public partial class TTTRound
    {
        DateTime nextHudShow = DateTime.Now;

        private void hudUpdate() // Todo: Optimise this
        {
            if (DateTime.Now < nextHudShow)
            {
                return;
            }
            nextHudShow = DateTime.Now.AddSeconds(1.1);

            foreach (Player pl in Player.List)
            {
                if (pl.IsNPC || (!playerTeams.ContainsKey(pl)))
                {
                    continue;
                }
                //float ping = (pl.Ping) + 0.1f;
                //Math.Min(ping, 1.9f); // 1.8 is the max I get on satelitte on AU servers, so I sure hope noone gets worse than this!
                hud.ShowHud(pl, DateTime.Now.Subtract(getSpawnTime(pl)).TotalSeconds < config.hudConfig.ShowCustomSpawnMessageDuration, 1.3f);
            }
        }
    }
}
