using PluginAPI.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPTroubleInTerroristTown.TTT.Players
{
    public class Notification
    {
        public string message { get; set; }
        public DateTime deathTime { get; set; }

        public Notification(string msg, DateTime deathTime)
        {
            this.message = msg;
            this.deathTime = deathTime;
        }
    }
    
    public class NotificationManager
    {
        Round.Round round;

        public Dictionary<Player, List<Notification>> playerNotificationList = new Dictionary<Player, List<Notification>>();

        public NotificationManager(Round.Round round)
        { 
            this.round = round;
        }
        public void Cleanup()
        {
            playerNotificationList.Clear();
        }

        public void PlayerNotify(Player player, string message, int duration=5)
        {
            if (!playerNotificationList.ContainsKey(player)) 
            {
                playerNotificationList.Add(player, new List<Notification>());
            }
            playerNotificationList[player].Add(new Notification(message, DateTime.Now.AddSeconds(duration)));
        }
        public void PlayerClearNotifications(Player player)
        {
            if (!playerNotificationList.ContainsKey(player))
                return;
            playerNotificationList[player].Clear();
        }
        public void PlayerCheckNotifications(Player player)
        {
            if (!playerNotificationList.ContainsKey(player))
                return;
            List<Notification> tobeRemoved= new List<Notification>();
            foreach(var notify in playerNotificationList[player])
            {
                if (DateTime.Now > notify.deathTime) 
                {
                    tobeRemoved.Add(notify);
                }
            }
            foreach(var notify in tobeRemoved)
            {
                playerNotificationList[player].Remove(notify);
            }
        }
        public void NotifyAll(string message, int duration=5)
        {
            foreach(Player pl in Player.GetPlayers())
            {
                PlayerNotify(pl, message, duration);
            }
        }
        public string getNotificationWidget(string hud, Player player)
        {
            if (!playerNotificationList.ContainsKey(player))
                return hud.Replace("{notify1}", "").Replace("{notify2}", "").Replace("{notify3}","");

            PlayerCheckNotifications(player);
            for (int i=0; i<3;i++)
            {
                string tobeReplaced = "{notify" + (i + 1).ToString() + "}";
                string replacement = "";
                if (i < playerNotificationList[player].Count)
                {
                    replacement = playerNotificationList[player][i].message;
                }
                hud = hud.Replace(tobeReplaced, replacement);
            }
            return hud;
        }



    }
}
