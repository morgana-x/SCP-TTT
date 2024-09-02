using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SCPTroubleInTerroristTown.TTT.Award
{
    public enum AwardType
    {
        Suicide_Cult, //  "Suicide Cult Leader" "showed the other suiciders how to do it by being the first to go."
        Suicide_Lonely, //"Lonely and Depressed" "was the only one who killed themselves.

        Explode_ExplosiveGrant, // "Explosives Research Grant" "was recognized for their research on explosions. {num} test subjects helped out."
        Explode_FieldResearch, // "Field Research" "tested their own resistance to explosions. It was not high enough."

        FirstBlood, // "First Blood" "delivered the first innocent death at a traitor's hands."
        FirstBlood_Stupid, // "First Bloody Stupid Kill" "scored the first kill by shooting a fellow traitor. Good job."
        FirstBlood_Blooper, // "First Blooper" "was the first to kill. Too bad it was an innocent comrade."
        FirstBlood_Blow, //"First Blow"  "struck the first blow for the innocent terrorists by making the first death a traitor's."

        AllKill_Innocent, //  "Deadliest Among Equals" "was responsible for every kill made by the innocent this round."
        AllKill_Traitor, // "Lone Wolf" "was responsible for every kill made by a traitor this round."

        TraitorKill_Kill1Innocent, // "I Got One, Boss!" "managed to kill a single innocent. Sweet!"
        TraitorKill_Kill2Innocent, // "A Bullet For Two" "showed the first one was not a lucky shot by killing another."
        TraitorKill_Kill3Innocent, // "Serial Traitor" "ended three innocent lives of terrorism today."
        TraitorKill_KillManyInnocent, // "Wolf Among More Sheep-Like Wolves" "eats innocent terrorists for dinner. A dinner of {num} courses."
        TraitorKill_KillAlotInnocent, // "Counter-Terrorism Operative" "gets paid per kill. Can now buy another luxury yacht."

        InnocentKill_Kill1Traitor, // "Betray This"  "found a traitor. Shot a traitor. Easy."
        InnocentKill_Kill2Traitor,  // "Applied to the Justice Squad",  "escorted two traitors to the great beyond."
        InnocentKill_Kill3Traitor, // "Do Traitors Dream Of Traitorous Sheep?"  "put three traitors to rest."
        InnocentKill_KillManyTraitor, // "Internal Affairs Employee" "gets paid per kill. Can now order their fifth swimming pool."

        Fall_Pushed, // "No Mr. Bond, I Expect You To Fall",  "pushed someone off a great height."
        Fall_Floored, // "Floored"  "let their body hit the floor after falling from a significant altitude."
        Fall_Metorite, // ""The Human Meteorite", ""crushed someone by falling on them from a great height."
        
        Headshot_1, //  "Efficiency" "discovered the joy of headshots and made {num}."
        Headshot_2, // "Neurology"  "removed the brains from {num} heads for a closer examination."
        Headshot_3, // "Videogames Made Me Do It" "applied their murder simulation training and headshotted {num} foes."

        Teamkill_1, //  "Made An Oopsie" "had their finger slip just when they were aiming at a buddy."
        Teamkill_2, // "Double-Oops"  "thought they got a Traitor twice, but was wrong both times."
        Teamkill_3, // Karma-conscious" "couldn't stop after killing two teammates. Three is their lucky number."
        Teamkill_all, // "Teamkiller" "murdered the entirety of their team. OMGBANBANBAN."
        Teamkill_most, // "Roleplayer" "was roleplaying a madman, honest. That's why they killed most of their team."
        Teamkill_moron,// "Moron" "couldn't figure out which side they were on, and killed over half of their comrades."
        Teamkill_redneck, // "Redneck" "protected their turf real good by killing over a quarter of their teammates."

        CorpseFind_Coroner, // "Coroner"  "found {num} bodies lying around."
        CorpseFind_Poke, // "Gotta Catch Em All" "found {num} corpses for their collection."
        CorpseFind_DeathScent, // "Death Scent" "keeps stumbling on random corpses, {num} times this round."

        CreditFind_1, // "Recycler" "scrounged up {num} leftover credits from corpses."
        
        Death_Late, // "Pyrrhic Victory" "died only seconds before their team won the round."
        Death_Early, // "I Hate This Game" "died right after the start of the round."

    }

    public class AwardTranslation
    {
        public string Title { get; set; } = "Null";
        public string Description { get; set; } = "was the last one to...";
    }


    public class AwardConfig
    {
        public string AwardTitle { get; set; } = "<color=red>Round Awards</color>";
        public Dictionary<AwardType, AwardTranslation> AwardTranslation { get; set; } = new Dictionary<AwardType, AwardTranslation>()
        {
            [AwardType.Suicide_Cult] = new AwardTranslation() { Title = "Suicide Cult Leader", Description = "showed the other suiciders how to do it by being the first to go."},
            [AwardType.Suicide_Lonely] = new AwardTranslation() { Title = "Lonely and Depressed", Description = "was the only one who killed themselves."},

            [AwardType.Explode_ExplosiveGrant] = new AwardTranslation() { Title = "Explosives Research Grant", Description = "was recognized for their research on explosions. {num} test subjects helped out." },
            [AwardType.Explode_FieldResearch] = new AwardTranslation() { Title = "Field Research", Description = "tested their own resistance to explosions. It was not high enough." },

            [AwardType.FirstBlood] = new AwardTranslation() { Title = "First Blood", Description = "delivered the first innocent death at a traitor's hands." },
            [AwardType.FirstBlood_Stupid] = new AwardTranslation() { Title = "First Bloody Stupid Kill", Description = "scored the first kill by shooting a fellow traitor. Good job."},
            [AwardType.FirstBlood_Blooper] = new AwardTranslation() { Title = "First Blooper", Description = "was the first to kill. Too bad it was an innocent comrade." },
            [AwardType.FirstBlood_Blow] = new AwardTranslation() { Title = "First Blow", Description = "struck the first blow for the innocent terrorists by making the first death a traitor's."},

            [AwardType.AllKill_Innocent] = new AwardTranslation() { Title = "Deadliest Among Equals", Description = "was responsible for every kill made by the innocent this round."},
            [AwardType.AllKill_Traitor] = new AwardTranslation() { Title = "Lone Wolf", Description= "was responsible for every kill made by a traitor this round." },

            [AwardType.TraitorKill_Kill1Innocent] = new AwardTranslation() { Title = "I Got One, Boss!", Description = "managed to kill a single innocent. Sweet!" },
            [AwardType.TraitorKill_Kill2Innocent] = new AwardTranslation() { Title = "A Bullet For Two", Description = "showed the first one was not a lucky shot by killing another."},
            [AwardType.TraitorKill_Kill3Innocent] = new AwardTranslation() { Title = "Serial Traitor", Description = "ended three innocent lives of terrorism today." },
            [AwardType.TraitorKill_KillManyInnocent] = new AwardTranslation() { Title = "Wolf Among More Sheep-Like Wolves", Description = "eats innocent terrorists for dinner. A dinner of {num} courses." },
            [AwardType.TraitorKill_KillAlotInnocent] = new AwardTranslation() { Title = "Counter-Terrorism Operative", Description = "gets paid per kill. Can now buy another luxury yacht." },

            [AwardType.InnocentKill_Kill1Traitor] = new AwardTranslation() { Title = "Betray This", Description = "found a traitor. Shot a traitor. Easy."},
            [AwardType.InnocentKill_Kill2Traitor] = new AwardTranslation() { Title = "Applied to the Justice Squad", Description = "escorted two traitors to the great beyond."},
            [AwardType.InnocentKill_Kill3Traitor] = new AwardTranslation() { Title = "Do Traitors Dream Of Traitorous Sheep?", Description = "put three traitors to rest." },
            [AwardType.InnocentKill_KillManyTraitor] = new AwardTranslation() { Title = "Internal Affairs Employee" , Description = "gets paid per kill. Can now order their fifth swimming pool." },

            [AwardType.Fall_Pushed] = new AwardTranslation() { Title = "No Mr. Bond, I Expect You To Fall" , Description = "pushed someone off a great height."},
            [AwardType.Fall_Floored] = new AwardTranslation() { Title = "Floored", Description = "let their body hit the floor after falling from a significant altitude." },
            [AwardType.Fall_Metorite] = new AwardTranslation() { Title = "The Human Meteorite", Description = "crushed someone by falling on them from a great height." },

            [AwardType.Headshot_1] = new AwardTranslation() { Title = "Efficiency", Description = "discovered the joy of headshots and made {num}." },
            [AwardType.Headshot_2] = new AwardTranslation() { Title = "Neurology", Description = "removed the brains from {num} heads for a closer examination." },
            [AwardType.Headshot_3] = new AwardTranslation() { Title = "Videogames Made Me Do It", Description = "applied their murder simulation training and headshotted {num} foes." },

            [AwardType.Teamkill_1] = new AwardTranslation() { Title = "Made an Oopsie", Description = "had their finger slip just when they were aiming at a buddy." },
            [AwardType.Teamkill_2] = new AwardTranslation() { Title = "Double-Oops", Description = "thought they got a Traitor twice, but was wrong both times." },
            [AwardType.Teamkill_3] = new AwardTranslation() { Title = "Karma-conscious", Description = "couldn't stop after killing two teammates. Three is their lucky number." },
            [AwardType.Teamkill_all] = new AwardTranslation() { Title = "Teamkiller", Description = "murdered the entirety of their team. OMGBANBANBAN." },
            [AwardType.Teamkill_most] = new AwardTranslation() { Title = "Roleplayer", Description = "was roleplaying a madman, honest. That's why they killed most of their team." },
            [AwardType.Teamkill_moron] = new AwardTranslation() { Title = "Moron", Description = "couldn't figure out which side they were on, and killed over half of their comrades." },
            [AwardType.Teamkill_redneck] = new AwardTranslation() { Title = "Redneck", Description = "protected their turf real good by killing over a quarter of their teammates." },

            [AwardType.CorpseFind_Coroner] = new AwardTranslation() { Title = "Coroner", Description = "found {num} bodies lying around." },
            [AwardType.CorpseFind_Poke] = new AwardTranslation() { Title = "Gotta Catch Em All", Description = "found {num} corpses for their collection." },
            [AwardType.CorpseFind_DeathScent] = new AwardTranslation() { Title = "Death Scent", Description = "keeps stumbling on random corpses, {num} times this round." },

            [AwardType.CreditFind_1] = new AwardTranslation() { Title = "Recycler", Description = "scrounged up {num} leftover credits from corpses." },

            [AwardType.Death_Early] = new AwardTranslation() { Title = "Pyrrhic Victory", Description = "died only seconds before their team won the round." },
            [AwardType.Death_Late] = new AwardTranslation() { Title = "I Hate This Game", Description = "died right after the start of the round." }

        };

    }
}
