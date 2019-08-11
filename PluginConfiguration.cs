using Rocket.API;
using System.Collections.Generic;

namespace RestartServer
{
    public class PluginConfiguration : IRocketPluginConfiguration
    {
        public bool Enabled;
        public Server Server;

        public void LoadDefaults()
        {
            Server = new Server() { StartPath = @"E:\Program Files (x86)\steam\steamapps\common\Unturned\Servers\test\Rocket\Plugins\Unturned - Shortcut.lnk" };
            Enabled = true;
        }
    }
}
