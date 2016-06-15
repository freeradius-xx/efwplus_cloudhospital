using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using EFWCoreLib.CoreFrame.Business;
using EFWCoreLib.CoreFrame.Business.AttributeInfo;
using EFWCoreLib.CoreFrame.Business.Interface;
using EFWCoreLib.CoreFrame.DbProvider;
using EFWCoreLib.CoreFrame.Init;
using EFWCoreLib.CoreFrame.Plugin;
using EFWCoreLib.WcfFrame;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.Unity;
using EFWCoreLib.WcfFrame.DataSerialize;

namespace EFWCoreLib.WebFrame.WebAPI
{

    public class WebApiController : ApiController, INewObject, INewDao
    {
        private AbstractDatabase _oleDb = null;
        private IUnityContainer _container = null;
        private ICacheManager _cache = null;
        private string _pluginName = null;
        //private HttpContext _context = HttpContext.Current;

        public AbstractDatabase oleDb
        {
            get
            {
                return _oleDb;
            }
        }
        public string PluginName
        {
            get { return _pluginName; }
            set { _pluginName = value; }
        }

       

        public WebApiController()
            : base()
        {
            efwplusApiControllerAttribute[] apiC = ((efwplusApiControllerAttribute[])this.GetType().GetCustomAttributes(typeof(efwplusApiControllerAttribute), true));
            if (apiC.Length > 0)
                _pluginName = apiC[0].PluginName;
        }

        #region INewObject 成员
        public T NewObject<T>()
        {
            T t = FactoryModel.GetObject<T>(_oleDb, _container, _cache, _pluginName, null);
            return t;
        }

        public T NewObject<T>(string unityname)
        {
            T t = FactoryModel.GetObject<T>(_oleDb, _container, _cache, _pluginName, unityname);
            return t;
        }

        #endregion

        #region INewDao 成员

        public T NewDao<T>()
        {
            T t = FactoryModel.GetObject<T>(_oleDb, _container, _cache, _pluginName, null);
            return t;
        }

        public T NewDao<T>(string unityname)
        {
            T t = FactoryModel.GetObject<T>(_oleDb, _container, _cache, _pluginName, unityname);
            return t;
        }

        #endregion

        protected void InitPlugin(string pluginname)
        {
            //Page_Load 进行初始化
            _pluginName = pluginname;
            if (pluginname == "coresys") return;
            ModulePlugin mp;
            mp = AppPluginManage.PluginDic[_pluginName];
            if (mp == null)
                throw new Exception("插件名：" + pluginname + "不存在！");
            _oleDb = mp.database;
            _container = mp.container;
            _cache = mp.cache;
        }

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            
            _pluginName = this.SetPluginName();
            if (_pluginName != null)
            {
                InitPlugin(_pluginName);
            }
        }

        protected virtual string SetPluginName()
        {
            return _pluginName;
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