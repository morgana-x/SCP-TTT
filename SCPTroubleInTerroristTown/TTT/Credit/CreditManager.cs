using System.Collections.Generic;
using PluginAPI.Core;
namespace SCPTroubleInTerroristTown.TTT.Credit
{
    public class CreditManager
    {
        Round.Round round;
        public Dictionary<Player, int> PlayerCredits = new Dictionary<Player, int>();

        public CreditManager(Round.Round round)
        {
            this.round = round;
        }
        public void Cleanup()
        {
            PlayerCredits.Clear();
        }

        public void SetCredits(Player player, int creditamt)
        {
            if (player == null)
                return;
            if (!PlayerCredits.ContainsKey(player))
                PlayerCredits.Add(player, creditamt);
            PlayerCredits[player] = creditamt;
        }
        public int GetCredits(Player player)
        {
            if (!PlayerCredits.ContainsKey(player))
            {
                SetCredits(player, 0);
            }
            return PlayerCredits[player];
        }
        public void AddCredits(Player player, int amount)
        {
            if (player == null)
                return;
            SetCredits(player, GetCredits(player) + amount);
        }
        public string BuyCreditStoreItem(Player player, string id)
        {
            if (!round.config.creditConfig.CreditStore.ContainsKey(id))
            {
                return $"Item \"{id}\" doesn't exist!";
            }
            CreditStoreItem item = round.config.creditConfig.CreditStore[id];

            if (!CanPurchaseItem(player, item))
            {
                return $"$Not allowed to purchase item {item.Name}.";
            }
            if (item.Price > GetCredits(player))
            {
                return $"Cannot afford Item {item.Name}! ${item.Price} > ${GetCredits(player)}!";
            }

            if (item.ItemType != ItemType.None)
                player.AddItem(item.ItemType);
            if (item.AmmoType != ItemType.None)
                player.AddAmmo(item.AmmoType, 100);
            AddCredits(player, -item.Price);

            return $"Bought {item.Name} for {item.Price} credits!";
        }


        private bool CanPurchaseItem(Player player, CreditStoreItem item)
        {
            if (!player.IsAlive)
                return false;
            Team.Team team = round.teamManager.GetTeam(player);
            if (item.AllowDetective && team == Team.Team.Detective)
                return true;
            if (item.AllowTraitor && team == Team.Team.Traitor)
                return true;
            if (item.AllowInnocent && team == Team.Team.Innocent)
                return true;
            return false;
        }
        private bool CanCollectCredits(Player player)
        {
            if (!player.IsAlive)
                return false;
            Team.Team team = round.teamManager.GetTeam(player);
            if (team == Team.Team.Detective)
                return true;
            if (team == Team.Team.Traitor)
                return true;
            return false;
        }
        public void OnCorpseDiscovery(Player player)
        {
            if (round == null)
                return;
            if (!CanCollectCredits(player))
                return;
            int credits = round.config.creditConfig.KillAwardCredit;
            AddCredits(player, credits);
            round.playerManager.notificationManager.PlayerNotify(player, $"You found <color=red>{credits}</color> credits!");
        }
        public void GiveStartingCredits(Player player)
        {
            if (round == null)
                return;
            Team.Team team = round.teamManager.GetTeam(player);
            if (team == Team.Team.Detective)
                AddCredits(player, round.config.creditConfig.TeamDetectiveStartCredits);
            if (team == Team.Team.Traitor)
                AddCredits(player, round.config.creditConfig.TeamTraitorStartCredits);
        }
        public string GetCreditStoreItemsForPlayer(Player player)
        {
            string list = $"\n==============================\nCurrent Credits: ${GetCredits(player)}\n==============================\n";
            foreach (var a in round.config.creditConfig.CreditStore)
            {
                if (!CanPurchaseItem(player, a.Value))
                    continue;
                list += $"[{a.Key}] {a.Value.Name} | ${a.Value.Price}\n";
            }
            list += "==============================";
            return list;
        }
    }
}
