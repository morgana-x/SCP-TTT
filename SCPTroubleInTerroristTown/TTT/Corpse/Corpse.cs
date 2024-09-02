using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Enums;
using PlayerStatsSystem;
using System.Runtime.InteropServices;

namespace SCPTroubleInTerroristTown.TTT.Corpse
{
    public class Corpse
    {
        private static bool isHeadshot(string serverLogsText)
        {
            return serverLogsText.Contains("Headshot");
        }
        private static DamageType getDamageType(string serverLogsText)
        {
            var dmgType = DamageType.PlayerLeft;
            foreach (DamageType type in Enum.GetValues(typeof(DamageType)).Cast<DamageType>())
            {
                if (serverLogsText.ToLower().Contains(type.ToString().ToLower()))
                {
                    dmgType = type;
                    break;
                }

            }
            return dmgType;
        }
        private static ItemType getAmmoType(ItemType type)
        {
            switch (type)
            {
                case ItemType.GunCOM15:
                case ItemType.GunCOM18:
                case ItemType.GunCom45:
                case ItemType.GunCrossvec:
                case ItemType.GunFSP9:
                    return ItemType.Ammo9x19;
                case ItemType.GunE11SR:
                case ItemType.GunFRMG0:
                    return ItemType.Ammo556x45;
                case ItemType.GunLogicer:
                case ItemType.GunAK:
                case ItemType.GunA7:
                    return ItemType.Ammo762x39;
                case ItemType.GunRevolver:
                    return ItemType.Ammo44cal;
                case ItemType.GunShotgun:
                    return ItemType.Ammo12gauge;
                default:
                    return ItemType.None;
            }
        }
        private static ItemType getItemType(DamageType dmg)
        {
            var dmgType = ItemType.None;
          
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)).Cast<ItemType>())
            {
                if ("Gun" + dmg.ToString() == type.ToString())
                {
                    dmgType = type;
                    break;
                }

            }
            return dmgType;
        }

        private static ItemType getFirearmType(DamageType dmg)
        {
            var dmgType = ItemType.None;
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)).Cast<ItemType>())
            {
                if (dmg.ToString().ToLower() == type.ToString().ToLower())
                {
                    dmgType = type;
                    break;
                }

            }
            return dmgType;
        }
        
        private static string getTeamInfo(TTTConfig config, Team.Team playerTeam)
        {
            string prefix = config.teamsConfig.TeamName[playerTeam].ToLower().StartsWith("i") ? "an" : "a";
            string corpseInfo = $"They were {prefix} <color={config.teamsConfig.TeamColor[playerTeam]}>{config.teamsConfig.TeamName[playerTeam]}</color>!";
            return corpseInfo;
        }

        private static DamageType getDamageTypeFromHandler(DamageHandlerBase dmgbase, string logs)
        {
            if (dmgbase is FirearmDamageHandler)
            {
                return DamageType.Firearm;
            }
            if (dmgbase is ExplosionDamageHandler)
            {
                return DamageType.Explosion;
            }
            if (dmgbase is JailbirdDamageHandler)
            {
                return DamageType.Jailbird;
            }
            logs = logs.ToLower();
            Log.Debug(logs);
            if (logs.Contains("poison"))
            {
                return DamageType.Poisoned;
            }
            if (logs.Contains("pocket"))
            {
                return DamageType.PocketDecay;
            }
            if (logs.Contains("fall") || logs.Contains("fell"))
            {
                return DamageType.Falldown;
            }
            if (logs.Contains("hypothermia"))
            {
                return DamageType.Hypothermia;
            }
            if (logs.Contains("207"))
            {
                return DamageType.Scp207;
            }
            if (logs.Contains("173"))
            {
                return DamageType.Scp173;
            }
            if (logs.Contains("shot"))
            {
                return DamageType.Firearm;
            }
            if (logs.Contains("jailbird"))
            {
                return DamageType.Jailbird;
            }
            if (logs.Contains("asphyxiated") || logs.Contains("strangle"))
            {
                return DamageType.Asphyxiated;
            }
            return DamageType.Universal;

        }
        private static CorpseConfig.deathMessage getDeathReasonFromType(TTTConfig config, DamageType type)
        {
            foreach (var pair in config.corpseConfig.deathTranslations)
            {
                if (pair.Key == type)
                {
                    return pair.Value;
                }
            }

            return config.corpseConfig.unknownDeathTranslation;
        }
        private static string GetDeathInfo(TTTConfig config, DamageHandlerBase handler, string deathReason)
        {
            string DeathInfoText = "{title}\n{description}";


            DamageType damageType = getDamageTypeFromHandler(handler, deathReason);
            DamageType accurateDamageType = getDamageType(deathReason);
            CorpseConfig.deathMessage msg = getDeathReasonFromType(config, damageType);
            string desc = msg.Description;
            desc = desc.Replace("{ammo}", getAmmoType(getItemType(accurateDamageType)).ToString().Replace("Ammo", ""));
            DeathInfoText = DeathInfoText.Replace("{title}", msg.Title);
            DeathInfoText = DeathInfoText.Replace("{description}", desc);

            if (isHeadshot(handler.ServerLogsText))
            {
                DeathInfoText += "\n\n" + config.corpseConfig.headShotTranslation.Title + "\n" + config.corpseConfig.headShotTranslation.Description;
            }
            return DeathInfoText;
        }
        public static string GetCorpseInfo(TTTConfig config, PluginAPI.Core.Player player, Team.Team playerTeam, DamageHandlerBase handler, string deathReason)
        {

            string DeathInfoText = GetDeathInfo(config, handler, deathReason);


            string teamInfo = getTeamInfo(config, playerTeam);

            string MainText = $"\n\n{teamInfo}\n\n{DeathInfoText}";
            return MainText;
        }
    }
}
