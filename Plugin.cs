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
                this.Unload();
            }
        }
        public void OnPostLevelLoaded(int a) => ConfigureLobby();
        public void ConfigureLobby()
        {
            string mode;
            string perspective;

            if (Configuration.Instance.HidePlugins)
                SteamGameServer.SetKeyValue("rocketplugins", "");
            else
                SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", Rocket.Core.R.Plugins.GetPlugins().Select(plugin => plugin.Name).ToArray()));

            if (Configuration.Instance.HideWorkshop)
            {
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", "0");
                SteamGameServer.SetKeyValue("Browser_Workshop_Line_", "0");
            }
            else
            {
                string empty = string.Empty;
                for (int index = 0; index < SDG.Unturned.Provider.getServerWorkshopFileIDs().Count; ++index)
                {
                    if (empty.Length > 0)
                        empty += (string)(object)',';
                    empty += (string)(object)SDG.Unturned.Provider.getServerWorkshopFileIDs()[index];
                }
                int num5 = (empty.Length - 1) / 120 + 1;
                int num6 = 0;
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", num5.ToString());
                for (int startIndex = 0; startIndex < empty.Length; startIndex += 120)
                {
                    int length = 120;
                    if (startIndex + length > empty.Length)
                        length = empty.Length - startIndex;
                    string pValue = empty.Substring(startIndex, length);
                    SteamGameServer.SetKeyValue("Browser_Workshop_Line_" + (object)num6, pValue);
                    ++num6;
                }
            }
            if (Configuration.Instance.HideConfig)
                SteamGameServer.SetKeyValue("Browser_Config_Count", "0");
            else
            {
                string str = string.Empty;
                foreach (FieldInfo field1 in SDG.Unturned.Provider.modeConfigData.GetType().GetFields())
                {
                    object obj1 = field1.GetValue((object)SDG.Unturned.Provider.modeConfigData);
                    foreach (FieldInfo field2 in obj1.GetType().GetFields())
                    {
                        object obj2 = field2.GetValue(obj1);
                        if (str.Length > 0)
                            str += (string)(object)',';
                        str = !(obj2 is bool) ? str + obj2 : str + (!(bool)obj2 ? "F" : "T");
                    }
                }
                int num7 = (str.Length - 1) / 120 + 1;
                int num8 = 0;
                SteamGameServer.SetKeyValue("Browser_Config_Count", num7.ToString());
                for (int startIndex = 0; startIndex < str.Length; startIndex += 120)
                {
                    int length = 120;
                    if (startIndex + length > str.Length)
                        length = str.Length - startIndex;
                    string pValue = str.Substring(startIndex, length);
                    SteamGameServer.SetKeyValue("Browser_Config_Line_" + (object)num8, pValue);
                    ++num8;
                }
            }
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
