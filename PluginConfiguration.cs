using Rocket.API;
namespace ConfigureLobbyInfo
{
    public class PluginConfiguration : IRocketPluginConfiguration
    {
        public bool PluginEnabled;
        public bool HidePlugins;
        public bool HideWorkshop;
        public bool HideConfig;
        public bool IsPVP;
        public bool IsBattlEyeSecure;
        public string Mode;
        public bool HasCheats;
        public string Perspective;
        public bool IsGold;
        public string Game;

        public void LoadDefaults()
        {
            this.PluginEnabled = true;
            this.HidePlugins = true;
            this.HideWorkshop = true;
            this.HideConfig = true;
            this.IsPVP = true;
            this.IsBattlEyeSecure = true;
            this.Mode = "normal";
            this.HasCheats = true;
            this.Perspective = "both";
            this.IsGold = false;
            this.Game = "Survival, survival and again survival...";
        }
    }
}
