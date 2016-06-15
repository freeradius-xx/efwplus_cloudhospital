using EFWCoreLib.WcfFrame.SDMessageHeader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using ProtoBuf.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFWCoreLib.WcfFrame.DataSerialize
{
    /// <summary>
    /// 客户端请求数据
    /// </summary>
    public class ClientRequestData
    {
        //string _retData;
        List<string> _listjson;
        
        //bool _iscustomwcfconfig=false;
        //List<Object> listdata;
        public ClientRequestData()
        {
            //Iscustomwcfconfig = false;
            //listdata = new List<object>();
            _listjson = new List<string>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsCustomWcfConfig">是否自定义Wcf配置</param>
        /// <param name="IsCompress">是否压缩</param>
        /// <param name="IsEncrytion">是否加密</param>
        /// <param name="SerializeType">序列化方式</param>
        public ClientRequestData(bool IsCompress,bool IsEncrytion,SerializeType SerializeType)
        {
            //Iscustomwcfconfig = IsCustomWcfConfig;
            _iscompressjson = IsCompress;
            _isencryptionjson = IsEncrytion;
            _serializetype = SerializeType;
            //listdata = new List<object>();
            _listjson = new List<string>();
        }
        bool _iscompressjson = false;
        bool _isencryptionjson = false;
        SerializeType _serializetype=SerializeType.Newtonsoft;

        public bool Iscompressjson
        {
            get
            {
                return _iscompressjson;
            }

            set
            {
                _iscompressjson = value;
            }
        }

        public bool Isencryptionjson
        {
            get
            {
                return _isencryptionjson;
            }

            set
            {
                _isencryptionjson = value;
            }
        }

        public SerializeType Serializetype
        {
            get
            {
                return _serializetype;
            }

            set
            {
                _serializetype = value;
            }
        }

        //public bool Iscustomwcfconfig
        //{
        //    get
        //    {
        //        return _iscustomwcfconfig;
        //    }

        //    set
        //    {
        //        _iscustomwcfconfig = value;
        //    }
        //}

        public void AddData<T>(T data)
        {

            if (_serializetype == SerializeType.Newtonsoft)
            {
                if (data is DataTable)
                {
                    _listjson.Add(JsonConvert.SerializeObject(data, Formatting.Indented));
                }
                else
                {
                    _listjson.Add(JsonConvert.SerializeObject(data));
                }
            }
            else if (_serializetype == SerializeType.protobuf)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    if (data is DataTable)
                    {
                        object obj = data;
                        DataSerializer.Serialize(ms, (DataTable)obj);
                    }
                    else
                    {
                        Serializer.Serialize<T>(ms, data);
                    }
                    _listjson.Add(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
                }
            }
        }

        public string GetJsonData()
        {
            return JsonConvert.SerializeObject(_listjson);
        }

        public void SetJsonData(string retData)
        {
            //_retData = retData;
            _listjson = JsonConvert.DeserializeObject<List<string>>(retData);
        }

        public T GetData<T>(int index)
        {
            if (_serializetype == SerializeType.Newtonsoft)
            {
                return JsonConvert.DeserializeObject<T>(_listjson[index]);
            }
            else if (_serializetype == SerializeType.protobuf)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(_listjson[index]);
                using (MemoryStream ms = new MemoryStream(bytes))
                {

                    if (default(T) is DataTable)
                    {
                        Object obj = DataSerializer.DeserializeDataTable(ms);
                        return (T)obj;
                    }
                    else
                    {
                        return Serializer.Deserialize<T>(ms);
                    }
                }
            }
            else
                return default(T);
        }

        /*
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
        */
    }
}
