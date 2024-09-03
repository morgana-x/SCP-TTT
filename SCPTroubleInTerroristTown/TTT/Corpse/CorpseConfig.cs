using CommandSystem.Commands.Shared;
using PlayerStatsSystem;
using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPTroubleInTerroristTown.TTT.Corpse
{
    public class CorpseConfig
    {
        public class deathMessage
        {
            public string Title { get; set; } = "Title Not Set";
            public string Description { get; set; } = "Description Not Set";
            public deathMessage (string title, string description)
            {
                Title = title;
                Description = description;
            }
            public deathMessage()
            {

            }
        }
        public string UndiscoveredNick { get; set; } = "Unidentified Corpse";
        public string UndiscoveredText { get; set; } = "Press <color=yellow>Alt</color> to investigate the body.";

        public string DiscoverMessage { get; set; } = "{player} discovered {victim}'s corpse!";
        public Dictionary<DamageType, deathMessage> deathTranslations { get; set; } = new Dictionary<DamageType, deathMessage>()
        {
            [DamageType.Firearm] = new deathMessage()
            {
                Title = "<color=red>Ouch!</color>",
                Description = "They were shot with <color=yellow>{ammo}</color> rounds."
            },

            [DamageType.Tesla] = new deathMessage()
            { 
                Title = "<color=red>Zap!</color>",
                Description = "Severe burns indicate electricution from an <color=red>extremely high powered</color> source."
            },
            [DamageType.CardiacArrest] = new deathMessage() { 
                Title = "<color=red>Flatline</color>", 
                Description = "They appear to have died from a heart attack." 
            },
            [DamageType.Explosion] = new deathMessage() { 
                Title = "<color=red>Crispy Demise</color>",
                Description = "They met a fiery end in an <color=orange>explosion</color>"
            },
            [DamageType.Poisoned] = new deathMessage() { 
                Title = "<color=green>Don't feel too good...</color>", 
                Description = "They had consumed some form <color=green>poison</color> before succumbing to it's effects." 
            },
            [DamageType.Falldown] = new deathMessage() { 
                Title = "<color=red>My legs!</color>", 
                Description = "Shattered legs indicate a death by a <color=yellow>fall</color>" 
            },
            [DamageType.Jailbird] = new deathMessage() { 
                Title = "<color=red>Oomph</color>", 
                Description = "Visible trauma suggets they were hit with a <color=yellow>blunt object</color>" 
            },
            [DamageType.Bleeding] = new deathMessage() { 
                Title = "<color=red>Inverse Vampire</color>",
                Description = "Severe discolorment and visible wounds suggest a death by <color=red>Blood Loss</color>." 
            },
            [DamageType.Checkpoint] = new deathMessage()
            {
                Title = "<color=red>Door Stuck!</color>",
                Description = "They appear to had been jammed between <color=yellow>sliding panels</color>, resulting in their death."
            },
            [DamageType.PocketDecay] = new deathMessage()
            {
                Title = "<color=blue>Rotten Luck</color>",
                Description = "Somehow this poor chap when into the pocket dimension, against the creators wishes. Deserved."
            },
            [DamageType.Hypothermia] = new deathMessage()
            {
                Title = "<color=blue>Fr..freezing</color>",
                Description = "Evident from them generally being an ice block, they died of <color=blue>Hypothermia</color>."
            },
            [DamageType.PlayerLeft] = new deathMessage()
            {
                Title = "<color=red>???</color>",
                Description = "The player has somehow <color=yellow>left this world</color>."
            },
            [DamageType.Hemorrhage] = new deathMessage()
            {
                Title = "<color=red>Oh dear</color>",
                Description = "They have suffered a <color=yellow>Hemorrhage</color>."
            },
            [DamageType.Asphyxiated] = new deathMessage()
            {
                Title = "<color=blue>Oxygen not included</color>",
                Description = "They died, <color=yellow>starved of oxygen</color>."
            },
            [DamageType.ForcedDeath] = new deathMessage()
            {
                Title = "<color=yellow>God's Will</color>",
                Description = "The heavens decided that this individual deserved to die."
            },
            [DamageType.MolecularDisruptor] = new deathMessage()
            {
                Title = "<color=blue>Human Particle Accelerator</color>",
                Description = "...Science!"
            },
            [DamageType.Scp207] = new deathMessage()
            {
                Title = "<color=red>Not a toy!</color>",
                Description = "A small round mark is impressioned upond the fatal wound, perhaps a <color=red>ball</color>?"
            },
            [DamageType.Scp173] = new deathMessage()
            {
                Title = "<color=red>Oh Snap</color>",
                Description = "Their neck has been broken."
            }
        };
        public deathMessage headShotTranslation { get; set; } = new deathMessage("<color=red>No time to scream!</color>", "They were <color=yellow>shot in the head</color>.");
        public deathMessage unknownDeathTranslation { get; set; } = new deathMessage("<color=red>???</color>", "Unknown cause of death.");
    }
}
