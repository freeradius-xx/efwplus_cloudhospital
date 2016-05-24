using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EFWWin
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


        public static void AddPlugin(string pluginType, string name, string path, string title, string isdevelopment)
        {
            if (xmlDoc == null) InitConfig();
            XmlNode root = xmlDoc.DocumentElement.SelectSingleNode(pluginType);//查找   
            XmlElement xe1 = xmlDoc.CreateElement("Plugin");//创建一个节点   
            xe1.SetAttribute("name", name);//
            xe1.SetAttribute("path", path);//
            xe1.SetAttribute("title", title);//
            xe1.SetAttribute("isdevelopment", isdevelopment);//

            root.AppendChild(xe1);//添加到<bookstore>节点中   
            xmlDoc.Save(pluginsysFile);
        }

        public static PluginClass GetPlugin(string pluginType, string name)
        {
            if (xmlDoc == null) InitConfig();
            XmlNodeList nl = xmlDoc.DocumentElement.SelectSingleNode(pluginType).SelectNodes("Plugin");
            foreach (XmlNode n in nl)
            {
                if (n.Attributes["name"].Value == name)
                {
                    PluginClass plugin = new PluginClass();
                    plugin.plugintype = pluginType;
                    plugin.name = n.Attributes["name"].Value;
                    plugin.title = n.Attributes["title"].Value;
                    plugin.path = n.Attributes["path"].Value;
                    plugin.isdevelopment = n.Attributes["isdevelopment"].Value;
                    return plugin;
                }
            }
            return null;
        }

        public static bool ContainsPlugin(string pluginType, string name)
        {
            if (xmlDoc == null) InitConfig();
            XmlNodeList nl = xmlDoc.DocumentElement.SelectSingleNode(pluginType).SelectNodes("Plugin");
            foreach (XmlNode n in nl)
            {
                if (n.Attributes["name"].Value == name)
                    return true;
            }
            return false;
        }

        public static void DeletePlugin(string pluginType)
        {
            if (xmlDoc == null) InitConfig();
            XmlNode xn = xmlDoc.DocumentElement.SelectSingleNode(pluginType);
            if (xn != null)
                xn.RemoveAll();
            xmlDoc.Save(pluginsysFile);
        }

        public static void DeletePlugin(string pluginType, string name)
        {
            if (xmlDoc == null) InitConfig();
            XmlNode xn = xmlDoc.DocumentElement.SelectSingleNode(pluginType + "/Plugin[@name='" + name + "']");
            if (xn != null)
                xn.ParentNode.RemoveChild(xn);
            xmlDoc.Save(pluginsysFile);
        }

        public static void GetWinformEntry(out string entryplugin, out string entrycontroller)
        {
            if (xmlDoc == null) InitConfig();

            entryplugin = xmlDoc.DocumentElement.SelectNodes("WinformModulePlugin")[0].Attributes["EntryPlugin"].Value.ToString();
            entrycontroller = xmlDoc.DocumentElement.SelectNodes("WinformModulePlugin")[0].Attributes["EntryController"].Value.ToString();
        }

        public static void SetWinformEntry(string entryplugin, string entrycontroller)
        {
            if (xmlDoc == null) InitConfig();

            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("WinformModulePlugin");
            node.Attributes["EntryPlugin"].Value = entryplugin;
            node.Attributes["EntryController"].Value = entrycontroller;

            xmlDoc.Save(pluginsysFile);
        }

        public static void GetWcfClientEntry(out string entryplugin, out string entrycontroller)
        {
            if (xmlDoc == null) InitConfig();

            entryplugin = xmlDoc.DocumentElement.SelectNodes("WcfModulePlugin")[0].Attributes["EntryPlugin"].Value.ToString();
            entrycontroller = xmlDoc.DocumentElement.SelectNodes("WcfModulePlugin")[0].Attributes["EntryController"].Value.ToString();
        }

        public static void SetWcfClientEntry(string entryplugin, string entrycontroller)
        {
            if (xmlDoc == null) InitConfig();

            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("WcfModulePlugin");
            node.Attributes["EntryPlugin"].Value = entryplugin;
            node.Attributes["EntryController"].Value = entrycontroller;

            xmlDoc.Save(pluginsysFile);
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
