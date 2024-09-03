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
        public ReferenceHub referenceHub;
        public DamageHandlerBase newDamageHandler;

        public bool Discovered = false;
        public Corpse(BasicRagdoll ragdoll, ReferenceHub referenceHub, string undiscoveredNick, string undiscoveredDeathText, DamageHandlerBase newDamageHandler)
        {
            Ragdoll = ragdoll;
            
            this.referenceHub = referenceHub;
            this.newDamageHandler = newDamageHandler;

            var damageHandlerTemp = new CustomReasonDamageHandler(undiscoveredDeathText);
            Ragdoll.NetworkInfo = new RagdollData(Ragdoll.NetworkInfo.OwnerHub, damageHandlerTemp, Ragdoll.NetworkInfo.RoleType, Ragdoll.NetworkInfo.StartPosition, Ragdoll.NetworkInfo.StartRotation, undiscoveredNick, Ragdoll.NetworkInfo.CreationTime);
        }

        public void Discover(Player discoverer, Round round)
        {
            if (Discovered)
            {
                return;
            }
            Discovered = true;
            Ragdoll.NetworkInfo = new RagdollData(Ragdoll.NetworkInfo.OwnerHub, newDamageHandler, Ragdoll.NetworkInfo.RoleType, Ragdoll.NetworkInfo.StartPosition, Ragdoll.NetworkInfo.StartRotation, Ragdoll.NetworkInfo.Nickname, Ragdoll.NetworkInfo.CreationTime);
            Cassie.Message(round.config.corpseConfig.DiscoverMessage.Replace("{player}", discoverer.Nickname).Replace("{victim}", Player.Get(referenceHub).Nickname), isNoisy:false, isSubtitles:true);
        }
    }

    public class CorpseManager
    {
        Round round;
        public CorpseManager(Round round)
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
            Corpse corpse = new Corpse(ragdoll, hub, round.config.corpseConfig.UndiscoveredNick, round.config.corpseConfig.UndiscoveredText, new CustomReasonDamageHandler(GetCorpseInfo(Player.Get(hub), round.teamManager.GetTeam(Player.Get(hub)), ragdoll.NetworkInfo.Handler)));
            corpseList.Add(corpse);
        }
        public void OnCorpseDiscoverHotKey(Player player)
        {
            Log.Debug($"{player.Nickname} is trying to discover corpse!");
            Corpse ragdoll = getPlayerLookedatCorpse(player);
            if (ragdoll == null) return;
            ragdoll.Discover(player, round);
        }
        private Corpse getPlayerLookedatCorpse(PluginAPI.Core.Player pl)
        {

            foreach (var r in corpseList)
            {
                if (Vector3.Distance(r.Ragdoll.CenterPoint.position, pl.Position) < 4)
                {
                    return r;
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
