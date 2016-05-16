using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using EFWCoreLib.WcfFrame.WcfService.Contract;
using EFWCoreLib.WcfFrame.ServerController;
using System.Threading;

namespace EFWCoreLib.WcfFrame.WcfService
{
    //InstanceContextMode.PerSession  为每一个客户端创建服务实列,但是每次只能响应会话通道的一次请求，异步请求就没意义，客户端多线程没有意义
    //InstanceContextMode.PerCall  会话的每一次请求都会创建服务对象，异步请求也支持，但是影响请求性能
    //InstanceContextMode.Single 所有会话的请求都共用一个服务对象

    //ConcurrencyMode = ConcurrencyMode.Multiple  使用这个配置可能会出现不同步问题，WcfServerManage.wcfClientDic对象出现问题
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple,UseSynchronizationContext=false, IncludeExceptionDetailInFaults = true)]
    public class WCFHandlerService : MarshalByRefObject, IWCFHandlerService
    {

        public WCFHandlerService()
        {
        }

        #region IapiWCFHandlerService 成员

        public string CreateDomain(string ipAddress)
        {
            //客户端回调
            IClientService mCallBack = OperationContext.Current.GetCallbackChannel<IClientService>();

            string ClientID= WcfServerManage.CreateClient(OperationContext.Current.SessionId, ipAddress, DateTime.Now, mCallBack);
          
            //mCallBackList.Add(mCallBack);
            return ClientID;
        }


        public string ProcessRequest(string mProxyID, string controller, string method, string jsondata)
        {
            //WcfServerManage.hostwcfMsg.BeginInvoke(DateTime.Now, mProxyID, null, null);//异步方式不影响后台数据请求
            //Thread.Sleep(40000);//测试并发问题， 此处没有问题

            return WcfServerManage.ProcessRequest(mProxyID, controller, method, jsondata);
        }

        //异步请求
        public IAsyncResult BeginProcessRequest(string mProxyID, string controller, string method, string jsondata, AsyncCallback callback, object asyncState)
        {
            return new CompletedAsyncResult<string>(WcfServerManage.ProcessRequest(mProxyID, controller, method, jsondata));
        }

        public string EndProcessRequest(IAsyncResult result)
        {
            CompletedAsyncResult<string> ret = result as CompletedAsyncResult<string>;
            return ret.Data as string;
        }

        public bool UnDomain(string mProxyID)
        {
            //OperationContext.Current.Channel.Close();
            return WcfServerManage.UnClient(mProxyID);
        }

        public bool Heartbeat(string mProxyID)
        {
            return WcfServerManage.Heartbeat(mProxyID);
        }

        public void SendBroadcast(string jsondata)
        {
            WcfServerManage.SendBroadcast(jsondata);
        }

        public string ServerConfig()
        {
            return WcfServerManage.ServerConfig();
        }

        public string WcfServicesAllInfo()
        {
            return DebugWcfServices.getWcfServicesAllInfo();
        }
        #endregion

        void Channel_Faulted(object sender, EventArgs e)
        {
            //throw new Exception("WCF通道出错");
        }

        void Channel_Closing(object sender, EventArgs e)
        {
            //Loader.ShowHostMsg(DateTime.Now, "WCF通道关闭");
            //throw new Exception("WCF通道关闭");
        }

       

       
 
    }


    class CompletedAsyncResult<T> : IAsyncResult
    {
        T data;

        public CompletedAsyncResult(T data)
        { this.data = data; }

        public T Data
        { get { return data; } }

        #region IAsyncResult Members
        public object AsyncState
        { get { return (object)data; } }

        public WaitHandle AsyncWaitHandle
        { get { throw new Exception("The method or operation is not implemented."); } }

        public bool CompletedSynchronously
        { get { return true; } }

        public bool IsCompleted
        { get { return true; } }
        #endregion
    }

}
