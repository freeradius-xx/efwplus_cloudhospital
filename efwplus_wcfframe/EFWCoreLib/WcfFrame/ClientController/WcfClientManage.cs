using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using EFWCoreLib.WcfFrame.WcfService.Contract;
using EFWCoreLib.CoreFrame.Init;
using System.Net;
using System.Text.RegularExpressions;
using EFWCoreLib.CoreFrame.Common;
using EFWCoreLib.WcfFrame.ServerController;
using Newtonsoft.Json;
using System.IO;

namespace EFWCoreLib.WcfFrame.ClientController
{
    /// <summary>
    /// WCF通讯客户端管理
    /// </summary>
    public class WcfClientManage
    {
        private static Dictionary<string, ClientLink> ClientLinkDic=new Dictionary<string,ClientLink>();

        public static bool IsMessage = false;
        public static int MessageTime = 1;//默认间隔1秒
        
        /// <summary>
        /// 创建wcf服务连接
        /// </summary>
        public static void CreateConnection()
        {
            try
            {
                ClientLinkDic.Clear();

                foreach (var p in AppPluginManage.PluginDic)
                {
                    string pname = p.Value.plugin.name;
                    ClientLink link = new ClientLink(null, pname, ((ism, met) => {
                        IsMessage = ism;
                        MessageTime = met;
                    }));
                    link.CreateConnection();
                    ClientLinkDic.Add(pname, link);
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        /// <summary>
        /// 获取客户端连接
        /// </summary>
        /// <param name="pluginname"></param>
        /// <returns></returns>
        public static ClientLink GetClientLink(string pluginname)
        {
            if (ClientLinkDic.ContainsKey(pluginname)==false)
            {
                throw new Exception(pluginname+"#插件名不正确或无此插件");
            }

            return ClientLinkDic[pluginname];
        }
        /// <summary>
        /// 卸载连接
        /// </summary>
        public static void UnConnection()
        {
            if (ClientLinkDic.Count==0) return;

            foreach (var c in ClientLinkDic)
            {
                c.Value.Dispose();
            }
        }     
    }
}
