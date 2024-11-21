using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using PlayerRoles;
using PluginAPI.Commands;
using PluginAPI.Core;
namespace SCPTroubleInTerroristTown.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Investigate : ParentCommand, ICommand
    {
        public override string Command { get; } = "ttt_search";
        public override string Description { get; } = "Alternative Investigate Corpse Hotkey";

        public override string[] Aliases { get; } = new string[] { "ttt_investigate" };
        public override void LoadGeneratedCommands() { }
        public static Dictionary<string, DateTime> Cooldowns = new Dictionary<string, DateTime>();

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {

           
            Player caller = Player.Get(sender);
            if (Cooldowns.TryGetValue(caller.UserId, out DateTime value) && DateTime.Now < value)
            {
                response = "Cooldown active!";
                return false;
            }
            if (!Cooldowns.TryGetValue(caller.UserId, out _))
            {
                Cooldowns.Add(caller.UserId, DateTime.Now.AddMilliseconds(250));
            }
            else
            {
                Cooldowns[caller.UserId] = DateTime.Now.AddMilliseconds(250);
            }
            MainClass.Singleton.tttRound.corpseManager.OnCorpseDiscoverHotKey(caller);
            response = "";
            return true;
        }
    }
}
