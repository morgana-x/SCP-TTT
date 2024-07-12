﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using UnityEngine;

namespace SCP_SL_Trouble_In_Terrorist_Town.TTT
{
    internal class TTTCorpse
    {
        
       // private static Dictionary<DamageType, string> deathReasonTranslations = new Dictionary<DamageType, string>();

        private static string headShotTranslation = "\n\n<color=red>No time to scream!</color>\nThey were <color=yellow>shot in the head</color>.";
        private static string fellTranslation = "\n\n<color=red>My legs!</color>\nThey <color=red>fell to their death.</color>";
        private static string explodedTranslation = "\n\n<color=red>Crispy Demise</color>\nThey met a fiery end in an <color=orange>explosion</color>";
        private static bool isHeadshot(string serverLogsText)
        {
            return serverLogsText.Contains("Headshot");
        }

        private static DamageType getDamageType(string serverLogsText) 
        {
            var dmgType = DamageType.Unknown;
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
        private static ItemType getItemType(DamageType dmg)
        {
            var dmgType = ItemType.None;
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)).Cast<ItemType>())
            {
                if ("Gun" + dmg.ToString() ==  (type.ToString()))
                {
                    dmgType = type;
                    break;
                }

            }
            return dmgType;
        }
        private static FirearmType getFirearmType(DamageType dmg)
        {
            var dmgType = FirearmType.None;
            foreach (FirearmType type in Enum.GetValues(typeof(FirearmType)).Cast<FirearmType>())
            {
                if (dmg.ToString().ToLower() == (type.ToString().ToLower()))
                {
                    dmgType = type;
                    break;
                }

            }
            return dmgType;
        }
        public static string GetCorpseInfo(TTTConfig config, Player player, TTTRound.Team playerTeam, PlayerStatsSystem.DamageHandlerBase handler, string deathReason)
        {
            string baseText = "Unknown cause of death.";
            /*if (deathReason2 != null && deathReason2.handler != null)
            {
                baseText = (deathReason2.handler.Type.IsWeapon() ? "Killed with " : "") + deathReason2.handler.Type.ToString();
            }*/
            DamageType type = getDamageType(handler.ServerLogsText);

            if (type != DamageType.Unknown)
            {
                baseText = type.ToString();
            }
            if (handler.ServerLogsText.Contains("Falldown"))
            {
                baseText = fellTranslation;
            }
            if (handler.ServerLogsText.Contains("Explosion"))
            {
                baseText = explodedTranslation;
            }
            if (handler.ServerLogsText.Contains("Shot"))
            {
                baseText = "<color=red>Ouch!</color>\nThey were shot with a <color=yellow>" + baseText + "</color>.";
            }
            string prefix = config.teamsConfig.TeamName[playerTeam].ToLower().StartsWith("i") ? "an" : "a";
            string corpseInfo =  $"\n\nThey were {prefix} <color={config.teamsConfig.TeamColor[playerTeam]}>{config.teamsConfig.TeamName[playerTeam]}</color>!";
            corpseInfo = corpseInfo + "\n\n" + baseText;

            if (isHeadshot(handler.ServerLogsText))
            {
                corpseInfo += headShotTranslation;
            }
            return corpseInfo;
        }
    }
}
