using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using EFWCoreLib.CoreFrame.Business;
using EFWCoreLib.WcfFrame.DataSerialize;

namespace EFWCoreLib.WcfFrame.ServerController
{
    /// <summary>
    /// WCF控制器服务端基类
    /// </summary>
    public class WcfServerController : AbstractController
    {
        private SysLoginRight _loginRight = null;
        protected override SysLoginRight GetUserInfo()
        {
            if (_loginRight != null)
                return _loginRight;
            return base.GetUserInfo();
        }

        public void BindLoginRight(SysLoginRight loginRight)
        {
            _loginRight = loginRight;
        }

         /// <summary>
        /// 创建BaseWCFController的实例
        /// </summary>
        public WcfServerController()
        {
            
        }

        /// <summary>
        /// 初始化全局web服务参数对象
        /// </summary>
        public virtual void Init() { }


        /// <summary>
        /// 客户端传递的参数
        /// </summary>
        public ClientRequestData requestData
        {
            get;
            set;
        }

        /// <summary>
        /// 服务输出数据
        /// </summary>
        public ServiceResponseData responseData {
            get;
            set;
        }

        #region CHDEP通讯
        public ServiceResponseData InvokeWcfService(string wcfpluginname, string wcfcontroller, string wcfmethod)
        {
            return InvokeWcfService(wcfpluginname, wcfcontroller, wcfmethod, null);
        }

        public ServiceResponseData InvokeWcfService(string wcfpluginname, string wcfcontroller, string wcfmethod, Action<ClientRequestData> requestAction)
        {
            ClientLink wcfClientLink = ClientLinkManage.CreateConnection(wcfpluginname);
            ServiceResponseData retData = wcfClientLink.Request(wcfcontroller, wcfmethod, requestAction);
            return retData;
        }

        public IAsyncResult InvokeWcfServiceAsync(string wcfpluginname, string wcfcontroller, string wcfmethod, Action<ClientRequestData> requestAction, Action<ServiceResponseData> responseAction)
        {
            ClientLink wcfClientLink = ClientLinkManage.CreateConnection(wcfpluginname);
            return wcfClientLink.RequestAsync(wcfcontroller, wcfmethod, requestAction, responseAction);
        }
        #endregion


    }


}
