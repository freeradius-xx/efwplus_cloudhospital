using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using EFWCoreLib.CoreFrame.Business;
using EFWCoreLib.WcfFrame.ServerController;
using EFWCoreLib.WcfFrame.WcfService.Contract;
using System.Reflection;
using EFWCoreLib.CoreFrame.Init;
using EFWCoreLib.CoreFrame.Common;
using System.Drawing;
using EFWCoreLib.WcfFrame.ClientController;

namespace EFWCoreLib.WcfFrame.ServerController
{
    /// <summary>
    /// WCF通讯服务端管理
    /// </summary>
    public class WcfServerManage
    {
        //客户端列表
        static Dictionary<string, WCFClientInfo> wcfClientDic = new Dictionary<string, WCFClientInfo>();
        public static string HostName = "";
        public static bool IsDebug = false;
        public static bool IsHeartbeat = false;
        public static int HeartbeatTime = 1;//默认间隔1秒,客户端5倍
        public static bool IsMessage = false;
        public static int MessageTime = 1;//默认间隔1秒
        public static bool IsCompressJson = false;//是否压缩Json数据
        public static bool IsEncryptionJson = false;//是否加密Json数据
        public static bool IsOverTime = false;
        public static int OverTime = 1;//超时记录日志

        /// <summary>
        /// 开始服务主机
        /// </summary>
        public static void StartWCFHost()
        {
            hostwcfMsg(Color.Blue, DateTime.Now, "WCFHandlerService服务正在初始化...");
            AppGlobal.AppStart();
            hostwcfMsg(Color.Blue, DateTime.Now, "WCFHandlerService服务初始化完成");

            if (IsHeartbeat == true)
            {
                if (timer == null)
                    StartListenClients();
                else
                    timer.Start();
            }
            else
            {
                if (timer != null)
                    timer.Stop();
            }
        }
        /// <summary>
        /// 停止服务主机
        /// </summary>
        public static void StopWCFHost()
        {
            foreach (WCFClientInfo client in wcfClientDic.Values)
            {
                client.IsConnect = false;
            }
        }

