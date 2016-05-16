//2011.10.11 添加模板功能
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using EFWCoreLib.CoreFrame.DbProvider;
using EFWCoreLib.CoreFrame.Init;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using EFWCoreLib.CoreFrame.Business;
using System.Text;
using Newtonsoft.Json;
using EFWCoreLib.WcfFrame;
using EFWCoreLib.WcfFrame.ClientController;
using Newtonsoft.Json.Linq;

namespace EFWCoreLib.WebFrame.HttpHandler.Controller
{
    /// <summary>
    /// WebHttpController控制器
    /// </summary>
    public class WebHttpController : AbstractController
    {
        public HttpContext context { get; set; }

        /// <summary>
        /// Ajax请求返回Json数据
        /// </summary>
        public string JsonResult { get; set; }
        /// <summary>
        /// URL请求界面
        /// </summary>
        public string ViewResult { get; set; }
        /// <summary>
        /// 界面数据
        /// </summary>
        public Dictionary<string, Object> ViewData { get; set; }

        protected override SysLoginRight GetUserInfo()
        {
            if (sessionData != null && sessionData.ContainsKey("RoleUser"))
            {
                return (SysLoginRight)sessionData["RoleUser"];
            }
            return base.GetUserInfo();
        }

        

        //封装的页面子权限
        //public DataTable GetPageRight(int MenuId)
        //{
        //    DataTable data = (DataTable)ExecuteFun.invoke(oleDb, "getPageRight", MenuId, LoginUserInfo.UserId);
        //    return data;
        //}

        private System.Collections.Generic.Dictionary<string, Object> _sessionData;
        /// <summary>
        /// Session数据传入后台
        /// </summary>
        public System.Collections.Generic.Dictionary<string, Object> sessionData
        {
            get
            {
                return _sessionData;
            }
            set
            {
                _sessionData = value;
            }
        }

        private System.Collections.Generic.Dictionary<string, Object> _putOutData;
        /// <summary>
        /// 后台传出数据到Session数据
        /// </summary>
        public System.Collections.Generic.Dictionary<string, Object> PutOutData
        {
            get
            {
                return _putOutData;
            }
            set
            {
                _putOutData = value;
            }
        }

        private List<string> _clearKey;
        /// <summary>
        /// 清除Session的数据
        /// </summary>
        public List<string> ClearKey
        {
            get { return _clearKey; }
            set { _clearKey = value; }
        }

        private System.Collections.Generic.Dictionary<string, string> _paramsData;
        /// <summary>
        /// Url参数传递数据
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> ParamsData
        {
            get { return _paramsData; }
            set { _paramsData = value; }
        }

        private System.Collections.Generic.Dictionary<string, string> _formData;
        /// <summary>
        /// Form提交的数据
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> FormData
        {
            get { return _formData; }
            set { _formData = value; }
        }


        public string RetSuccess()
        {
            return RetSuccess(null, null);
        }

        public string RetSuccess(string info)
        {
            return RetSuccess(info, null);
        }

        public string RetSuccess(string info, string data)
        {
            info = info == null ? "" : info;
            data = data == null ? "{}" : data;
            return "{\"ret\":0,\"msg\":" + "\"" + info + "\"" + ",\"data\":" + data + "}";
        }

        public string RetError()
        {
            return RetError(null, null);
        }

        public string RetError(string info)
        {
            return RetError(info, null);
        }

        public string RetError(string info, string data)
        {
            info = info == null ? "" : info;
            data = data == null ? "{}" : data;
            return "{\"ret\":1,\"msg\":" + "\"" + info + "\"" + ",\"data\":" + data + "}";
        }

        public string ToUrl(string url)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script language=\"javascript\" type=\"text/javascript\">\n");
            sb.Append("window.location.href='" + url + "'\n");
            sb.Append("</script>\n");
            return sb.ToString();
        }

        public string ToJson2(object obj)
        {
            string value = JsonConvert.SerializeObject(obj);
            return value;
        }


        #region 与CHDEP通讯
        public ClientLink wcfClientLink
        {
            get
            {
                if (context.Session["wcfClientLink"] == null)
                {
                    //创建对象
                    ReplyClientCallBack callback = new ReplyClientCallBack();
                    ClientLink clientlink = new ClientLink("myendpoint", callback, GetUserInfo().EmpId.ToString() + "." + GetUserInfo().EmpName);
                    clientlink.CreateConnection();
                    context.Session["wcfClientLink"] = clientlink;
                }
                return context.Session["wcfClientLink"] as ClientLink;
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