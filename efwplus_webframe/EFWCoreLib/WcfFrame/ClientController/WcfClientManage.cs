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
        #region 数据交换
        public static bool IsHeartbeat = false;
        public static int HeartbeatTime = 1;//默认间隔1秒,客户端5倍
        public static bool IsMessage = false;
        public static int MessageTime = 1;//默认间隔1秒
        public static bool IsCompressJson = false;//是否压缩Json数据
        public static bool IsEncryptionJson = false;//是否加密Json数据
        public static readonly string myNamespace = "http://www.efwplus.cn/";

        private static bool ServerConfigRequestState = false;//获取服务端配置读取状态
        private static DuplexChannelFactory<IWCFHandlerService> mChannelFactory;
        /// <summary>
        /// 创建wcf服务连接
        /// </summary>
        /// <param name="mainfrm"></param>
        public static IWCFHandlerService CreateConnection(IClientService client)
        {
            try
            {
                //NetTcpBinding binding = new NetTcpBinding("NetTcpBinding_WCFHandlerService");
                mChannelFactory = new DuplexChannelFactory<IWCFHandlerService>(client, "myendpoint");
                IWCFHandlerService wcfHandlerService = mChannelFactory.CreateChannel();
                
                string routerID;
                string mProxyID;
                using (var scope = new OperationContextScope(wcfHandlerService as IContextChannel))
                {
                    // 注意namespace必须和ServiceContract中定义的namespace保持一致，默认是：http://tempuri.org   
                    routerID = Guid.NewGuid().ToString();
                    var router = System.ServiceModel.Channels.MessageHeader.CreateHeader("routerID", myNamespace, routerID);
                    OperationContext.Current.OutgoingMessageHeaders.Add(router);
                    mProxyID = wcfHandlerService.CreateDomain(getLocalIPAddress());

                    if (WcfClientManage.ServerConfigRequestState == false)
                    {
                        //重新获取服务端配置，如：是否压缩Json、是否加密Json
                        string serverConfig = wcfHandlerService.ServerConfig();
                        WcfClientManage.IsHeartbeat = serverConfig.Split(new char[] { '#' })[0] == "1" ? true : false;
                        WcfClientManage.HeartbeatTime = Convert.ToInt32(serverConfig.Split(new char[] { '#' })[1]);
                        WcfClientManage.IsMessage = serverConfig.Split(new char[] { '#' })[2] == "1" ? true : false;
                        WcfClientManage.MessageTime = Convert.ToInt32(serverConfig.Split(new char[] { '#' })[3]);
                        WcfClientManage.IsCompressJson = serverConfig.Split(new char[] { '#' })[4] == "1" ? true : false;
                        WcfClientManage.IsEncryptionJson = serverConfig.Split(new char[] { '#' })[5] == "1" ? true : false;

                        if (WcfClientManage.IsHeartbeat)
                        {
                            //开启发送心跳
                            if (timer == null)
                                StartTimer();
                            else
                                timer.Start();
                        }
                        else
                        {
                            if (timer != null)
                                timer.Stop();
                        }

                        WcfClientManage.ServerConfigRequestState = true;
                    }
                }


                if (AppGlobal.cache.Contains("WCFClientID")) AppGlobal.cache.Remove("WCFClientID");
                if (AppGlobal.cache.Contains("WCFService")) AppGlobal.cache.Remove("WCFService");
                if (AppGlobal.cache.Contains("ClientService")) AppGlobal.cache.Remove("ClientService");
                if (AppGlobal.cache.Contains("routerID")) AppGlobal.cache.Remove("routerID");

                AppGlobal.cache.Add("routerID", routerID);
                AppGlobal.cache.Add("WCFClientID", mProxyID);
                AppGlobal.cache.Add("WCFService", wcfHandlerService);
                AppGlobal.cache.Add("ClientService", client);

                return wcfHandlerService;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        /// <summary>
        /// 重新连接wcf服务，服务端存在ClientID
        /// </summary>
        /// <param name="mainfrm"></param>
        public static void ReConnection(bool isRequest)
        {
            try
            {
                IWCFHandlerService wcfHandlerService = mChannelFactory.CreateChannel();

                if (AppGlobal.cache.Contains("WCFService")) AppGlobal.cache.Remove("WCFService");
                AppGlobal.cache.Add("WCFService", wcfHandlerService);

                if (isRequest==true)//避免死循环
                    Heartbeat();//重连之后必须再次调用心跳
            }
            catch
            {
                //throw new Exception(err.Message);
            }
        }
        /// <summary>
        /// 发送心跳
        /// </summary>
        /// <returns></returns>
        public static bool Heartbeat()
        {
            try
            {
                bool ret = false;
                IWCFHandlerService _wcfService = AppGlobal.cache.GetData("WCFService") as IWCFHandlerService;
                using (var scope = new OperationContextScope(_wcfService as IContextChannel))
                {
                    var router = System.ServiceModel.Channels.MessageHeader.CreateHeader("routerID", myNamespace, AppGlobal.cache.GetData("routerID").ToString());
                    OperationContext.Current.OutgoingMessageHeaders.Add(router);
                    ret = _wcfService.Heartbeat(AppGlobal.cache.GetData("WCFClientID").ToString());

                    if (WcfClientManage.ServerConfigRequestState == false)
                    {
                        //重新获取服务端配置，如：是否压缩Json、是否加密Json
                        string serverConfig = _wcfService.ServerConfig();
                        WcfClientManage.IsHeartbeat = serverConfig.Split(new char[] { '#' })[0] == "1" ? true : false;
                        WcfClientManage.HeartbeatTime = Convert.ToInt32(serverConfig.Split(new char[] { '#' })[1]);
                        WcfClientManage.IsMessage = serverConfig.Split(new char[] { '#' })[2] == "1" ? true : false;
                        WcfClientManage.MessageTime = Convert.ToInt32(serverConfig.Split(new char[] { '#' })[3]);
                        WcfClientManage.IsCompressJson = serverConfig.Split(new char[] { '#' })[4] == "1" ? true : false;
                        WcfClientManage.IsEncryptionJson = serverConfig.Split(new char[] { '#' })[5] == "1" ? true : false;

                        if (WcfClientManage.IsHeartbeat)
                        {
                            //开启发送心跳
                            if (timer == null)
                                StartTimer();
                            else
                                timer.Start();
                        }
                        else
                        {
                            if (timer != null)
                                timer.Stop();
                        }
                        WcfClientManage.ServerConfigRequestState = true;
                    }
                }

                if (ret == false)//表示服务主机关闭过，丢失了clientId，必须重新创建连接
                {
                    mChannelFactory.Abort();//关闭原来通道
                    CreateConnection(AppGlobal.cache.GetData("ClientService") as IClientService);
                }
                return ret;
            }
            catch
            {
                WcfClientManage.ServerConfigRequestState = false;
                ReConnection(false);//连接服务主机失败，重连
                return false;
            }
        }

        /// <summary>
        /// 向服务发送请求
        /// </summary>
        /// <param name="controller">控制器名称</param>
        /// <param name="method">方法名称</param>
        /// <param name="jsondata">数据</param>
        /// <returns>返回Json数据</returns>
        public static string Request(string controller, string method, string jsondata)
        {
            try
            {
                if (WcfClientManage.IsCompressJson)//开启压缩
                {
                    jsondata = ZipComporessor.Compress(jsondata);//压缩传入参数
                    //jsondata = JsonComporessor.Compress(jsondata);//压缩传入参数
                }

                IWCFHandlerService _wcfService = AppGlobal.cache.GetData("WCFService") as IWCFHandlerService;
                string retJson;
                using (var scope = new OperationContextScope(_wcfService as IContextChannel))
                {
                    var router = System.ServiceModel.Channels.MessageHeader.CreateHeader("routerID", myNamespace, AppGlobal.cache.GetData("routerID").ToString());
                    OperationContext.Current.OutgoingMessageHeaders.Add(router);
                    retJson = _wcfService.ProcessRequest(AppGlobal.cache.GetData("WCFClientID").ToString(), controller, method, jsondata);
                }

                if (WcfClientManage.IsCompressJson)
                {
                    retJson = ZipComporessor.Decompress(retJson);
                    //retJson = JsonComporessor.Decompress(retJson);
                }

                if (WcfClientManage.IsHeartbeat == false)//如果没有启动心跳，则请求发送心跳
                {
                    WcfClientManage.ServerConfigRequestState = false;
                    Heartbeat();
                }

                return retJson;
            }
            catch (Exception e)
            {
                WcfClientManage.ServerConfigRequestState = false;
                ReConnection(true);//连接服务主机失败，重连
                throw new Exception(e.Message + "\n连接服务主机失败，请联系管理员！");
            }
        }

        /// <summary>
        /// 向服务发送异步请求
        /// 客户端建议不要用多线程，都采用异步请求方式
        /// </summary>
        /// <param name="controller">控制器名称</param>
        /// <param name="method">方法名称</param>
        /// <param name="jsondata">数据</param>
        /// <returns>返回Json数据</returns>
        public static IAsyncResult RequestAsync(string controller, string method, string jsondata, Action<string> action)
        {
            try
            {
                if (WcfClientManage.IsCompressJson)//开启压缩
                {
                    jsondata = ZipComporessor.Compress(jsondata);//压缩传入参数
                    //jsondata = JsonComporessor.Compress(jsondata);//压缩传入参数
                }

                IWCFHandlerService _wcfService = AppGlobal.cache.GetData("WCFService") as IWCFHandlerService;
                //string retJson;
                IAsyncResult result = null;
                using (var scope = new OperationContextScope(_wcfService as IContextChannel))
                {
                    var router = System.ServiceModel.Channels.MessageHeader.CreateHeader("routerID", myNamespace, AppGlobal.cache.GetData("routerID").ToString());
                    OperationContext.Current.OutgoingMessageHeaders.Add(router);

                    AsyncCallback callback = delegate(IAsyncResult r)
                    {
                        string retJson = _wcfService.EndProcessRequest(r);

                        if (WcfClientManage.IsCompressJson)
                        {
                            retJson = ZipComporessor.Decompress(retJson);
                            //retJson = JsonComporessor.Decompress(retJson);
                        }

                        action(retJson);
                    };
                    result = _wcfService.BeginProcessRequest(AppGlobal.cache.GetData("WCFClientID").ToString(), controller, method, jsondata, callback, null);
                }

                if (WcfClientManage.IsHeartbeat == false)//如果没有启动心跳，则请求发送心跳
                {
                    WcfClientManage.ServerConfigRequestState = false;
                    Heartbeat();
                }

                //return retJson;
                return result;
            }
            catch (Exception e)
            {
                WcfClientManage.ServerConfigRequestState = false;
                ReConnection(true);//连接服务主机失败，重连
                throw new Exception(e.Message + "\n连接服务主机失败，请联系管理员！");
            }
        }


        /// <summary>
        /// 卸载连接
        /// </summary>
        public static void UnConnection()
        {
            if (AppGlobal.cache.GetData("WCFClientID") == null) return;

            //bool b = false;
            string mClientID = AppGlobal.cache.GetData("WCFClientID").ToString();
            IWCFHandlerService mWcfService = AppGlobal.cache.GetData("WCFService") as IWCFHandlerService;
            if (mClientID != null)
            {
                using (var scope = new OperationContextScope(mWcfService as IContextChannel))
                {
                    try
                    {
                        var router = System.ServiceModel.Channels.MessageHeader.CreateHeader("routerID", "http://www.efwplus.cn/", AppGlobal.cache.GetData("routerID").ToString());
                        OperationContext.Current.OutgoingMessageHeaders.Add(router);
                        var cmd = System.ServiceModel.Channels.MessageHeader.CreateHeader("CMD", "http://www.efwplus.cn/", "Quit");
                        OperationContext.Current.OutgoingMessageHeaders.Add(cmd);
                        mWcfService.UnDomain(mClientID);

                        mChannelFactory.Close();//关闭通道

                        if (timer != null)//关闭连接必须停止心跳
                            timer.Stop();
                    }
                    catch
                    {
                        if (mChannelFactory != null)
                            mChannelFactory.Abort();
                    }
                }
            }
        }
        /// <summary>
        /// 广播消息接收(暂无用)
        /// </summary>
        /// <param name="jsondata"></param>
        public static void ReplyClient(string jsondata)
        {

        }


        public static List<dwPlugin> GetWcfServicesAllInfo()
        {
            IWCFHandlerService _wcfService = AppGlobal.cache.GetData("WCFService") as IWCFHandlerService;
            using (var scope = new OperationContextScope(_wcfService as IContextChannel))
            {
                var router = System.ServiceModel.Channels.MessageHeader.CreateHeader("routerID", myNamespace, AppGlobal.cache.GetData("routerID").ToString());
                OperationContext.Current.OutgoingMessageHeaders.Add(router);
                string ret = _wcfService.WcfServicesAllInfo();
                return JsonConvert.DeserializeObject<List<dwPlugin>>(ret);
            }
        }


        

        //向服务端发送心跳，间隔时间为5s
        static System.Timers.Timer timer;
        static void StartTimer()
        {
            timer = new System.Timers.Timer();
            timer.Interval = WcfClientManage.HeartbeatTime*5*1000;//客户端比服务端心跳间隔多5倍
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }
        static Object syncObj = new Object();////定义一个静态对象用于线程部份代码块的锁定，用于lock操作
        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (syncObj)
            {
                try
                {
                    Heartbeat();
                }
                catch
                {
                    //throw new Exception(err.Message);
                }
            }
        }
        static string getLocalIPAddress()
        {
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = "";
            foreach (IPAddress ip in IpEntry.AddressList)
            {
                if (Regex.IsMatch(ip.ToString(), @"\d{0,3}\.\d{0,3}\.\d{0,3}\.\d{0,3}"))
                {
                    myip = ip.ToString();
                    break;
                }
            }
            return myip;
        }

        #endregion

        #region 上传下载文件
        /// <summary>
        /// 上传文件，上传文件的进度只能通过时时获取服务端Read的进度
        /// </summary>
        /// <param name="filepath">文件本地路径</param>
        /// <returns>上传成功后返回的文件名</returns>
        public static string UpLoadFile(string filepath)
        {
            return UpLoadFile(filepath, null);
        }
        /// <summary>
        /// 上传文件，上传文件的进度只能通过时时获取服务端Read的进度
        /// </summary>
        /// <param name="filepath">文件本地路径</param>
        /// <returns>上传成功后返回的文件名</returns>
        public static string UpLoadFile(string filepath, Action<int> action)
        {
            ChannelFactory<IFileTransfer> mfileChannelFactory = null;
            IFileTransfer fileHandlerService = null;
            try
            {
                FileInfo finfo = new FileInfo(filepath);
                if (finfo.Exists == false)
                    throw new Exception("文件不存在！");

                mfileChannelFactory = new ChannelFactory<IFileTransfer>("fileendpoint");
                fileHandlerService = mfileChannelFactory.CreateChannel();

                UpFile uf = new UpFile();
                if (AppGlobal.cache.Contains("WCFClientID"))
                    uf.clientId = AppGlobal.cache.GetData("WCFClientID").ToString();
                uf.UpKey = Guid.NewGuid().ToString();
                uf.FileExt = finfo.Extension;
                uf.FileName = finfo.Name;
                uf.FileSize = finfo.Length;
                uf.FileStream = finfo.OpenRead();

                if (action != null)
                    getUpLoadFileProgress(uf.UpKey, action);//获取上传进度条

                UpFileResult result = fileHandlerService.UpLoadFile(uf);

                //mfileChannelFactory.Close();//关闭会话

                if (result.IsSuccess)
                    return result.Message;
                else
                    throw new Exception("上传文件失败！");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n上传文件失败！");
            }
            finally
            {
                if (fileHandlerService != null)
                    (fileHandlerService as IContextChannel).Close();
                if (mfileChannelFactory != null)
                    mfileChannelFactory.Close();
            }
        }
        /// <summary>
        /// 下载文件，下载文件进度在Read的时候可以显示
        /// </summary>
        /// <param name="filename">下载文件名</param>
        /// <returns>下载成功后返回存储在本地文件路径</returns>
        public static string DownLoadFile(string filename)
        {
            return DownLoadFile(filename, null);
        }
        /// <summary>
        /// 下载文件，下载文件进度在Read的时候可以显示
        /// </summary>
        /// <param name="filename">下载文件名</param>
        /// <param name="action">进度0-100</param>
        /// <returns>下载成功后返回存储在本地文件路径</returns>
        public static string DownLoadFile(string filename, Action<int> action)
        {
            ChannelFactory<IFileTransfer> mfileChannelFactory = null;
            IFileTransfer fileHandlerService = null;
            try
            {
                if (string.IsNullOrEmpty(filename))
                    throw new Exception("文件名不为空！");

                string filebufferpath = AppGlobal.AppRootPath + @"filebuffer\";
                if (!Directory.Exists(filebufferpath))
                {
                    Directory.CreateDirectory(filebufferpath);
                }

                mfileChannelFactory = new ChannelFactory<IFileTransfer>("fileendpoint");
                fileHandlerService = mfileChannelFactory.CreateChannel();
                DownFile df = new DownFile();
                if (AppGlobal.cache.Contains("WCFClientID"))
                    df.clientId = AppGlobal.cache.GetData("WCFClientID").ToString();
                df.DownKey = Guid.NewGuid().ToString();
                df.FileName = filename;


                DownFileResult result = fileHandlerService.DownLoadFile(df);
                //mfileChannelFactory.Close();//关闭会话
                if (result.IsSuccess)
                {
                    string filepath = filebufferpath + filename;
                    if (File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }

                    FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write);

                    int oldprogressnum = 0;
                    decimal progressnum = 0;
                    long bufferlen = 4096;
                    int count = 0;
                    byte[] buffer = new byte[bufferlen];

                    //设置服务端的下载进度
                    setDownFileProgress(df.clientId, df.DownKey, (delegate()
                    {
                        return Convert.ToInt32(Math.Ceiling(progressnum));
                    }));

                    while ((count = result.FileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, count);

                        //获取下载进度
                        getprogress(result.FileSize, bufferlen, ref progressnum);
                        if (oldprogressnum < Convert.ToInt32(Math.Ceiling(progressnum)))
                        {
                            oldprogressnum = Convert.ToInt32(Math.Ceiling(progressnum));
                            //setDownFileProgress(df.clientId, df.DownKey, oldprogressnum);//设置服务端的下载进度
                            if (action != null)
                            {
                                action(Convert.ToInt32(Math.Ceiling(progressnum)));
                            }
                        }
                    }
                    //清空缓冲区
                    fs.Flush();
                    //关闭流
                    fs.Close();

                    //System.Threading.Thread.Sleep(200);
                    return filepath;
                }
                else
                    throw new Exception("下载文件失败！");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n下载文件失败！");
            }
            finally
            {
                if (fileHandlerService != null)
                    (fileHandlerService as IContextChannel).Close();
                if (mfileChannelFactory != null)
                    mfileChannelFactory.Close();
            }
        }

        static void getprogress(long filesize, long bufferlen, ref decimal progressnum)
        {
            decimal percent = Convert.ToDecimal(100 / Convert.ToDecimal(filesize / bufferlen));
            progressnum = progressnum + percent > 100 ? 100 : progressnum + percent;
        }
        static void getUpLoadFileProgress(string upkey, Action<int> action)
        {
            new Action<string, Action<int>>(delegate(string _upkey, Action<int> _action)
            {
                ChannelFactory<IFileTransfer> mfileChannelFactory = null;
                IFileTransfer fileHandlerService = null;
                try
                {

                    mfileChannelFactory = new ChannelFactory<IFileTransfer>("fileendpoint");
                    fileHandlerService = mfileChannelFactory.CreateChannel();

                    int oldnum = 0;
                    int num = 0;
                    while ((num = fileHandlerService.GetUpLoadFileProgress(_upkey)) != 100)
                    {
                        if (oldnum < num)
                        {
                            oldnum = num;
                            action(num);
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                    action(100);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + "\n获取上传文件进度失败！");
                }
                finally
                {
                    if (fileHandlerService != null)
                        (fileHandlerService as IContextChannel).Close();
                    if (mfileChannelFactory != null)
                        mfileChannelFactory.Close();
                }

            }).BeginInvoke(upkey, action, null, null);
        }
        static void setDownFileProgress(string clientId, string downkey, Func<int> func)
        {
            //string _clientId = clientId;
            //string _downkey = downkey;
            //int _progressnum = progressnum;
            new Action<string, string, Func<int>>(delegate(string _clientId, string _downkey, Func<int> _func)
            {
                ChannelFactory<IFileTransfer> mfileChannelFactory = null;
                IFileTransfer fileHandlerService = null;
                try
                {
                    mfileChannelFactory = new ChannelFactory<IFileTransfer>("fileendpoint");
                    fileHandlerService = mfileChannelFactory.CreateChannel();

                    int _oldprogressnum = 0;
                    int _progressnum = 0;
                    while ((_progressnum = _func()) != 100)
                    {
                        if (_oldprogressnum < _progressnum)
                        {
                            _oldprogressnum = _progressnum;
                            fileHandlerService.SetDownLoadFileProgress(_clientId, _downkey, _progressnum);
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                    fileHandlerService.SetDownLoadFileProgress(_clientId, _downkey, 100);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + "\n设置下载文件进度失败！");
                }
                finally
                {
                    if (fileHandlerService != null)
                        (fileHandlerService as IContextChannel).Close();
                    if (mfileChannelFactory != null)
                        mfileChannelFactory.Close();
                }
            }).BeginInvoke(clientId, downkey, func, null, null);
        }
        #endregion
    }
}
