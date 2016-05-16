using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Reflection;
using System.Data;
using Newtonsoft.Json.Linq;

namespace EFWCoreLib.WcfFrame.ClientController
{
    public class JsonWcfClientController : WcfClientController
    {
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
    }

}
