using System;
using System.Collections.Generic;
using CommandSystem;
using PluginAPI.Core;
namespace SCPTroubleInTerroristTown.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Help : ParentCommand, ICommand
    {
        public override string Command { get; } = "ttt_help";
        public override string Description { get; } = "Lists commands for TTT";

        public override string[] Aliases { get; } = new string[] { "help_ttt" };
        public override void LoadGeneratedCommands() { }
        public static Dictionary<string, DateTime> Cooldowns = new Dictionary<string, DateTime>();
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player caller = Player.Get(sender);
            if (Cooldowns.TryGetValue(caller.UserId, out DateTime value) && DateTime.Now < value)
            {
                response = "Wait a second before using this command again!";
                return false;
            }
            if (!Cooldowns.TryGetValue(caller.UserId, out _))
            {
                Cooldowns.Add(caller.UserId, DateTime.Now.AddSeconds(1));
            }
            else
            {
                Cooldowns[caller.UserId] = DateTime.Now.AddSeconds(1);
            }
            response = "Commands:\n";
            response += "   General\n       .ttt_search = search/investigate a corpse\n";
            response += "   Credit Store\n      .ttt_list = List all store items\n      .ttt_buy item_id = Purchase a credit store item\n";
            return true;
        }
    }
}
