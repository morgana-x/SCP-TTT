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
            onPlayerTogglingNoclip.Invoke(null, new ToggleNoclipArgs(player));
            if (FpcNoclip.IsPermitted(player))
                return true;
            return false;
        }
    }
}
