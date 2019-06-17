using System;
using System.Linq;
using System.Collections.Generic;
using Rocket.Core;
using Rocket.Core.Plugins;
using SDG.Unturned;
using SDG.Framework.Modules;
using Steamworks;
using SDG.Framework.Translations;
using System.Globalization;
using Logger = Rocket.Core.Logging.Logger;

namespace ConfigureLobbyInfo
{
    public class Plugin : RocketPlugin<PluginConfiguration>
    {
        private static Plugin Instance;
        protected override void Load()
        {
            if (Configuration.Instance.PluginEnabled)
            {
                Instance = this;
                Logger.Log("ConfigureLobbyInfo loaded!", ConsoleColor.Cyan);
                if(Level.isLoaded)
                    ConfigureLobby();
            }
            else
            {
                Logger.Log("Plugin is disabled in Configuration");
                this.Unload();
            }
        }

        public void ConfigureLobby()
        {
            string mode;
            string perspective;
            if (Configuration.Instance.HidePlugins)
                SteamGameServer.SetKeyValue("rocketplugins", "");
            else
                SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", Rocket.Core.R.Plugins.GetPlugins().Select(plugin => plugin.Name).ToArray()));
            
            switch (Configuration.Instance.Mode.ToLower().Trim())   
            {
                case ("easy"):
                    mode = "EZY";
                    break;
                case ("hard"):
                    mode = "HRD";
                    break;
                default:
                    mode = "NRM"; 
                    break;
            }
            switch (Configuration.Instance.Perspective.ToLower().Trim())
            {
                case ("first"):
                    perspective = "1Pp";
                    break;
                case ("third"):
                    perspective = "3Pp";
                    break;
                case ("vehicle"):
                    perspective = "4Pp";
                    break;
                default:
                    perspective = "2Pp";
                    break;
            }
            string tags = String.Concat(new string[]
            {
                Configuration.Instance.IsPVP ? "PVP" : "PVE",
                ",",
                Configuration.Instance.HasCheats ? "CHy" : "CHn",
                ",",
                mode,
                ",",
                perspective,
                ",",
                Configuration.Instance.HideWorkshop ? "WSn" : "WSy",
                ",",
                Configuration.Instance.IsGold ? "GLD" : "F2P",
                ",",
                Configuration.Instance.IsBattlEyeSecure ? "BEy" : "BEn",
                ",<gm>",
                Configuration.Instance.Game,
                "</gm>",
            });
            SteamGameServer.SetGameTags(tags);
        }
    }
}
