﻿/*
 *Wcf客户端控制器基类
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using EFWCoreLib.CoreFrame.Init;
using System.Windows.Forms;
using EFWCoreLib.WinformFrame.Controller;
using EFWCoreLib.WcfFrame.DataSerialize;

namespace EFWCoreLib.WcfFrame.ClientController
{
 
    /// <summary>
    /// Winform控制器基类
    /// </summary>
    public class WcfClientController : WinformController
    {
        
        /// <summary>
        /// 创建WinformController的实例
        /// </summary>
        public WcfClientController()
        {
            
        }
        /// <summary>
        /// 界面控制事件
        /// </summary>
        /// <param name="eventname">事件名称</param>
        /// <param name="objs">参数数组</param>
        /// <returns></returns>
        public override object UI_ControllerEvent(string eventname, params object[] objs)
        {
            try
            {
                switch (eventname)
                {
                    case "Show":
                        if (objs.Length == 1)
                        {
                            Form form = objs[0] as Form;
                            string tabName = form.Text;
                            string tabId = "view" + form.GetHashCode();
                            InvokeController("MainFrame.UI", "wcfclientLoginController", "ShowForm", form, tabName, tabId);
                        }
                        else if (objs.Length == 2)
                        {
                            Form form = objs[0] as Form;
                            string tabName = objs[1].ToString();
                            string tabId = "view" + form.GetHashCode();
                            InvokeController("MainFrame.UI", "wcfclientLoginController", "ShowForm", form, tabName, tabId);
                        }
                        else if (objs.Length == 3)
                        {
                            InvokeController("MainFrame.UI", "wcfclientLoginController", "ShowForm", objs);
                        }
                        return true;
                    case "Close":
                        if (objs[0] is Form)
                        {
                            string tabId = "view" + objs[0].GetHashCode();
                            InvokeController("MainFrame.UI", "wcfclientLoginController", "CloseForm", tabId);
                        }
                        else
                        {
                            InvokeController("MainFrame.UI", "wcfclientLoginController", "CloseForm", objs);
                        }
                        return true;
                    case "Exit":
                        AppGlobal.AppExit();
                        return null;
                    case "this":
                        return this;
                }

                MethodInfo meth = ControllerHelper.CreateMethodInfo(_pluginName + "@" + this.GetType().Name, eventname);
                return meth.Invoke(this, objs);
            }
            catch (Exception err)
            {
                //记录错误日志
                EFWCoreLib.CoreFrame.EntLib.ZhyContainer.CreateException().HandleException(err, "HISPolicy");
                if(err.InnerException!=null)
                    throw new Exception(err.InnerException.Message);
                throw new Exception(err.Message);
            }
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
