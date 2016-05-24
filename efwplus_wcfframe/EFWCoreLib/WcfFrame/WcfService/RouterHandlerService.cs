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

namespace EFWCoreLib.WcfFrame.WcfService
{
    
    /// <summary>
    /// 路由服务,用InstanceContextMode.Single
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any, UseSynchronizationContext = false, ValidateMustUnderstand = false)]
    public class RouterHandlerService : IRouterService
    {
        public RouterHandlerService()
        {
           
        }

        #region IRouterService Members

        /// <summary>
        /// 截获从Client端发送的消息转发到目标终结点并获得返回值给Client端
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public Message ProcessMessage(Message requestMessage)
        {
            try
            {
                //Binding binding = null;
                EndpointAddress endpointAddress = null;
                Uri touri = null;
                RouterServerManage.GetServiceEndpoint(requestMessage, out endpointAddress, out touri);
                requestMessage.Headers.To = touri;

                IDuplexRouterCallback callback = OperationContext.Current.GetCallbackChannel<IDuplexRouterCallback>();
                NetTcpBinding tbinding = new NetTcpBinding("NetTcpBinding_WCFHandlerService");
                using (DuplexChannelFactory<IRouterService> factory = new DuplexChannelFactory<IRouterService>(new InstanceContext(null, new DuplexRouterCallback(callback)), tbinding, endpointAddress))
                {

                    factory.Endpoint.Behaviors.Add(new MustUnderstandBehavior(false));
                    IRouterService proxy = factory.CreateChannel();

                    using (proxy as IDisposable)
                    {
                        // 请求消息记录
                        IClientChannel clientChannel = proxy as IClientChannel;
                        //Console.WriteLine(String.Format("Request received at {0}, to {1}\r\n\tAction: {2}", DateTime.Now, clientChannel.RemoteAddress.Uri.AbsoluteUri, requestMessage.Headers.Action));
                        if (WcfServerManage.IsDebug)
                            RouterServerManage.hostwcfMsg(Color.Black, DateTime.Now, String.Format("路由请求消息发送：  {0}", clientChannel.RemoteAddress.Uri.AbsoluteUri));
                        // 调用绑定的终结点的服务方法
                        Message responseMessage = proxy.ProcessMessage(requestMessage);

                        // 应答消息记录
                        //Console.WriteLine(String.Format("Reply received at {0}\r\n\tAction: {1}", DateTime.Now, responseMessage.Headers.Action));
                        //Console.WriteLine();
                        //hostwcfMsg(DateTime.Now, String.Format("应答消息： {0}", responseMessage.Headers.Action));
                        return responseMessage;
                    }
                }
            }
            catch (Exception e)
            {
                return Message.CreateMessage(requestMessage.Version, FaultCode.CreateReceiverFaultCode("error", RouterServerManage.ns), e.Message, requestMessage.Headers.Action);
            }
        }
        
        #endregion
    }

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DuplexRouterCallback : IDuplexRouterCallback
    {
        private IDuplexRouterCallback m_clientCallback;

        public DuplexRouterCallback(IDuplexRouterCallback clientCallback)
        {
            m_clientCallback = clientCallback;
        }

        public void ProcessMessage(Message requestMessage)
        {
            this.m_clientCallback.ProcessMessage(requestMessage);
        }
    }

    //UseSynchronizationContext 这个设置为false，必须异步执行
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any, UseSynchronizationContext = false, ValidateMustUnderstand = false)]
    public class FileRouterHandlerService : IFileRouterService
    {
        public FileRouterHandlerService()
        {

        }

        #region IRouterService Members

        /// <summary>
        /// 截获从Client端发送的消息转发到目标终结点并获得返回值给Client端
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public Message ProcessMessage(Message requestMessage)
        {
            try
            {
                //Binding binding = null;
                EndpointAddress endpointAddress = null;
                Uri touri = null;


                RouterServerManage.GetServiceEndpointFile(requestMessage, out endpointAddress, out touri);
                requestMessage.Headers.To = touri;


                NetTcpBinding tbinding = new NetTcpBinding("NetTcpBinding_FileTransferHandlerService");
                using (ChannelFactory<IFileRouterService> factory = new ChannelFactory<IFileRouterService>(tbinding, endpointAddress))
                {

                    factory.Endpoint.Behaviors.Add(new MustUnderstandBehavior(false));
                    IFileRouterService proxy = factory.CreateChannel();

                    using (proxy as IDisposable)
                    {
                        // 请求消息记录
                        IClientChannel clientChannel = proxy as IClientChannel;
                        if (WcfServerManage.IsDebug)
                            RouterServerManage.hostwcfMsg(Color.Black, DateTime.Now, String.Format("路由请求消息发送：  {0}", clientChannel.RemoteAddress.Uri.AbsoluteUri));
                        // 调用绑定的终结点的服务方法
                        Message responseMessage = proxy.ProcessMessage(requestMessage);

                        return responseMessage;
                    }
                }

            }
            catch (Exception e)
            {
                return Message.CreateMessage(requestMessage.Version, FaultCode.CreateReceiverFaultCode("error", RouterServerManage.ns), e.Message, requestMessage.Headers.Action);
            }
        }


        #endregion
    }

}
