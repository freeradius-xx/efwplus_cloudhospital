using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EFWCoreLib.CoreFrame.Common;
using EFWCoreLib.CoreFrame.Init;
using EFWCoreLib.WcfFrame.ClientController;
using EFWCoreLib.WcfFrame.ServerController;
using EFWCoreLib.WcfFrame.WcfService.Contract;
using Newtonsoft.Json;

namespace EFWCoreLib.WcfFrame
{
    /// <summary>
    /// 客户端连接WCF服务，一个对象一个会话通道
    /// </summary>
    public class ClientLink : IDisposable
    {
        public CHDEPConnection mConn;

        private readonly string myNamespace = "http://www.efwplus.cn/";
        private DuplexChannelFactory<IWCFHandlerService> mChannelFactory;
        private ChannelFactory<IFileTransfer> mfileChannelFactory = null;
        private Action<bool, int> backConfig = null;

        public ClientLink(string pluginname)
        {
            InitComm(null, pluginname);
        }
        
        public ClientLink(string clientname,string pluginname)
        {
            InitComm(clientname,pluginname);
        }

        public ClientLink(string clientname, string pluginname, Action<bool, int> actionConfig)
        {
            backConfig = actionConfig;
            InitComm(clientname, pluginname);
        }


        private void InitComm(string clientname, string pluginname)
        {
            if (string.IsNullOrEmpty(clientname))
                clientname = getLocalIPAddress();

            mConn = new CHDEPConnection();
            mConn.ClientName = clientname;
            mConn.RouterID = Guid.NewGuid().ToString();
            mConn.PluginName = pluginname;
            mConn.ClientService = new ReplyClientCallBack();

            if (mChannelFactory == null)
                mChannelFactory = new DuplexChannelFactory<IWCFHandlerService>(mConn.ClientService, "wcfendpoint");
            if (mfileChannelFactory == null)
                mfileChannelFactory = new ChannelFactory<IFileTransfer>("fileendpoint");
        }

        #region IDisposable 成员

        ~ClientLink()
        {
            Dispose();
        } 

        public void Dispose()
        {
            UnConnection();

            try
            {
                if (mChannelFactory != null)
                    mChannelFactory.Close();
                if (mfileChannelFactory != null)
                    mfileChannelFactory.Close();
            }
            catch
            {
                if (mChannelFactory != null)
                    mChannelFactory.Abort();
                if (mfileChannelFactory != null)
                    mfileChannelFactory.Abort();
            }
        }

        #endregion

        #region 数据交互

        private bool IsHeartbeat = false;
        private int HeartbeatTime = 1;//默认间隔1秒,客户端5倍
        private bool IsMessage = false;
        private int MessageTime = 1;//默认间隔1秒
        private bool IsCompressJson = false;//是否压缩Json数据
        private bool IsEncryptionJson = false;//是否加密Json数据
        private bool ServerConfigRequestState = false;//获取服务端配置读取状态

        /// <summary>
        /// 创建连接
        /// </summary>
        public void CreateConnection()
        {
            IWCFHandlerService wcfHandlerService = mChannelFactory.CreateChannel();
            mConn.WcfService = wcfHandlerService;
            string serverConfig = null;

            AddMessageHeader(wcfHandlerService as IContextChannel, "", "wcfservice", (() =>
            {

                mConn.ClientID = wcfHandlerService.CreateDomain(mConn.ClientName);//创建连接获取ClientID
                if (ServerConfigRequestState == false)
                {
                    //重新获取服务端配置，如：是否压缩Json、是否加密Json
                    serverConfig = wcfHandlerService.ServerConfig();
                    ServerConfigRequestState = true;
                }
            }));

            if (!string.IsNullOrEmpty(serverConfig))
            {
                IsHeartbeat = serverConfig.Split(new char[] { '#' })[0] == "1" ? true : false;
                HeartbeatTime = Convert.ToInt32(serverConfig.Split(new char[] { '#' })[1]);
                IsMessage = serverConfig.Split(new char[] { '#' })[2] == "1" ? true : false;
                MessageTime = Convert.ToInt32(serverConfig.Split(new char[] { '#' })[3]);
                IsCompressJson = serverConfig.Split(new char[] { '#' })[4] == "1" ? true : false;
                IsEncryptionJson = serverConfig.Split(new char[] { '#' })[5] == "1" ? true : false;

                if (backConfig != null)
                    backConfig(IsMessage, MessageTime);

                if (IsHeartbeat)
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
            }
        }

