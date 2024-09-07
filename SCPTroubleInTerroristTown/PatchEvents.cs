using Hints;
using PlayerRoles.FirstPersonControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPTroubleInTerroristTown
{
    public static class PatchEvents
    {
        public static EventHandler<ToggleNoclipArgs> onPlayerTogglingNoclip;
        public class ToggleNoclipArgs : EventArgs
        {
            public ReferenceHub referenceHub { get; set; }

            public ToggleNoclipArgs(ReferenceHub pl)
            {
                referenceHub = pl;
            }
        }
        public static bool OnPlayerTogglingNoClip(ReferenceHub player)
        {
            onPlayerTogglingNoClip(new ToggleNoclipArgs(player));
            if (FpcNoclip.IsPermitted(player))
                return true;
            return false;
        }
        static void onPlayerTogglingNoClip(ToggleNoclipArgs ev)
        {
            onPlayerTogglingNoclip.Invoke(null,ev);
        }


    }
}