        #region 服务操作
        public static string CreateClient(string sessionId, string ipaddress, DateTime time, IClientService clientService)
        {
            string clientId = Guid.NewGuid().ToString();

            try
            {
                AddClient(sessionId, clientId, ipaddress, time, clientService);
                hostwcfclientinfoList(wcfClientDic.Values.ToList());
                return clientId;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Source + "：创建客户端运行环境失败！");
            }
        }
        public static bool Heartbeat(string clientId)
        {   
            bool b= UpdateHeartbeatClient(clientId);
            if (b == true)
            {
                ReConnectionClient(clientId);
                return true;
            }
            else
                return false;
        }
        public static string ProcessRequest(string clientId, string controller, string method, string jsondata)
        {
            string retJson = null;
            WCFClientInfo ClientInfo = null;
            try
            {
                lock (wcfClientDic)
                {
                    if (wcfClientDic.ContainsKey(clientId) == false)
                        throw new Exception("客户端不存在，正在创建新的连接！");

                    ClientInfo = wcfClientDic[clientId].Clone() as WCFClientInfo;
                }

                //显示调试信息
                if (WcfServerManage.IsDebug == true)
                    ShowHostMsg(Color.Black, DateTime.Now, "客户端[" + clientId + "]正在执行：" + controller + "." + method + "(" + jsondata + ")");


                begintime();
                object[] paramValue = null;//jsondata?
                string[] names = controller.Split(new char[] { '@' });
                if (names.Length != 2)
                    throw new Exception("控制器名称错误!");
                string pluginname = names[0];
                string cname = names[1];

                Object retObj = null;
                if (AppPluginManage.PluginDic.ContainsKey(pluginname) == true)
                {
                    //解压参数
                    string _jsondata = jsondata;
                    if (WcfServerManage.IsCompressJson)
                    {
                        _jsondata = ZipComporessor.Decompress(jsondata);
                        //_jsondata = JsonComporessor.Decompress(jsondata);
                    }

                    EFWCoreLib.CoreFrame.Plugin.ModulePlugin plugin = AppPluginManage.PluginDic[pluginname];
                    retObj = plugin.WcfServerExecuteMethod(cname, method, paramValue, _jsondata, ClientInfo);

                    if (retObj != null)
                        retJson = retObj.ToString();


                    retJson = "{\"flag\":0,\"msg\":" + "\"\"" + ",\"data\":" + retJson + "}";
                    //压缩结果
                    if (WcfServerManage.IsCompressJson)
                    {
                        retJson = ZipComporessor.Compress(retJson);
                        //retJson = JsonComporessor.Compress(retJson);
                    }
                }
                else//本地插件找不到，就执行远程插件
                {
                    foreach (var rp in AppPluginManage.RemotePluginDic)
                    {
                        if (rp.plugin.ToList().FindIndex(x => x == pluginname) > -1)
                        {
                            retJson = rp.clientService.SuperReplyClient(pluginname, cname, method, jsondata);
                            break;
                        }
                    }
                }

                //System.Threading.Thread.Sleep(20000);//测试并发问题，此处也没有问题
                double outtime = endtime();
                //记录超时的方法
                if (WcfServerManage.IsOverTime == true)
                {
                    if (outtime > Convert.ToDouble(WcfServerManage.OverTime * 1000))
                    {
                        WriterOverTimeLog(outtime, controller + "." + method + "(" + jsondata + ")");
                    }
                }
                //显示调试信息
                if (WcfServerManage.IsDebug == true)
                    ShowHostMsg(Color.Green, DateTime.Now, "客户端[" + clientId + "]收到结果(耗时[" + outtime + "])：" + retJson);

                //更新客户端信息
                UpdateRequestClient(clientId, jsondata == null ? 0 : jsondata.Length, retJson == null ? 0 : retJson.Length);


                if (retJson == null)
                    throw new Exception("请求的插件未获取到有效数据");

                return retJson;
            }
            catch (Exception err)
            {
                //记录错误日志
                //EFWCoreLib.CoreFrame.EntLib.ZhyContainer.CreateException().HandleException(err, "HISPolicy");

                if (err.InnerException == null)
                {
                    retJson = "{\"flag\":1,\"msg\":" + "\"" + err.Message + "\"" + "}";
                    if (WcfServerManage.IsCompressJson)
                    {
                        retJson = ZipComporessor.Compress(retJson);
                        //retJson = JsonComporessor.Compress(retJson);
                    }
                    ShowHostMsg(Color.Red, DateTime.Now, "客户端[" + clientId + "]执行失败：" + err.Message);
                    return retJson;
                }
                else
                {
                    retJson = "{\"flag\":1,\"msg\":" + "\"" + err.InnerException.Message + "\"" + "}";
                    if (WcfServerManage.IsCompressJson)
                    {
                        retJson = ZipComporessor.Compress(retJson);
                        //retJson = JsonComporessor.Compress(retJson);
                    }
                    ShowHostMsg(Color.Red, DateTime.Now, "客户端[" + clientId + "]执行失败：" + err.InnerException.Message);
                    return retJson;
                }
            }
        }
        public static bool UnClient(string clientId)
        {
            RemoveClient(clientId);
            hostwcfclientinfoList(wcfClientDic.Values.ToList());
            return true;
        }
        public static void SendBroadcast(string jsondata)
        {
            lock (wcfClientDic)
            {
                foreach (WCFClientInfo client in wcfClientDic.Values)
                {
                    IClientService mCallBack = client.clientServiceCallBack;
                    //?
                    mCallBack.ReplyClient(jsondata);
                }
            }
        }
        public static string ServerConfig()
        {
            string IsHeartbeat = WcfServerManage.IsHeartbeat ? "1" : "0";
            string HeartbeatTime = WcfServerManage.HeartbeatTime.ToString();
            string IsMessage = WcfServerManage.IsMessage ? "1" : "0";
            string MessageTime = WcfServerManage.MessageTime.ToString();
            string IsCompressJson = WcfServerManage.IsCompressJson ? "1" : "0";
            string IsEncryptionJson = WcfServerManage.IsEncryptionJson ? "1" : "0";

            StringBuilder sb = new StringBuilder();
            sb.Append(IsHeartbeat);
            sb.Append("#");
            sb.Append(HeartbeatTime);
            sb.Append("#");
            sb.Append(IsMessage);
            sb.Append("#");
            sb.Append(MessageTime);
            sb.Append("#");
            sb.Append(IsCompressJson);
            sb.Append("#");
            sb.Append(IsEncryptionJson);
            return sb.ToString();
        }
        #endregion

