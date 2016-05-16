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
using EFWCoreLib.WcfFrame.ClientController;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EFWCoreLib.WebFrame.WebAPI
{

    public class WebApiController : ApiController, INewObject, INewDao
    {
        private AbstractDatabase _oleDb = null;
        private IUnityContainer _container = null;
        private ICacheManager _cache = null;
        private string _pluginName = null;
        private HttpContext _context = HttpContext.Current;

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


        #region 与CHDEP通讯
        public ClientLink wcfClientLink
        {
            get
            {
                if (_context.Session["wcfClientLink"] == null)
                {
                    //创建对象
                    ReplyClientCallBack callback = new ReplyClientCallBack();
                    ClientLink clientlink = new ClientLink("myendpoint", callback, _context.Session.SessionID);
                    clientlink.CreateConnection();
                    _context.Session["wcfClientLink"] = clientlink;
                }
                return _context.Session["wcfClientLink"] as ClientLink;
            }
        }

        public Object InvokeWcfService(string wcfpluginname, string wcfcontroller, string wcfmethod)
        {
            return InvokeWcfService(wcfpluginname, wcfcontroller, wcfmethod, "[]");
        }

        public Object InvokeWcfService(string wcfpluginname, string wcfcontroller, string wcfmethod, string jsondata)
        {
            if (string.IsNullOrEmpty(jsondata)) jsondata = "[]";
            string retJson = wcfClientLink.Request(wcfpluginname + "@" + wcfcontroller, wcfmethod, jsondata);

            object Result = JsonConvert.DeserializeObject(retJson);
            int ret = Convert.ToInt32((((Newtonsoft.Json.Linq.JObject)Result)["flag"]).ToString());
            string msg = (((Newtonsoft.Json.Linq.JObject)Result)["msg"]).ToString();
            if (ret == 1)
            {
                throw new Exception(msg);
            }
            else
            {
                return ((Newtonsoft.Json.Linq.JObject)(Result))["data"];
            }
        }

        public IAsyncResult InvokeWcfServiceAsync(string wcfpluginname, string wcfcontroller, string wcfmethod, string jsondata, Action<Object> action)
        {
            if (string.IsNullOrEmpty(jsondata)) jsondata = "[]";
            Action<string> retAction = delegate(string retJson)
            {
                object Result = JsonConvert.DeserializeObject(retJson);
                int ret = Convert.ToInt32((((Newtonsoft.Json.Linq.JObject)Result)["flag"]).ToString());
                string msg = (((Newtonsoft.Json.Linq.JObject)Result)["msg"]).ToString();
                if (ret == 1)
                {
                    throw new Exception(msg);
                }
                else
                {
                    action(((Newtonsoft.Json.Linq.JObject)(Result))["data"]);
                }
            };
            return wcfClientLink.RequestAsync(wcfpluginname + "@" + wcfcontroller, wcfmethod, jsondata, retAction);
        }

        #region ToJson

        public string ToJson(params object[] data)
        {
            string value = JsonConvert.SerializeObject(data);
            return value;
        }
        public string ToJson(object model)
        {
            string value = JsonConvert.SerializeObject(model);
            return value;
        }
        public string ToJson(System.Data.DataTable dt)
        {
            string value = JsonConvert.SerializeObject(dt, Formatting.Indented);
            return value;
        }
        public string ToJson(string data)
        {
            object[] objs = new object[] { data };
            return ToJson(objs);
        }
        public string ToJson(int data)
        {
            object[] objs = new object[] { data };
            return ToJson(objs);
        }
        public string ToJson(decimal data)
        {
            object[] objs = new object[] { data };
            return ToJson(objs);
        }
        public string ToJson(bool data)
        {
            object[] objs = new object[] { data };
            return ToJson(objs);
        }
        public string ToJson(DateTime data)
        {
            object[] objs = new object[] { data };
            return ToJson(objs);
        }
        #endregion

        #region ToObject
        private object convertVal(Type t, object _data)
        {

            string data = _data.ToString();
            object val = null;
            if (t == typeof(Int32))
                val = Convert.ToInt32(data);
            else if (t == typeof(DateTime))
                val = Convert.ToDateTime(data);
            else if (t == typeof(Decimal))
                val = Convert.ToDecimal(data);
            else if (t == typeof(Boolean))
                val = Convert.ToBoolean(data);
            else if (t == typeof(String))
                val = Convert.ToString(data).Trim();
            else if (t == typeof(Guid))
                val = new Guid(data.ToString());
            else if (t == typeof(byte[]))
                if (data != null && data.ToString().Length > 0)
                {
                    val = Convert.FromBase64String(data.ToString());
                }
                else
                {
                    val = null;
                }
            else
                val = data;
            return val;
        }

        public object[] ToArray(object data)
        {
            return (data as JArray).ToArray();
        }
        public List<T> ToListObj<T>(object data)
        {
            if (data is JArray)
            {
                PropertyInfo[] pros = typeof(T).GetProperties();
                List<T> list = new List<T>();
                for (int i = 0; i < (data as JArray).Count; i++)
                {
                    T obj = (T)Activator.CreateInstance(typeof(T));
                    object _data = (data as JArray)[i];
                    for (int k = 0; k < pros.Length; k++)
                    {
                        object val = convertVal(pros[k].PropertyType, (_data as JObject)[pros[k].Name]);
                        pros[k].SetValue(obj, val, null);
                    }
                    list.Add(obj);
                }
                return list;
            }

            return null;
        }

        public DataTable ToDataTable(Object data)
        {
            return ToDataTable(data.ToString());
        }

        public DataTable ToDataTable(string data)
        {
            return JsonConvert.DeserializeObject<DataTable>(data);
        }
        public T ToObject<T>(object data)
        {
            T obj = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] pros = typeof(T).GetProperties();
            for (int k = 0; k < pros.Length; k++)
            {
                object val = convertVal(pros[k].PropertyType, (data as JObject)[pros[k].Name]);
                pros[k].SetValue(obj, val, null);
            }

            return obj;
        }
        public T ToObject<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
        public string ToString(object data)
        {
            return Convert.ToString(ToArray(data)[0].ToString());
        }
        public bool ToBoolean(object data)
        {
            return Convert.ToBoolean(ToArray(data)[0].ToString());
        }
        public int ToInt32(object data)
        {
            return Convert.ToInt32(ToArray(data)[0].ToString());
        }
        public decimal ToDecimal(object data)
        {
            return Convert.ToDecimal(ToArray(data)[0].ToString());
        }
        public DateTime ToDateTime(object data)
        {
            return Convert.ToDateTime(ToArray(data)[0].ToString());
        }
        #endregion

        #endregion
    }
}