using System;
using System.Collections.Generic;
using CommandSystem;
using PluginAPI.Core;
namespace SCPTroubleInTerroristTown.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class List : ParentCommand, ICommand
    {
        public override string Command { get; } = "ttt_list";
        public override string Description { get; } = "Lists all credit store items you can purchase";

        public override string[] Aliases { get; }= new string[] { "store" };
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
                Cooldowns.Add(caller.UserId, DateTime.Now.AddSeconds(5));
            }
            else
            {
                Cooldowns[caller.UserId] = DateTime.Now.AddSeconds(5);
            }
            response = MainClass.Singleton.tttRound.creditManager.GetCreditStoreItemsForPlayer(caller);
            return true;
        }
    }
}