        /// <summary>
        /// 向服务发送请求
        /// </summary>
        /// <param name="controller">插件名@控制器名称</param>
        /// <param name="method">方法名称</param>
        /// <param name="jsondata">数据</param>
        /// <returns>返回Json数据</returns>
        public string Request(string controller, string method, string jsondata)
        {
            if (mConn == null) throw new Exception("还没有创建连接！");
            try
            {
                if (IsCompressJson)//开启压缩
                {
                    jsondata = ZipComporessor.Compress(jsondata);//压缩传入参数
                    //jsondata = JsonComporessor.Compress(jsondata);//压缩传入参数
                }

                IWCFHandlerService _wcfService = mConn.WcfService;
                string retJson = "";

                AddMessageHeader(_wcfService as IContextChannel, "", "wcfservice", (() =>
                {
                    retJson = _wcfService.ProcessRequest(mConn.ClientID, controller, method, jsondata);
                }));

                if (IsCompressJson)
                {
                    retJson = ZipComporessor.Decompress(retJson);
                    //retJson = JsonComporessor.Decompress(retJson);
                }

                if (IsHeartbeat == false)//如果没有启动心跳，则请求发送心跳
                {
                    ServerConfigRequestState = false;
                    Heartbeat();
                }

                return retJson;
            }
            catch (Exception e)
            {
                ServerConfigRequestState = false;
                ReConnection(true);//连接服务主机失败，重连
                throw new Exception(e.Message + "\n连接服务主机失败，请联系管理员！");
            }
        }

        /// <summary>
        /// 向服务发送异步请求
        /// 客户端建议不要用多线程，都采用异步请求方式
        /// </summary>
        /// <param name="controller">插件名@控制器名称</param>
        /// <param name="method">方法名称</param>
        /// <param name="jsondata">数据</param>
        /// <returns>返回Json数据</returns>
        public IAsyncResult RequestAsync(string controller, string method, string jsondata, Action<string> action)
        {
            if (mConn == null) throw new Exception("还没有创建连接！");
            try
            {
                if (IsCompressJson)//开启压缩
                {
                    jsondata = ZipComporessor.Compress(jsondata);//压缩传入参数
                    //jsondata = JsonComporessor.Compress(jsondata);//压缩传入参数
                }

                IWCFHandlerService _wcfService = mConn.WcfService;
                IAsyncResult result = null;

                AddMessageHeader(_wcfService as IContextChannel, "", "wcfservice", (() =>
                {
                    AsyncCallback callback = delegate(IAsyncResult r)
                    {
                        string retJson = _wcfService.EndProcessRequest(r);

                        if (IsCompressJson)
                        {
                            retJson = ZipComporessor.Decompress(retJson);
                            //retJson = JsonComporessor.Decompress(retJson);
                        }

                        action(retJson);
                    };
                    result = _wcfService.BeginProcessRequest(mConn.ClientID, controller, method, jsondata, callback, null);
                }));

                if (IsHeartbeat == false)//如果没有启动心跳，则请求发送心跳
                {
                    ServerConfigRequestState = false;
                    Heartbeat();
                }

                //return retJson;
                return result;
            }
            catch (Exception e)
            {
                ServerConfigRequestState = false;
                ReConnection(true);//连接服务主机失败，重连
                throw new Exception(e.Message + "\n连接服务主机失败，请联系管理员！");
            }
        }

