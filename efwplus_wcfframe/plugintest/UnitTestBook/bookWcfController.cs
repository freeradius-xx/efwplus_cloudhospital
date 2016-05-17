using System;
using System.Collections.Generic;
using Books_Wcf.Entity;
using EFWCoreLib.UnitTestFrame;
using EFWCoreLib.WcfFrame;
using EFWCoreLib.WcfFrame.ClientController;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestBook
{
    [TestClass]
    public class bookWcfController : wcfBaseUnitTest
    {
        //创建对象
        //private static ReplyClientCallBack callback = new ReplyClientCallBack();
        //private static ClientLink clientlink = null;
         

        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        [ClassInitialize()]
        public static void Init(TestContext testContext)
        {
            //clientlink = new ClientLink("myendpoint", callback, "kakake");
            //clientlink.CreateConnection();
            wcfpluginname = "Books_Wcf";
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
            object retobj = InvokeWcfService("GetBooks");
            List<Books> list = ToListObj<Books>(retobj);
            Assert.AreEqual(list.Count > 0, true,"没有返回数据");
        }

        [TestMethod]
        public void SaveBook()
        {
            Books book = new Books();
            object retobj= InvokeWcfService("SaveBook", ToJson(book));
            Assert.AreEqual(ToBoolean(retobj), true, "保存数据失败");
        }
    }
}
