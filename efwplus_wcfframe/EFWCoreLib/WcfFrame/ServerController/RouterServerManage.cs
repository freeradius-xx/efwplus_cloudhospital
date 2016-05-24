using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Xml.Linq;
using EFWCoreLib.WcfFrame.WcfService.Contract;
using EFWCoreLib.WcfFrame.ServerController;
using System.Drawing;
using EFWCoreLib.WcfFrame.WcfService;

namespace EFWCoreLib.WcfFrame.ServerController
{
    public class RouterServerManage
    {
        public static  IDictionary<int, RegistrationInfo> RegistrationList = new Dictionary<int, RegistrationInfo>();
        public static  IDictionary<string, int> RoundRobinCount = new Dictionary<string, int>();
        public static HostWCFMsgHandler hostwcfMsg;
        public static HostWCFRouterListHandler hostwcfRouter;
        public static string ns = "http://www.efwplus.cn/";

        public static void Start()
        {
            hostwcfMsg(Color.Blue, DateTime.Now, "RouterHandlerService服务正在初始化...");
            RegistrationInfo.LoadRouterBill();
            hostwcfMsg(Color.Blue, DateTime.Now, "RouterHandlerService服务初始化完成");
            hostwcfRouter(RegistrationList.Values.ToList());
        }

        public static void Stop()
        {
            RegistrationList.Clear();
            hostwcfRouter(RegistrationList.Values.ToList());
        }

        /// <summary>
        /// 从注册表容器中根据Message的Action找到匹配的 binding和 endpointaddress
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="binding"></param>
        /// <param name="endpointAddress"></param>
        public static void GetServiceEndpoint(Message requestMessage, out EndpointAddress endpointAddress, out Uri touri)
        {
            string routerID = GetHeaderValue(requestMessage, "routerID", ns);
            string cmd = GetHeaderValue(requestMessage, "CMD", ns);
            string pluginname = GetHeaderValue(requestMessage, "Plugin", ns);
            //string stype = GetHeaderValue(requestMessage, "ServiceType", ns);
            string contractNamespace = requestMessage.Headers.Action.Substring(0, requestMessage.Headers.Action.LastIndexOf("/"));

            RegistrationInfo regInfo = null;

            if (RoundRobinCount.ContainsKey(routerID))
            {
                lock (RegistrationList)
                {
                    int key = RoundRobinCount[routerID];
                    regInfo = RegistrationList[key];
                    if (cmd == "Quit")
                    {

                        regInfo.ClientNum -= 1;
                    }
                }
            }
            else
            {
                lock (RegistrationList)
                {
                    List<KeyValuePair<int, RegistrationInfo>> krlist = RegistrationList.OrderBy(x => x.Value.ClientNum).ToList().FindAll(x => x.Value.ContractNamespace.Contains(contractNamespace));
                    if (krlist.Count > 0)
                    {
                        foreach (var r in krlist)
                        {
                            if (r.Value.pluginList.FindIndex(x => x.name == pluginname) > -1)
                            {
                                RoundRobinCount.Add(routerID, r.Key);
                                r.Value.ClientNum += 1;
                                regInfo = r.Value;
                                break;
                            }
                        }
                    }
                }

            }


            if (regInfo == null)
                throw new Exception("找不到对应的路由地址");

            Uri addressUri = new Uri(regInfo.Address);

            //binding = CustomBindConfig.GetRouterBinding(addressUri.Scheme);
            endpointAddress = new EndpointAddress(regInfo.Address);
            //重设Message的目标终结点
            touri = new Uri(regInfo.Address);

            hostwcfRouter(RegistrationList.Values.ToList());
        }

