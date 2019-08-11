using Rocket.API;
using Rocket.Core.Plugins;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace RestartServer
{
    class Plugin : RocketPlugin<PluginConfiguration>
    {
        private static Plugin Instance;
        internal static string path;
        protected override void Load()
        {
            if (Configuration.Instance.Enabled)
            {
                Instance = this;
                path = $@"Plugins\RestartServer\RestartServer.configuration.xml";
            }
            else
            {
                UnloadPlugin();
            }
        }
        protected override void Unload()
        {
        }
    }
    public class Server
    {
        [XmlAttribute("StartPath")]
        public string StartPath { get; set; }
        public Server()
        {

        }
    }
    class CommandRestart : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "restart";
        public string Help => "command to restart your server";
        public string Syntax => "[/restart]  [/restart <after seconds>]  [/restart <restart index> <every seconds> on]  [/restart <restart index> <every seconds> off]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "rocket.restartserver" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(PluginConfiguration));
            Server server;
            using (FileStream fs = new FileStream(Plugin.path, FileMode.Open))
            {
                PluginConfiguration config = (PluginConfiguration)formatter.Deserialize(fs);
                server = config.Server;
            }
            Application.Quit();
            System.Diagnostics.Process.Start(server.StartPath);
        }
    }
}