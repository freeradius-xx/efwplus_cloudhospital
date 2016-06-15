using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWCoreLib.WebFrame.HttpHandler.Controller;
using EFWCoreLib.CoreFrame.Business.AttributeInfo;
using Books_Web.Entity;
using System.Data;
using EFWCoreLib.WcfFrame.DataSerialize;

namespace Books_Web.WebController
{
    [WebController]
    public class bookController : JEasyUIController
    {

        [WebMethod]
        public void SaveBook()
        {
            Books book = GetModel<Books>();
            book.save();
            JsonResult = RetSuccess("保存书籍成功！");
        }

        [WebMethod]
        public void SearchBook()
        {
            string schar = ParamsData["schar"];
            int flag = Convert.ToInt32(ParamsData["flag"]);

            //wcfClientLink.Request("Books_Wcf@bookWcfController", "GetBooks", "[]");
            //DataTable dt = NewObject<Books>().gettable();

            ServiceResponseData retData = InvokeWcfService("Books.Service", "bookWcfController", "GetBooks");
            JsonResult = ToGridJson(retData.GetData<DataTable>(0));
        }
    }
}
