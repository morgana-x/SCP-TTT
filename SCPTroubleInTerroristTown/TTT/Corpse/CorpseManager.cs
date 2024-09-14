using System;
using System.Linq;
using PluginAPI.Core;
using PluginAPI.Enums;
using PlayerStatsSystem;
using PlayerRoles.Ragdolls;
using System.Collections.Generic;
using UnityEngine;

namespace SCPTroubleInTerroristTown.TTT.Corpse
{

    public class Corpse
    {
        public BasicRagdoll Ragdoll;
        public string victimName;
        public Team.Team victimTeam;
        public DamageHandlerBase newDamageHandler;

        public bool Discovered = false;
        public Corpse(BasicRagdoll ragdoll, string victimName, Team.Team victimTeam, string undiscoveredNick, string undiscoveredDeathText, DamageHandlerBase newDamageHandler)
        {
            Ragdoll = ragdoll;

            this.victimTeam = victimTeam;
            this.victimName = victimName;
            this.newDamageHandler = newDamageHandler;

            var damageHandlerTemp = new CustomReasonDamageHandler(undiscoveredDeathText);
            Ragdoll.NetworkInfo = new RagdollData(Ragdoll.NetworkInfo.OwnerHub, damageHandlerTemp, Ragdoll.NetworkInfo.RoleType, Ragdoll.NetworkInfo.StartPosition, Ragdoll.NetworkInfo.StartRotation, undiscoveredNick, Ragdoll.NetworkInfo.CreationTime);
        }

        public void Discover(Player discoverer, Round.Round round)
        {
            if (Discovered)
            {
                return;
            }
            Discovered = true;

            Ragdoll.NetworkInfo = new RagdollData(Ragdoll.NetworkInfo.OwnerHub, newDamageHandler, Ragdoll.NetworkInfo.RoleType, Ragdoll.NetworkInfo.StartPosition, Ragdoll.NetworkInfo.StartRotation, victimName, Ragdoll.NetworkInfo.CreationTime);

            string discovererName = discoverer != null ? discoverer.Nickname : "Unknown";
            Team.Team discovererTeam = discoverer != null ? round.teamManager.GetVisibleTeam(discoverer) : Team.Team.Undecided;

            string message = round.config.corpseConfig.DiscoverMessage.Replace("{player}", $"<color={round.config.teamsConfig.TeamColor[discovererTeam]}>{discovererName}</color>").Replace("{victim}", victimName).Replace("{team}", $"<color={round.config.teamsConfig.TeamColor[victimTeam]}>{round.config.teamsConfig.TeamName[victimTeam]}</color>");
            round.playerManager.notificationManager.NotifyAll(message);
        }
    }

    public class CorpseManager
    {
        Round.Round round;
        public CorpseManager(Round.Round round)
        {
            this.round = round;
        }

        public List<Corpse> corpseList = new List<Corpse>();

        public void Cleanup()
        {
            corpseList.Clear();
        }
        private static string getTeamInfo(TTTConfig config, Team.Team playerTeam)
        {
            string prefix = config.teamsConfig.TeamName[playerTeam].ToLower().StartsWith("i") ? "an" : "a";
            string corpseInfo = $"They were {prefix} <color={config.teamsConfig.TeamColor[playerTeam]}>{config.teamsConfig.TeamName[playerTeam]}</color>!";
            return corpseInfo;
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


            DamageType damageType = Util.Util.getDamageTypeFromHandler(handler);
            DamageType accurateDamageType = Util.Util.getDamageType(deathReason);
            CorpseConfig.deathMessage msg = getDeathReasonFromType(config, damageType);
            string desc = msg.Description;
            desc = desc.Replace("{ammo}", Util.Util.getAmmoType(Util.Util.getItemType(accurateDamageType)).ToString().Replace("Ammo", ""));
            DeathInfoText = DeathInfoText.Replace("{title}", msg.Title);
            DeathInfoText = DeathInfoText.Replace("{description}", desc);

            if (Util.Util.isHeadshot(handler.ServerLogsText))
            {
                DeathInfoText += "\n\n" + config.corpseConfig.headShotTranslation.Title + "\n" + config.corpseConfig.headShotTranslation.Description;
            }
            return DeathInfoText;
        }
        public string GetCorpseInfo( PluginAPI.Core.Player player, Team.Team playerTeam, DamageHandlerBase handler)
        {
            string deathReason = handler.ServerLogsText;
            string DeathInfoText = GetDeathInfo(round.config, handler, deathReason);


            string teamInfo = getTeamInfo(round.config, playerTeam);

            string MainText = $"\n\n{teamInfo}\n\n{DeathInfoText}";
            return MainText;
        }
        public void OnCorpseSpawn(ReferenceHub hub, BasicRagdoll ragdoll)
        {
            Team.Team victimTeam = round.teamManager.GetPreviousTeam(Player.Get(hub));
            string victimName = Player.Get(hub).Nickname;
            Corpse corpse = new Corpse(ragdoll, victimName, victimTeam, round.config.corpseConfig.UndiscoveredNick, round.config.corpseConfig.UndiscoveredText, new CustomReasonDamageHandler(GetCorpseInfo(Player.Get(hub), round.teamManager.GetTeam(Player.Get(hub)), ragdoll.NetworkInfo.Handler)));
            corpseList.Add(corpse);
        }
        public void OnCorpseDiscoverHotKey(Player player)
        {
            Corpse ragdoll = getPlayerLookedatCorpse(player);
            if (ragdoll == null) return;
            ragdoll.Discover(player, round);
            corpseList.Remove(ragdoll);
        }
        private Corpse getPlayerLookedatCorpse(PluginAPI.Core.Player pl)
        {
            Ray ray = new Ray(pl.Camera.position + (pl.Camera.forward * 0.16f), pl.Camera.forward);
            Physics.Raycast(ray, out RaycastHit hit, 3f);
            float minDist = 3f;

            foreach (var r in corpseList)
            {
                try
                {
                    if (Vector3.Distance(hit.transform.position, r.Ragdoll.CenterPoint.position) < minDist)
                    {
                        return r;
                    }
                    if (Vector3.Distance(hit.transform.position, r.Ragdoll.transform.position) < minDist)
                    {
                        return r;
                    }
                }
                catch
                {

                }
            }
            return null;
/*Ray ray = new Ray(pl.Camera.position + (pl.Camera.forward * 0.16f), pl.Camera.forward);
if (!Physics.Raycast(ray, out RaycastHit hit, 10))
{
    return null;
}
var found = GetRagdoll(hit.collider.transform.root.gameObject);
return found;*/
        }
        private Corpse GetRagdoll(GameObject obj)
        {
            foreach(var r in corpseList)
            {
                if (r.Ragdoll.transform.gameObject == obj)
                    return r;
                if (r.Ragdoll.gameObject == obj)
                    return r;
            }
            return null;
        }
    }
}
