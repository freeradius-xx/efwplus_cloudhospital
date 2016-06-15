using System;
using System.Collections.Generic;
using Books_Wcf.Entity;
using EFWCoreLib.UnitTestFrame;
using EFWCoreLib.WcfFrame;
using EFWCoreLib.WcfFrame.ClientController;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EFWCoreLib.WcfFrame.DataSerialize;

namespace UnitTestBook
{
    [TestClass]
    public class bookWcfController : wcfBaseUnitTest
    { 

        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        [ClassInitialize()]
        public static void Init(TestContext testContext)
        {
            //clientlink = new ClientLink("wcfendpoint", callback, "kakake");
            //clientlink.CreateConnection();
            wcfpluginname = "Books.Service";
            wcfcontroller = "bookWcfController";
        }

        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        [ClassCleanup()]
        public static void Quit()
        {
            wcfClientLink.Dispose();
        }

        [TestMethod]
        public void GetBooks()
        {
            try
            {
                ServiceResponseData retobj = InvokeWcfService("GetBooks");
                List<Books> list = retobj.GetData<List<Books>>(0);
                Assert.AreEqual(list.Count > 0, true, "没有返回数据");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void SaveBook()
        {
            try
            {
                Books book;
                ServiceResponseData retobj;

                book = new Books();
                book.BookName = "测试书籍";

                retobj = InvokeWcfService("SaveBook", (ClientRequestData request) =>
                {
                    request.AddData(book);
                });
                Assert.AreEqual(retobj.GetData<bool>(0), true, "保存数据失败");

                book = new Books();
                book.BookName = "测试书籍2";
                book.Flag = 1;
                retobj = InvokeWcfService("SaveBook", (ClientRequestData request) =>
                {
                    request.AddData(book);
                });
                Assert.AreEqual(retobj.GetData<bool>(0), true, "保存数据失败");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
