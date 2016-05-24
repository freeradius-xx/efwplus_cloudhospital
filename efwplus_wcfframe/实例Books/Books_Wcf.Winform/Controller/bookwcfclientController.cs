using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWCoreLib.WcfFrame.ClientController;
using EFWCoreLib.CoreFrame.Business.AttributeInfo;
using Books_Wcf.Winform.IView;
using System.Data;

namespace Books_Wcf.Winform.Controller
{
    [WinformController(DefaultViewName = "frmBookManager")]//在菜单上显示
    [WinformView(Name = "frmBookManager", DllName = "Books_Wcf.Winform.dll", ViewTypeName = "Books_Wcf.Winform.ViewForm.frmBookManager")]//控制器关联的界面
    public class bookwcfclientController : WcfClientController
    {
        IfrmBookManager _ifrmbookmanager;
        public override void Init()
        {
            _ifrmbookmanager = (IfrmBookManager)DefaultView;
            //初始化加载书籍目录
            //GetBooks();
            /*
            //利用wcf回调实现服务端发消息给客户端
            replyClientCallBack.ReplyClientAction = new Action<string>(
                (s) =>
                {
                    //将s输出到界面上
                });*/
        }

        //保存
        [WinformMethod]
        public void bookSave()
        {
            //通过wcf服务调用bookWcfController控制器中的SaveBook方法，并传递参数Book对象
            InvokeWcfService("Books_Wcf", "bookWcfController", "SaveBook", ToJson(_ifrmbookmanager.currBook));
            GetBooks();
        }

        //获取书籍目录
        [WinformMethod]
        public void GetBooks()
        {
            //通过wcf服务调用bookWcfController控制器中的GetBooks方法
            Object retdata = InvokeWcfService("Books_Wcf", "bookWcfController", "GetBooks");
            DataTable dt = ToDataTable(retdata);
            _ifrmbookmanager.loadbooks(dt);
        }
    }
}