        /// <summary>
        /// 卸载连接
        /// </summary>
        public void UnConnection()
        {
            if (mConn == null) return;
            string mClientID = mConn.ClientID;
            IWCFHandlerService mWcfService = mConn.WcfService;
            if (mClientID != null)
            {
                AddMessageHeader(mWcfService as IContextChannel, "Quit", "wcfservice", (() =>
                {
                    mWcfService.UnDomain(mClientID);
                }));

                try
                {
                    //mChannelFactory.Close();//关闭通道
                    (mWcfService as IContextChannel).Close();

                    if (timer != null)//关闭连接必须停止心跳
                        timer.Stop();
                }
                catch
                {
                    if ((mWcfService as IContextChannel) != null)
                        (mWcfService as IContextChannel).Abort();
                }

                mConn = null;
            }
        }

        /// <summary>
        /// 重新连接wcf服务，服务端存在ClientID
        /// </summary>
        /// <param name="mainfrm"></param>
        private void ReConnection(bool isRequest)
        {
            try
            {
                IWCFHandlerService wcfHandlerService = mChannelFactory.CreateChannel();
                mConn.WcfService = wcfHandlerService;
                if (isRequest == true)//避免死循环
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
        private bool Heartbeat()
        {
            IWCFHandlerService _wcfService = mConn.WcfService;
            try
            {
                bool ret = false;
                string serverConfig = null;
                AddMessageHeader(_wcfService as IContextChannel, "", "wcfservice", (() =>
                {
                    ret = _wcfService.Heartbeat(mConn.ClientID);
                    if (ServerConfigRequestState == false)
                    {
                        //重新获取服务端配置，如：是否压缩Json、是否加密Json
                        serverConfig = _wcfService.ServerConfig();
                        ServerConfigRequestState = true;
                    }
                }));

                if (!string.IsNullOrEmpty(serverConfig))
                {
                    IsHeartbeat = serverConfig.Split(new char[] { '#' })[0] == "1" ? true : false;
                    HeartbeatTime = Convert.ToInt32(serverConfig.Split(new char[] { '#' })[1]);
                    IsMessage = serverConfig.Split(new char[] { '#' })[2] == "1" ? true : false;
                    MessageTime = Convert.ToInt32(serverConfig.Split(new char[] { '#' })[3]);
                    IsCompressJson = serverConfig.Split(new char[] { '#' })[4] == "1" ? true : false;
                    IsEncryptionJson = serverConfig.Split(new char[] { '#' })[5] == "1" ? true : false;

                    if (backConfig != null)
                        backConfig(IsMessage, MessageTime);

                    if (IsHeartbeat)
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
                }

                if (ret == false)//表示服务主机关闭过，丢失了clientId，必须重新创建连接
                {
                    //mChannelFactory.Abort();//关闭原来通道
                    (_wcfService as IContextChannel).Abort();
                    CreateConnection();
                }
                return ret;
            }
            catch
            {
                ServerConfigRequestState = false;
                ReConnection(false);//连接服务主机失败，重连
                return false;
            }
        }


        public List<dwPlugin> GetWcfServicesAllInfo()
        {
            IWCFHandlerService _wcfService = mConn.WcfService;
            List<dwPlugin> list = new List<dwPlugin>();
            AddMessageHeader(_wcfService as IContextChannel, "", "wcfservice", (() =>
            {
                string ret = _wcfService.WcfServicesAllInfo();
                list = JsonConvert.DeserializeObject<List<dwPlugin>>(ret);
            }));

            return list;
        }

        private string getLocalIPAddress()
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

        //向服务端发送心跳，间隔时间为5s
        System.Timers.Timer timer;
        void StartTimer()
        {
            timer = new System.Timers.Timer();
            timer.Interval = HeartbeatTime * 5 * 1000;//客户端比服务端心跳间隔多5倍
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }
        Object syncObj = new Object();////定义一个静态对象用于线程部份代码块的锁定，用于lock操作
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
        #endregion

        #region 上传下载文件
        /// <summary>
        /// 上传文件，上传文件的进度只能通过时时获取服务端Read的进度
        /// </summary>
        /// <param name="filepath">文件本地路径</param>
        /// <returns>上传成功后返回的文件名</returns>
        public string UpLoadFile(string filepath)
        {
            return UpLoadFile(filepath, null);
        }
        /// <summary>
        /// 上传文件，上传文件的进度只能通过时时获取服务端Read的进度
        /// </summary>
        /// <param name="filepath">文件本地路径</param>
        /// <returns>上传成功后返回的文件名</returns>
        public string UpLoadFile(string filepath, Action<int> action)
        {
            
            IFileTransfer fileHandlerService = null;
            try
            {
                FileInfo finfo = new FileInfo(filepath);
                if (finfo.Exists == false)
                    throw new Exception("文件不存在！");


                fileHandlerService = mfileChannelFactory.CreateChannel();

                UpFile uf = new UpFile();
                uf.clientId = mConn == null ? "" : mConn.ClientID;
                uf.UpKey = Guid.NewGuid().ToString();
                uf.FileExt = finfo.Extension;
                uf.FileName = finfo.Name;
                uf.FileSize = finfo.Length;
                uf.FileStream = finfo.OpenRead();

                if (action != null)
                    getUpLoadFileProgress(uf.UpKey, action);//获取上传进度条

                UpFileResult result = new UpFileResult();

                //AddMessageHeader(fileHandlerService as IContextChannel, "", "fileservice", (() =>
                //{
                    result = fileHandlerService.UpLoadFile(uf);
                //}));

                

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
                {
                    (fileHandlerService as IContextChannel).Close();
                }
            }
        }
        /// <summary>
        /// 下载文件，下载文件进度在Read的时候可以显示
        /// </summary>
        /// <param name="filename">下载文件名</param>
        /// <returns>下载成功后返回存储在本地文件路径</returns>
        public string DownLoadFile(string filename)
        {
            return DownLoadFile(filename, null);
        }
        /// <summary>
        /// 下载文件，下载文件进度在Read的时候可以显示
        /// </summary>
        /// <param name="filename">下载文件名</param>
        /// <param name="action">进度0-100</param>
        /// <returns>下载成功后返回存储在本地文件路径</returns>
        public string DownLoadFile(string filename, Action<int> action)
        {
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

                fileHandlerService = mfileChannelFactory.CreateChannel();
                DownFile df = new DownFile();
                df.clientId = mConn == null ? "" : mConn.ClientID;
                df.DownKey = Guid.NewGuid().ToString();
                df.FileName = filename;


                DownFileResult result = new DownFileResult();

                //AddMessageHeader(fileHandlerService as IContextChannel, "", "fileservice", (() =>
                //{
                    result = fileHandlerService.DownLoadFile(df);
                //}));

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
                    int progressnum = 0;
                    int bufferlen = 4096;
                    int count = 0;
                    long readnum = 0;
                    byte[] buffer = new byte[bufferlen];

                    //设置服务端的下载进度
                    setDownFileProgress(df.clientId, df.DownKey, (delegate()
                    {
                        return progressnum;
                    }));

                    while ((count = result.FileStream.Read(buffer, 0, bufferlen)) > 0)
                    {
                        fs.Write(buffer, 0, count);
                        readnum += count;
                        //获取下载进度
                        getprogress(result.FileSize, readnum, ref progressnum);
                        if (oldprogressnum < progressnum)
                        {
                            oldprogressnum = progressnum;
                            //setDownFileProgress(df.clientId, df.DownKey, oldprogressnum);//设置服务端的下载进度
                            if (action != null)
                            {
                                action(progressnum);
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
            }
        }

        private void getprogress(long filesize, long readnum, ref int progressnum)
        {
            //decimal percent = Convert.ToDecimal(100 / Convert.ToDecimal(filesize / bufferlen));
            //progressnum = progressnum + percent > 100 ? 100 : progressnum + percent;
            decimal percent = Convert.ToDecimal(readnum) / Convert.ToDecimal(filesize) * 100;
            progressnum = Convert.ToInt32(Math.Ceiling(percent));
        }
        void getUpLoadFileProgress(string upkey, Action<int> action)
        {
            new Action<string, Action<int>>(delegate(string _upkey, Action<int> _action)
            {
                IFileTransfer fileHandlerService = null;
                try
                {

                    fileHandlerService = mfileChannelFactory.CreateChannel();

                    int oldnum = 0;
                    int num = 0;
                    //AddMessageHeader(fileHandlerService as IContextChannel, "", "fileservice", (() =>
                    //{
                    while ((num = fileHandlerService.GetUpLoadFileProgress(_upkey)) != 100)
                    {
                        if (oldnum < num)
                        {
                            oldnum = num;
                            _action(num);
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                    //}));


                    _action(100);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + "\n获取上传文件进度失败！");
                }
                finally
                {
                    if (fileHandlerService != null)
                        (fileHandlerService as IContextChannel).Close();
                }

            }).BeginInvoke(upkey, action, null, null);
        }
        void setDownFileProgress(string clientId, string downkey, Func<int> func)
        {
            //string _clientId = clientId;
            //string _downkey = downkey;
            //int _progressnum = progressnum;
            new Action<string, string, Func<int>>(delegate(string _clientId, string _downkey, Func<int> _func)
            {
                IFileTransfer fileHandlerService = null;
                try
                {
                    fileHandlerService = mfileChannelFactory.CreateChannel();

                    //AddMessageHeader(fileHandlerService as IContextChannel, "", "fileservice", (() =>
                    //{
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
                    //}));
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + "\n设置下载文件进度失败！");
                }
                finally
                {
                    if (fileHandlerService != null)
                        (fileHandlerService as IContextChannel).Close();
 
                }
            }).BeginInvoke(clientId, downkey, func, null, null);
        }
        #endregion

        #region 超级回调

        public void RegisterReplyPlugin(string serverHostName, string[] plugin)
        {
            if (mConn == null) throw new Exception("还没有创建连接！");
            try
            {
                IWCFHandlerService _wcfService = mConn.WcfService;
                _wcfService.RegisterReplyPlugin(serverHostName, plugin);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n连接服务主机失败，请联系管理员！");
            }
        }
        #endregion

        private void AddMessageHeader(IContextChannel channel, string cmd,string stype,Action callback)
        {
            using (var scope = new OperationContextScope(channel as IContextChannel))
            {
                var CMD = System.ServiceModel.Channels.MessageHeader.CreateHeader("CMD", myNamespace, cmd);
                OperationContext.Current.OutgoingMessageHeaders.Add(CMD);
                var router = System.ServiceModel.Channels.MessageHeader.CreateHeader("routerID", myNamespace, mConn.RouterID);
                OperationContext.Current.OutgoingMessageHeaders.Add(router);
                var plugin = System.ServiceModel.Channels.MessageHeader.CreateHeader("Plugin", myNamespace, mConn.PluginName);
                OperationContext.Current.OutgoingMessageHeaders.Add(plugin);
                if (string.IsNullOrEmpty(stype))
                    stype = "wcfservice";
                var servicetype = System.ServiceModel.Channels.MessageHeader.CreateHeader("ServiceType", myNamespace, stype);
                OperationContext.Current.OutgoingMessageHeaders.Add(servicetype);
                callback();
            }
        }
    }

    public class CHDEPConnection
    {
        public IWCFHandlerService WcfService { get; set; }
        public IClientService ClientService { get; set; }
        public string ClientID { get; set; }//服务端返回
        public string ClientName { get; set; }
        public string RouterID { get; set; }
        public string PluginName { get; set; }
    }
}
