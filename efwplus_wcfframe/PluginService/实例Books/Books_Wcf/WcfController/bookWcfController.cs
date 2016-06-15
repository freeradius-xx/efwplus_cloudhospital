using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWCoreLib.CoreFrame.Business.AttributeInfo;
using EFWCoreLib.WcfFrame.ServerController;
using Books_Wcf.Entity;
using System.Data;
using Books_Wcf.Dao;
using EFWCoreLib.WcfFrame.DataSerialize;

namespace Books_Wcf.WcfController
{
    [WCFController]
    public class bookWcfController : WcfServerController
    {
        [WCFMethod]
        public ServiceResponseData SaveBook()
        {
            Books book = requestData.GetData<Books>(0);
            book.BindDb(oleDb, _container,_cache,_pluginName);//反序列化的对象，必须绑定数据库操作对象
            book.save();

            responseData.AddData(true);
            return responseData;
        }

        [WCFMethod]
        public ServiceResponseData GetBooks()
        {
            DataTable dt = NewDao<IBookDao>().GetBooks("", 0);

            
            responseData.AddData(dt);
            return responseData;
        }
    }
}

