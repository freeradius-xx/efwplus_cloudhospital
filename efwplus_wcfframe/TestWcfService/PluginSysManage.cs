using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestWcfService
{
    /// <summary>
    /// pluginsys.xml 解析
    /// </summary>
    public class PluginSysManage
    {
        public static string localpath = System.Windows.Forms.Application.StartupPath;

        private static System.Xml.XmlDocument xmlDoc = null;
        private static string _pluginsysFile = System.Windows.Forms.Application.StartupPath + "\\Config\\pluginsys.xml";
        public static string pluginsysFile
        {
            get { return _pluginsysFile; }
            set
            {
                _pluginsysFile = value;
                xmlDoc = null;
            }
        }

        private static void InitConfig()
        {
            xmlDoc = new System.Xml.XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;//忽略文档里面的注释
            XmlReader reader = XmlReader.Create(pluginsysFile, settings);
            xmlDoc.Load(reader);
            reader.Close();
        }

        public static List<PluginClass> GetAllPlugin()
        {
            if (xmlDoc == null) InitConfig();
            List<PluginClass> plist = new List<PluginClass>();
            XmlNodeList nl = null;

            nl = xmlDoc.DocumentElement.SelectNodes("WebModulePlugin/Plugin");
            foreach (XmlNode n in nl)
            {
                PluginClass plugin = new PluginClass();
                plugin.plugintype = "WebModulePlugin";
                plugin.name = n.Attributes["name"].Value;
                plugin.title = n.Attributes["title"].Value;
                plugin.path = n.Attributes["path"].Value;
                plugin.isdevelopment = n.Attributes["isdevelopment"].Value;
                plist.Add(plugin);
            }
            nl = xmlDoc.DocumentElement.SelectNodes("WinformModulePlugin/Plugin");
            foreach (XmlNode n in nl)
            {
                PluginClass plugin = new PluginClass();
                plugin.plugintype = "WinformModulePlugin";
                plugin.name = n.Attributes["name"].Value;
                plugin.title = n.Attributes["title"].Value;
                plugin.path = n.Attributes["path"].Value;
                plugin.isdevelopment = n.Attributes["isdevelopment"].Value;
                plist.Add(plugin);
            }
            nl = xmlDoc.DocumentElement.SelectNodes("WcfModulePlugin/Plugin");
            foreach (XmlNode n in nl)
            {
                PluginClass plugin = new PluginClass();
                plugin.plugintype = "WcfModulePlugin";
                plugin.name = n.Attributes["name"].Value;
                plugin.title = n.Attributes["title"].Value;
                plugin.path = n.Attributes["path"].Value;
                plugin.isdevelopment = n.Attributes["isdevelopment"].Value;
                plist.Add(plugin);
            }
            return plist;
        }
    }

    public class PluginClass
    {
        public string plugintype { get; set; }

        public string name { get; set; }
        public string title { get; set; }
        public string path { get; set; }
        public string isdevelopment { get; set; }

        public string version { get; set; }
        public string author { get; set; }

    }
}
