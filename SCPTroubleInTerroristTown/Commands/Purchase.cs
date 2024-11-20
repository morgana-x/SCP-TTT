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
    public class Purchase : ParentCommand, ICommand
    {
        public override string Command { get; } = "ttt_purchase";
        public override string Description { get; } = "Purchaes an item from the credit store!";

        public override string[] Aliases { get; } = new string[] { "buy", "ttt_buy" };
        public override void LoadGeneratedCommands() { }
        public static Dictionary<string, DateTime> Cooldowns = new Dictionary<string, DateTime>();

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {

            if (arguments.Count < 1)
            {
                response = "Missing argument! Correct Usage: ttt_purchase item_id";
                return false;
            }
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
            response = MainClass.Singleton.tttRound.creditManager.BuyCreditStoreItem(caller, arguments.First());
            return true;
        }
    }
}
