using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWCoreLib.WcfFrame.ClientController;
using Books_Wcf.Entity;
using System.Data;

namespace Books_Wcf.Winform.IView
{
    public interface IfrmBookManager : IBaseView
    {
        //给网格加载数据
        void loadbooks(DataTable dt);
        //当前维护的书籍
        Books currBook { get; set; }
    }
}
