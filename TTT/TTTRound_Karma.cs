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
        public Dictionary<Player, int> Karma = new Dictionary<Player, int>();
        public Dictionary<Player, int> OldKarma = new Dictionary<Player, int>();
        public void SetKarma(Player pl, int karma)
        {
            if (!Karma.ContainsKey(pl)) { Karma.Add(pl, karma); return; }
            Karma[pl] = karma;
        }
        public int GetKarma(Player pl)
        {
            if (!Karma.ContainsKey(pl))
            {
                SetKarma(pl, 0);
                return 0;
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
        private void UpdateOldKarma(Player pl)
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
    }
}
