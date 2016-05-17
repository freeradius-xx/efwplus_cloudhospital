using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWCoreLib.CoreFrame.Business.AttributeInfo;
using EFWCoreLib.WcfFrame.ServerController;
using Books_Wcf.Entity;
using System.Data;
using Books_Wcf.Dao;

namespace Books_Wcf.WcfController
{
    [WCFController]
    public class bookWcfController : JsonWcfServerController
    {
        [WCFMethod]
        public string SaveBook()
        {
            Books book = ToObject<Books>(ParamJsonData);
            //book.BindDb(oleDb, _container,_cache,_pluginName);//反序列化的对象，必须绑定数据库操作对象
            book.save();
            return ToJson(true);
        }

        [WCFMethod]
        public string GetBooks()
        {
            DataTable dt = NewDao<IBookDao>().GetBooks("", 0);
            return base.ToJson(dt);
        }

        [WCFMethod]
        public string TestData()
        {
            DataTable dt = null;

            string retJson = null;

            dt = NewObject<Books>().gettable();

            DataTable _dt = dt.Clone();
            int num = Convert.ToInt32(ToArray(ParamJsonData)[0]);
            for (int i = 0; i < num; i++)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _dt.Rows.Add(dr.ItemArray);
                }
            }

            dt = _dt;

            retJson = ToJson(dt);

            return retJson;
        }
        [WCFMethod]
        public string Test191()
        {
            int num = Convert.ToInt32(ToArray(ParamJsonData)[0]);
            string strsql = string.Format(@"SELECT TOP {0} * FROM hisdb..books", num);
            DataTable dt = oleDb.GetDataTable(strsql);

            return ToJson(dt);
        }
    }
}

