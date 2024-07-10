using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;

namespace SCP_SL_Trouble_In_Terrorist_Town.TTT
{
    internal class TTTCorpse
    {
        
   
        public static string GetCorpseInfo(TTTConfig config, Player player, TTTRound.Team playerTeam)
        {
            return $"\nThey were a <color={config.teamsConfig.TeamColor[playerTeam]}>{config.teamsConfig.TeamName[playerTeam]}</color>!";
        }
    }
}
