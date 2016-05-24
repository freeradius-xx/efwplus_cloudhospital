/*
 *控制器的目的：
 *使界面对象与服务对象达到隔离和重用的目的 
 *所以控制器是把界面对象与服务对象组合一些业务功能、一些菜单。
 *如果一个界面有两个菜单那就分开建两个控制器对象。
 * 
 */


using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using EFWCoreLib.CoreFrame.Business;
using EFWCoreLib.CoreFrame.DbProvider;
using EFWCoreLib.CoreFrame.Init;
using Newtonsoft.Json;
using EFWCoreLib.CoreFrame.Common;
using System.Windows.Forms;
using EFWCoreLib.WinformFrame.Controller;
using Newtonsoft.Json.Linq;

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
                            InvokeController("WcfMainUIFrame", "wcfclientLoginController", "ShowForm", form, tabName, tabId);
                        }
                        else if (objs.Length == 2)
                        {
                            Form form = objs[0] as Form;
                            string tabName = objs[1].ToString();
                            string tabId = "view" + form.GetHashCode();
                            InvokeController("WcfMainUIFrame", "wcfclientLoginController", "ShowForm", form, tabName, tabId);
                        }
                        else if (objs.Length == 3)
                        {
                            InvokeController("WcfMainUIFrame", "wcfclientLoginController", "ShowForm", objs);
                        }
                        return true;
                    case "Close":
                        if (objs[0] is Form)
                        {
                            string tabId = "view" + objs[0].GetHashCode();
                            InvokeController("WcfMainUIFrame", "wcfclientLoginController", "CloseForm", tabId);
                        }
                        else
                        {
                            InvokeController("WcfMainUIFrame", "wcfclientLoginController", "CloseForm", objs);
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
        public Object InvokeWcfService(string wcfpluginname, string wcfcontroller, string wcfmethod)
        {
            return InvokeWcfService(wcfpluginname, wcfcontroller, wcfmethod, "[]");
        }

        public Object InvokeWcfService(string wcfpluginname, string wcfcontroller, string wcfmethod, string jsondata)
        {
            if (string.IsNullOrEmpty(jsondata)) jsondata = "[]";
            ClientLink wcfClientLink = ClientLinkManage.CreateConnection(wcfpluginname);
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
            ClientLink wcfClientLink = ClientLinkManage.CreateConnection(wcfpluginname);
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