        #region 界面显示
        public static HostWCFClientInfoListHandler hostwcfclientinfoList;
        public static HostWCFMsgHandler hostwcfMsg;
        private static void AddClient(string sessionId, string clientId, string ipaddress, DateTime time, IClientService clientService)
        {
            WCFClientInfo info = new WCFClientInfo();
            info.clientId = clientId;
            info.ipAddress = ipaddress;
            info.startTime = time;
            info.clientServiceCallBack = clientService;
            info.IsConnect = true;
            lock (wcfClientDic)
            {
                wcfClientDic.Add(clientId, info);
            }
            ShowHostMsg(Color.Blue, DateTime.Now, "客户端[" + ipaddress + "]已连接WCF服务主机");
        }
        private static bool UpdateRequestClient(string clientId, int rlen, int slen)
        {
            lock (wcfClientDic)
            {
                if (wcfClientDic.ContainsKey(clientId))
                {

                    wcfClientDic[clientId].RequestCount += 1;
                    wcfClientDic[clientId].receiveData += rlen;
                    wcfClientDic[clientId].sendData += slen;
                }
            }
            return true;
        }
        private static bool UpdateHeartbeatClient(string clientId)
        {
            lock (wcfClientDic)
            {
                if (wcfClientDic.ContainsKey(clientId))
                {

                    wcfClientDic[clientId].startTime = DateTime.Now;
                    wcfClientDic[clientId].HeartbeatCount += 1;
                    return true;
                }
                else
                    return false;
            }
        }
        private static void RemoveClient(string clientId)
        {
            lock (wcfClientDic)
            {
                if (wcfClientDic.ContainsKey(clientId))
                {

                    wcfClientDic.Remove(clientId);
                    ShowHostMsg(Color.Blue, DateTime.Now, "客户端[" + clientId + "]已退出断开连接WCF服务主机");
                }
            }
        }
        private static void ReConnectionClient(string clientId)
        {
            lock (wcfClientDic)
            {
                if (wcfClientDic.ContainsKey(clientId))
                {
                    if (wcfClientDic[clientId].IsConnect == false)
                    {
                        ShowHostMsg(Color.Blue,DateTime.Now, "客户端[" + clientId + "]已重新连接WCF服务主机");
                        wcfClientDic[clientId].IsConnect = true;
                    }
                }
            }
        }
        private static void DisConnectionClient(string clientId)
        {
            lock (wcfClientDic)
            {
                if (wcfClientDic.ContainsKey(clientId))
                {

                    if (wcfClientDic[clientId].IsConnect == true)
                    {
                        wcfClientDic[clientId].IsConnect = false;
                        ShowHostMsg(Color.Blue,DateTime.Now, "客户端[" + clientId + "]已超时断开连接WCF服务主机");
                    }
                }
            }
        }
        private static void ShowHostMsg(Color clr,DateTime time, string text)
        {
            //lock (hostwcfMsg)
            //{
                hostwcfMsg.BeginInvoke(clr,time, text, null, null);//异步方式不影响后台数据请求
                //hostwcfMsg(time, text);
            //}
        }
        #endregion