        public static void GetServiceEndpointFile(Message requestMessage, out EndpointAddress endpointAddress, out Uri touri)
        {
            string contractNamespace = requestMessage.Headers.Action.Substring(0, requestMessage.Headers.Action.LastIndexOf("/"));

            RegistrationInfo regInfo = null;

            lock (RegistrationList)
            {
                List<KeyValuePair<int, RegistrationInfo>> krlist = RegistrationList.OrderBy(x => x.Value.ClientNum).ToList().FindAll(x => x.Value.ContractNamespace.Contains(contractNamespace));
                if (krlist.Count > 0)
                {
                    regInfo = krlist.First().Value;
                }
            }


            if (regInfo == null)
                throw new Exception("找不到对应的路由地址");

            Uri addressUri = new Uri(regInfo.Address);

            //binding = CustomBindConfig.GetRouterBinding(addressUri.Scheme);
            endpointAddress = new EndpointAddress(regInfo.Address);
            //重设Message的目标终结点
            touri = new Uri(regInfo.Address);

            hostwcfRouter(RegistrationList.Values.ToList());
        }


        public static string GetHeaderValue(Message requestMessage, string name, string ns)
        {
            var headers = requestMessage.Headers;
            var index = headers.FindHeader(name, ns);
            if (index > -1)
                return headers.GetHeader<string>(index);
            else
                return null;
        }
    }

    public delegate void HostWCFRouterListHandler(List<RegistrationInfo> dic);

    [DataContract]
    public class RegistrationInfo
    {
        [DataMember(IsRequired = true, Order = 1)]
        public string HostName { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public string ServiceType { get; set; }

        [DataMember(IsRequired = true, Order = 3)]
        public string Address { get; set; }

        [DataMember(IsRequired = true, Order = 4)]
        public string ContractName { get; set; }

        [DataMember(IsRequired = true, Order = 5)]
        public string ContractNamespace { get; set; }

        [DataMember(IsRequired = true, Order = 6)]
        public List<PluginInfo> pluginList { get; set; }

        [DataMember(IsRequired = true, Order = 7)]
        public int ClientNum { get; set; }

        public override int GetHashCode()
        {
            return this.Address.GetHashCode() + this.ContractName.GetHashCode() + this.ContractNamespace.GetHashCode();
        }

        /// <summary>
        /// 加载路由器的路由表
        /// </summary>
        public static void LoadRouterBill()
        {
            string _hostname = null;
            string _servicetype = null;
            string _address = null;
            string _contractname = null;
            string _contractnamespace = null;

            XElement xes = XElement.Load(System.Windows.Forms.Application.StartupPath + "\\Config\\RouterBill.xml");
            IEnumerable<XElement> elements = from e in xes.Elements("record")
                                             select e;
            foreach (XElement xe in elements)
            {
                _hostname = xe.Element("hostname").Value;
                _servicetype = xe.Element("servicetype").Value;
                _address = xe.Element("address").Value;
                _contractname = xe.Element("ContractName").Value;
                _contractnamespace = xe.Element("ContractNamespace").Value;
                RegistrationInfo registrationInfo = new RegistrationInfo { HostName = _hostname, ServiceType = _servicetype, Address = _address, ContractName = _contractname, ContractNamespace = _contractnamespace };
                registrationInfo.pluginList = new List<PluginInfo>();
                IEnumerable<XElement> elementps = from p in xe.Elements("plugins")
                                                  select p;
                foreach (XElement ps in elementps)
                {
                    string name = ps.Element("plugin").Attribute("name").Value;
                    string title = ps.Element("plugin").Attribute("title").Value;
                    PluginInfo plugin = new PluginInfo();
                    plugin.name = name;
                    plugin.title = title;
                    registrationInfo.pluginList.Add(plugin);
                }

                if (!RouterServerManage.RegistrationList.ContainsKey(registrationInfo.GetHashCode()))
                {
                    RouterServerManage.RegistrationList.Add(registrationInfo.GetHashCode(), registrationInfo);
                }
            }

        }
    }

    [DataContract]
    public class PluginInfo
    {
        [DataMember(IsRequired = true, Order = 1)]
        public string name { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public string title { get; set; }
    }
}
