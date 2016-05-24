using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFWCoreLib.WcfFrame
{
    public class ClientLinkManage
    {
        public static bool IsMessage = false;
        public static int MessageTime = 1;//默认间隔1秒
        /// <summary>
        /// 缓存的客户连接
        /// </summary>
        private static Dictionary<string, ClientLink> ClientLinkDic = new Dictionary<string, ClientLink>();
        /// <summary>
        /// 创建wcf服务连接
        /// </summary>
        public static ClientLink CreateConnection(string pluginname)
        {
            try
            {
                if (ClientLinkDic.ContainsKey(pluginname))
                {
                    return ClientLinkDic[pluginname];
                }

                ClientLink link = new ClientLink(null, pluginname, ((ism, met) =>
                {
                    IsMessage = ism;
                    MessageTime = met;
                }));
                link.CreateConnection();
                ClientLinkDic.Add(pluginname, link);
                return link;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// 卸载连接
        /// </summary>
        public static void UnConnection(string pluginname)
        {
            if (ClientLinkDic.Count == 0) return;
            if (ClientLinkDic.ContainsKey(pluginname) == false) return;
            ClientLinkDic[pluginname].Dispose();
            ClientLinkDic.Remove(pluginname);
        }

        public static void UnAllConnection()
        {
            if (ClientLinkDic.Count == 0) return;
            foreach (var c in ClientLinkDic)
            {
                c.Value.Dispose();
            }
            ClientLinkDic.Clear();
        }
    }
}
