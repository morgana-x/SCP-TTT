using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPTroubleInTerroristTown.TTT.Credit
{
    
    public class CreditStoreItem
    {
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public ItemType AmmoType { get; set; }
        public int Price { get; set; }

        public bool AllowTraitor { get; set; } = true;
        public bool AllowDetective { get; set; } = true;
    }

    public class CreditConfig
    {
        public int KillAwardCredit { get; set; } = 10;

        public int TeamTraitorStartCredits { get; set; } = 200;

        public int TeamDetectiveStartCredits { get; set; } = 250;

        public Dictionary<string,CreditStoreItem> CreditStore { get; set; } = new Dictionary<string, CreditStoreItem>()
        {
           
            ["ak47"] = new CreditStoreItem() {
                Name = "Ak-47",
                ItemType = ItemType.GunAK,
                AmmoType = ItemType.Ammo762x39,
                Price = 200
            },
            ["shotgun"] = new CreditStoreItem() {
                Name = "Shotgun",
                ItemType = ItemType.GunShotgun,
                AmmoType = ItemType.Ammo12gauge,
                Price = 200
            },
        };
        
    }
}
