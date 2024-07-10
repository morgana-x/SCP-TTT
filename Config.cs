using Exiled.API.Interfaces;

namespace SCP_SL_Trouble_In_Terrorist_Town
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        public TTTConfig TTTConfig { get; set; } = new TTTConfig();
    }
}