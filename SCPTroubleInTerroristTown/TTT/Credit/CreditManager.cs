using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPTroubleInTerroristTown.TTT.Credit
{
    public class CreditManager
    {
        Round round;
        public Dictionary<PluginAPI.Core.Player, int> PlayerCredits = new Dictionary<PluginAPI.Core.Player, int>();

        public CreditManager(Round round)
        {
            this.round = round;
        }
        public void Cleanup()
        {
            PlayerCredits.Clear();
        }

        public void SetCredits(PluginAPI.Core.Player player, int creditamt)
        {
            if (player == null)
                return;
            if (!PlayerCredits.ContainsKey(player))
                PlayerCredits.Add(player, creditamt);
            PlayerCredits[player] = creditamt;
        }
        public int GetCredits(PluginAPI.Core.Player player)
        {
            if (!PlayerCredits.ContainsKey(player))
            {
                SetCredits(player, 0);
            }
            return PlayerCredits[player];
        }
        public void AddCredits(PluginAPI.Core.Player player, int amount)
        {
            if (player == null)
                return;
            SetCredits(player, GetCredits(player) + amount);
        }
        public string BuyCreditStoreItem(PluginAPI.Core.Player player, string id)
        {
            if (!round.config.creditConfig.CreditStore.ContainsKey(id))
            {
                return $"Item \"{id}\" doesn't exist!";
            }
            CreditStoreItem item = round.config.creditConfig.CreditStore[id];
            if (item.Price > GetCredits(player))
            {
                return $"Cannot afford Item {item.Name}!";
            }

            player.AddItem(item.ItemType);
            player.AddAmmo(item.AmmoType, 100);
            AddCredits(player, -item.Price);

            return $"Bought {item.Name} for {item.Price} credits!";
        }

        public string GetCreditStoreItemsForPlayer(PluginAPI.Core.Player player)
        {
            return "";
        }
    }
}
