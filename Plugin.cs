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
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using System.Reflection;

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
                Level.onPostLevelLoaded += OnPostLevelLoaded;
            }
            else
            {
                Logger.Log("Plugin is disabled in Configuration");
                UnloadPlugin();
            }
        }
        protected override void Unload()
        {
            Level.onPostLevelLoaded -= OnPostLevelLoaded;
        }

        public static int GetWorkshopCount() =>
            (String.Join(",", Provider.getServerWorkshopFileIDs().Select(x => x.ToString()).ToArray()).Length - 1) / 120 + 1;
        public static int GetConfigurationCount() =>
            (String.Join(",", typeof(ModeConfigData).GetFields()
            .SelectMany(x => x.FieldType.GetFields().Select(y => y.GetValue(x.GetValue(Provider.modeConfigData))))
            .Select(x => x is bool v ? v ? "T" : "F" : (String.Empty + x)).ToArray()).Length - 1) / 120 + 1;

        public void OnPostLevelLoaded(int a) => ConfigureLobby();
        public void HideRocketPlugins(bool hide)
        {

        }
        public void ConfigureLobby()
        {
            string mode;
            string perspective;

            if (Configuration.Instance.HidePlugins)
                SteamGameServer.SetKeyValue("rocketplugins", "");
            else
                SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", R.Plugins.GetPlugins().Select(plugin => plugin.Name).ToArray()));

            if (Configuration.Instance.HideWorkshop)
            {
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", "");
                SteamGameServer.SetKeyValue("Browser_Workshop_Line_", "");
                SteamGameServer.SetKeyValue("rocketplugins", "");
            }
            else
            {
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", GetWorkshopCount().ToString());
            }

            if (Configuration.Instance.HideConfig)
                SteamGameServer.SetKeyValue("Browser_Config_Count", "0");
            else
                SteamGameServer.SetKeyValue("Browser_Config_Count", GetConfigurationCount().ToString());

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