        #region 定时刷新界面
        //检测客户端是否在线，超时时间为10s
        static System.Timers.Timer timer;
        private static void StartListenClients()
        {
            timer = new System.Timers.Timer();
            timer.Interval = WcfServerManage.HeartbeatTime*1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }
        static Object syncObj = new Object();////定义一个静态对象用于线程部份代码块的锁定，用于lock操作
        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                lock (syncObj)
                {
                    foreach (WCFClientInfo client in wcfClientDic.Values)
                    {
                        if (client.startTime.AddSeconds(WcfServerManage.HeartbeatTime * 10) < DateTime.Now)//断开10秒就置为断开
                        {
                            DisConnectionClient(client.clientId);
                        }

                        if (client.startTime.AddSeconds(WcfServerManage.HeartbeatTime * 100) < DateTime.Now)//断开10分钟直接移除客户端
                        {
                            RemoveClient(client.clientId);
                        }
                    }
                    hostwcfclientinfoList(wcfClientDic.Values.ToList());
                    WriterOverTimeFile();
                }
            }
            catch { }
        }
        #endregion

        #region 超时服务请求记录
        static DateTime begindate;
        static void begintime()
        {
            begindate = DateTime.Now;
        }
        //返回毫秒
        static double endtime()
        {
            return DateTime.Now.Subtract(begindate).TotalMilliseconds;
        }
        static StringBuilder overtimesb = new StringBuilder();
        static void WriterOverTimeLog(double overtime, string text)
        {
            string info = "时间：" + DateTime.Now.ToString() + "\t\t" + "耗时：" + overtime + "\t\t" + "方法：" + text + "\r\n";
            lock (overtimesb)
            {
                overtimesb.AppendLine(info);
            }
        }
        static void WriterOverTimeFile()
        {
            string info = null;
            lock (overtimesb)
            {
                info = overtimesb.ToString();
                if (info != null)
                    overtimesb.Clear();
            }
            if (string.IsNullOrEmpty(info) == false)
            {
                string filepath = AppGlobal.AppRootPath + "OverTimeLog\\" + DateTime.Now.ToString("yyyyMM") + ".txt";
                if (System.IO.Directory.Exists(AppGlobal.AppRootPath + "OverTimeLog") == false)
                {
                    System.IO.Directory.CreateDirectory(AppGlobal.AppRootPath + "OverTimeLog");
                }
                System.IO.File.AppendAllText(filepath, info);
            }
        }
        #endregion

        #region 创建中间件与中间件之间的超级连接,通过EFWCore名称来转发
        static ClientLink superclient;
        public static void CreateSuperClient()
        {
            superclient = new ClientLink(HostName, "EFWCore");
            superclient.CreateConnection();
            superclient.RegisterReplyPlugin(HostName, AppPluginManage.PluginDic.Keys.ToArray());
            (superclient.mConn.ClientService as ReplyClientCallBack).SuperReplyClientAction=((string plugin, string controller, string method, string jsondata)=>{
                return ProcessReply(plugin,controller,method,jsondata);
            });
        }

        public static void UnCreateSuperClient()
        {
            if (superclient != null)
                superclient.Dispose();
        }

        public static void RegisterReplyPlugin(IClientService callback, string serverHostName, string[] plugin)
        {
            if (AppPluginManage.RemotePluginDic.FindIndex(x => x.serverHostName == serverHostName) > -1)
                AppPluginManage.RemotePluginDic.RemoveAll(x => x.serverHostName == serverHostName);

            RemotePlugin rp = new RemotePlugin();
            rp.serverHostName = serverHostName;
            rp.clientService = callback;
            rp.plugin = plugin;
            AppPluginManage.RemotePluginDic.Add(rp);
        }

        private static string ProcessReply(string plugin, string controller, string method, string jsondata)
        {
            string retJson = null;
            
            try
            {

                //显示调试信息
                if (WcfServerManage.IsDebug == true)
                    ShowHostMsg(Color.Black, DateTime.Now, "客户端[本地回调]正在执行：" + controller + "." + method + "(" + jsondata + ")");

                begintime();
                object[] paramValue = null;//jsondata?

                Object retObj = null;
                if (AppPluginManage.PluginDic.ContainsKey(plugin) == true)
                {
                    //解压参数
                    string _jsondata = jsondata;
                    if (WcfServerManage.IsCompressJson)
                    {
                        _jsondata = ZipComporessor.Decompress(jsondata);
                        //_jsondata = JsonComporessor.Decompress(jsondata);
                    }

                    EFWCoreLib.CoreFrame.Plugin.ModulePlugin _plugin = AppPluginManage.PluginDic[plugin];
                    retObj = _plugin.WcfServerExecuteMethod(controller, method, paramValue, _jsondata, null);

                    if (retObj != null)
                        retJson = retObj.ToString();


                    retJson = "{\"flag\":0,\"msg\":" + "\"\"" + ",\"data\":" + retJson + "}";
                    //压缩结果
                    if (WcfServerManage.IsCompressJson)
                    {
                        retJson = ZipComporessor.Compress(retJson);
                        //retJson = JsonComporessor.Compress(retJson);
                    }
                }

                //System.Threading.Thread.Sleep(20000);//测试并发问题，此处也没有问题
                double outtime = endtime();
                //记录超时的方法
                if (WcfServerManage.IsOverTime == true)
                {
                    if (outtime > Convert.ToDouble(WcfServerManage.OverTime * 1000))
                    {
                        WriterOverTimeLog(outtime, controller + "." + method + "(" + jsondata + ")");
                    }
                }
                //显示调试信息
                if (WcfServerManage.IsDebug == true)
                    ShowHostMsg(Color.Green, DateTime.Now, "客户端[本地回调]收到结果(耗时[" + outtime + "])：" + retJson);

                if (retJson == null)
                    throw new Exception("请求的插件未获取到有效数据");
               
                return retJson;
            }
            catch (Exception err)
            {
                //记录错误日志
                //EFWCoreLib.CoreFrame.EntLib.ZhyContainer.CreateException().HandleException(err, "HISPolicy");

                if (err.InnerException == null)
                {
                    retJson = "{\"flag\":1,\"msg\":" + "\"" + err.Message + "\"" + "}";
                    if (WcfServerManage.IsCompressJson)
                    {
                        retJson = ZipComporessor.Compress(retJson);
                        //retJson = JsonComporessor.Compress(retJson);
                    }
                    ShowHostMsg(Color.Red, DateTime.Now, "客户端[本地回调]执行失败：" + err.Message);
                    return retJson;
                }
                else
                {
                    retJson = "{\"flag\":1,\"msg\":" + "\"" + err.InnerException.Message + "\"" + "}";
                    if (WcfServerManage.IsCompressJson)
                    {
                        retJson = ZipComporessor.Compress(retJson);
                        //retJson = JsonComporessor.Compress(retJson);
                    }
                    ShowHostMsg(Color.Red, DateTime.Now, "客户端[本地回调]执行失败：" + err.InnerException.Message);
                    return retJson;
                }

            }
        }
        #endregion
    }

    /// <summary>
    /// 连接客户端信息
    /// </summary>
    public class WCFClientInfo : MarshalByRefObject,ICloneable
    {
        public string clientId { get; set; }
        public string ipAddress { get; set; }
        public DateTime startTime { get; set; }
        public IClientService clientServiceCallBack { get; set; }
        public SysLoginRight LoginRight { get; set; }
        public int HeartbeatCount { get; set; }
        public bool IsConnect { get; set; }
        /// <summary>
        /// 请求次数
        /// </summary>
        public int RequestCount { get; set; }
        /// <summary>
        /// 接收数据
        /// </summary>
        public long receiveData { get; set; }
        /// <summary>
        /// 发送数据
        /// </summary>
        public long sendData { get; set; }

        #region ICloneable 成员

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    public delegate void HostWCFClientInfoListHandler(List<WCFClientInfo> dic);
    public delegate void HostWCFMsgHandler(System.Drawing.Color clr, DateTime time, string text);
}
