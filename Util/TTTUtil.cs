using CentralAuth;
using Exiled.API.Features;
using Exiled.API.Features.Components;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCP_SL_Trouble_In_Terrorist_Town.Util
{
    public class TTTUtil
    {
        private static System.Random rng = new System.Random();

        public static void RandomShuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

      
    }
}
