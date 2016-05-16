using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFWCoreLib.CoreFrame.Init;
using EFWCoreLib.CoreFrame.Init.AttributeManager;
using Newtonsoft.Json;

namespace EFWCoreLib.WcfFrame.ServerController
{
    public class DebugWcfServices
    {
        public static string getWcfServicesAllInfo()
        {
            List<dwPlugin> pluginlist=new List<dwPlugin>();
            foreach (var item in AppPluginManage.PluginDic)
            {
                dwPlugin p = new dwPlugin();
                p.pluginname = item.Key;
                p.controllerlist = new List<dwController>();
                List<WcfControllerAttributeInfo> cmdControllerList = (List<WcfControllerAttributeInfo>)item.Value.cache.GetData(item.Key + "@" + "wcfControllerAttributeList");
                foreach (var cmd in cmdControllerList)
                {
                    dwController c = new dwController();
                    c.controllername = cmd.controllerName;
                    c.methodlist = new List<string>();
                    foreach (var m in cmd.MethodList)
                    {
                        c.methodlist.Add(m.methodName);
                    }
                    p.controllerlist.Add(c);
                }
                pluginlist.Add(p);
            }

            return JsonConvert.SerializeObject(pluginlist);
        }
    }

    public class dwPlugin
    {
        public string pluginname { get; set; }
        public List<dwController> controllerlist { get; set; }

    }

    public class dwController
    {
        public string controllername { get; set; }
        public List<string> methodlist { get; set; }
    }
}
