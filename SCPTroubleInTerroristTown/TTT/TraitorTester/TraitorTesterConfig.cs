namespace SCPTroubleInTerroristTown.TTT.TraitorTester
{
    public class TraitorTesterConfig
    {
        public string CantUseBroadcast { get; set; } = "<color=red>You need to be a <color=blue>Detective</color> to use the Traitor tester!</color>";
        public string NeedXPlayers { get; set; } = "<color=red>Need atleast <color=yellow>{x}</color> Players inside the <color=yellow>Left chamber</color> to activate the tester!</color>";

        public int MinimumPlayers { get; set; } = 2;

        public bool AllowNonDetective { get; set; } = false;
    }

}
