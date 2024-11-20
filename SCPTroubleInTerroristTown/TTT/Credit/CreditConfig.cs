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
        public ItemType ItemType { get; set; } = ItemType.None;
        public ItemType AmmoType { get; set; } = ItemType.None;
        public int Price { get; set; }

        public bool AllowTraitor { get; set; } = true;
        public bool AllowDetective { get; set; } = true;

        public bool AllowInnocent { get; set; } = false;
    }

    public class CreditConfig
    {
        public int KillAwardCredit { get; set; } = 2;

        public int TeamTraitorStartCredits { get; set; } = 4;

        public int TeamDetectiveStartCredits { get; set; } = 4;

        public Dictionary<string,CreditStoreItem> CreditStore { get; set; } = new Dictionary<string, CreditStoreItem>()
        {

            ["com15"] = new CreditStoreItem()
            {
                Name = "COM-15",
                ItemType = ItemType.GunCOM15,
                AmmoType = ItemType.Ammo9x19,
                Price = 4
            },
            ["revolver"] = new CreditStoreItem()
            {
                Name = "Revolver",
                ItemType = ItemType.GunRevolver,
                AmmoType = ItemType.Ammo44cal,
                Price = 4
            },
            ["crossvec"] = new CreditStoreItem()
            {
                Name = "Cross-Vec",
                ItemType = ItemType.GunCrossvec,
                AmmoType = ItemType.Ammo9x19,
                Price = 6
            },
            ["ak47"] = new CreditStoreItem() {
                Name = "Ak-47",
                ItemType = ItemType.GunAK,
                AmmoType = ItemType.Ammo762x39,
                Price = 5
            },
            ["shotgun"] = new CreditStoreItem() {
                Name = "Shotgun",
                ItemType = ItemType.GunShotgun,
                AmmoType = ItemType.Ammo12gauge,
                Price = 10
            },
            ["armorcombat"] = new CreditStoreItem()
            {
                Name = "Combat Armor",
                ItemType = ItemType.ArmorCombat,
                Price = 4,
                AllowDetective = false
            },
            ["armorheavy"] = new CreditStoreItem()
            {
                Name = "Heavy Armor",
                ItemType = ItemType.ArmorHeavy,
                Price = 6,
                AllowDetective = false
            },
            ["jailbird"] = new CreditStoreItem()
            {
                Name = "Jail-Bird",
                ItemType = ItemType.Jailbird,
                Price = 20,
                AllowDetective = false
            }
        };
        
    }
}
