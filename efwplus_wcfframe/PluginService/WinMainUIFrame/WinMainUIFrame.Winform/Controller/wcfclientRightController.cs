using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EFWCoreLib.CoreFrame.Business.AttributeInfo;
using EFWCoreLib.WcfFrame.ClientController;
using WinMainUIFrame.Entity;
using WinMainUIFrame.Winform.IView.RightManager;
using EFWCoreLib.WcfFrame.DataSerialize;

namespace WinMainUIFrame.Winform.Controller
{
    [WinformController(DefaultViewName = "frmMenu")]//与系统菜单对应
    [WinformView(Name = "frmMenu", DllName = "WinMainUIFrame.Winform.dll", ViewTypeName = "WinMainUIFrame.Winform.ViewForm.RightManager.frmMenu")]
    [WinformView(Name = "frmGroupMenu", DllName = "WinMainUIFrame.Winform.dll", ViewTypeName = "WinMainUIFrame.Winform.ViewForm.RightManager.frmGroupMenu")]
    public class wcfclientRightController : WcfClientController
    {
        IfrmMenu frmMenu;
        IfrmGroupMenu frmgroupmenu;
        public override void Init()
        {
            frmMenu = (IfrmMenu)iBaseView["frmMenu"];
            frmgroupmenu = (IfrmGroupMenu)iBaseView["frmGroupMenu"];
        }
        [WinformMethod]
        public void InitMenuData()
        {
            ServiceResponseData retdata = InvokeWcfService("MainFrame.Service", "RightController", "InitMenuData");
            List<BaseModule> modulelist = retdata.GetData<List<BaseModule>>(0);
            List<BaseMenu> menulist = retdata.GetData<List<BaseMenu>>(1);

            frmMenu.loadMenuTree(modulelist, menulist);
        }
        [WinformMethod]
        public void NewMenu()
        {
            ServiceResponseData retdata = InvokeWcfService("MainFrame.Service", "RightController", "NewMenu");
            BaseMenu menu = retdata.GetData<BaseMenu>(0);
            frmMenu.currentMenu = menu;

        }
        [WinformMethod]
        public void SaveMenu()
        {
            Action<ClientRequestData> requestAction = ((ClientRequestData request) =>
            {
                request.AddData(frmMenu.currentMenu);
            });

            Object retdata = InvokeWcfService("MainFrame.Service", "RightController", "SaveMenu", requestAction);
            InitMenuData();
        }
        [WinformMethod]
        public void DeleteMenu(int menuId)
        {
            Action<ClientRequestData> requestAction = ((ClientRequestData request) =>
            {
                request.AddData(menuId);
            });

            Object retdata = InvokeWcfService("MainFrame.Service", "RightController", "DeleteMenu", requestAction);
            InitMenuData();
        }

        [WinformMethod]
        public void InitGroupData()
        {
            ServiceResponseData retdata = InvokeWcfService("MainFrame.Service", "RightController", "InitGroupData");
            List<BaseGroup> grouplist = retdata.GetData<List<BaseGroup>>(0);
            frmgroupmenu.loadGroupGrid(grouplist);
        }
        [WinformMethod]
        public void LoadGroupMenuData(int groupId)
        {
            Action<ClientRequestData> requestAction = ((ClientRequestData request) =>
            {
                request.AddData(groupId);
            });

            ServiceResponseData retdata = InvokeWcfService("MainFrame.Service", "RightController", "LoadGroupMenuData", requestAction);

            List<BaseModule> modulelist = retdata.GetData<List<BaseModule>>(0);
            List<BaseMenu> menulist = retdata.GetData<List<BaseMenu>>(1);
            List<BaseMenu> groupmenulist = retdata.GetData<List<BaseMenu>>(2);

            frmgroupmenu.loadMenuTree(modulelist, menulist, groupmenulist);
        }
        [WinformMethod]
        public void SetGroupMenu(int groupId, int[] menuIds)
        {
            Action<ClientRequestData> requestAction = ((ClientRequestData request) =>
            {
                request.AddData(groupId);
                request.AddData(menuIds);
            });

            Object retdata = InvokeWcfService("MainFrame.Service", "RightController", "SetGroupMenu", requestAction);
        }

        #region 页面权限
        [WinformMethod]
        public void GetPageMenuData()
        {
            frmgroupmenu.panelPageEnabled = false;
            if (frmgroupmenu.currGroupId == -1 || frmgroupmenu.currMenu == null)
            {
                frmgroupmenu.loadPageMenu(null);
            }
            else
            {
                if (frmgroupmenu.currMenu.FunName.Trim() == "" && frmgroupmenu.currMenu.UrlName.Trim() == "")
                    frmgroupmenu.panelPageEnabled = false;
                else
                    frmgroupmenu.panelPageEnabled = true;

                Action<ClientRequestData> requestAction = ((ClientRequestData request) =>
                {
                    request.AddData(frmgroupmenu.currGroupId);
                    request.AddData(frmgroupmenu.currMenu.MenuId);
                });

                ServiceResponseData retdata = InvokeWcfService("MainFrame.Service", "RightController", "GetPageMenuData", requestAction);

                DataTable dt = retdata.GetData<DataTable>(0);
                frmgroupmenu.loadPageMenu(dt);
            }
        }
        [WinformMethod]
        public void SavePageMenu(int moduleId, int menuId, string code, string name)
        {
            Action<ClientRequestData> requestAction = ((ClientRequestData request) =>
            {
                request.AddData(moduleId);
                request.AddData(menuId);
                request.AddData(code);
                request.AddData(name);
            });
            Object retdata = InvokeWcfService("MainFrame.Service", "RightController", "SavePageMenu", requestAction);

            GetPageMenuData();
        }
        [WinformMethod]
        public void DeletePageMenu(int pageId)
        {
            Action<ClientRequestData> requestAction = ((ClientRequestData request) =>
            {
                request.AddData(pageId);
            });
            Object retdata = InvokeWcfService("MainFrame.Service", "RightController", "DeletePageMenu", requestAction);

            GetPageMenuData();
        }
        [WinformMethod]
        public void SetGroupPage(int groupId, int pageId)
        {
            Action<ClientRequestData> requestAction = ((ClientRequestData request) =>
            {
                request.AddData(groupId);
                request.AddData(pageId);
            });
            Object retdata = InvokeWcfService("MainFrame.Service", "RightController", "SetGroupPage", requestAction);

            GetPageMenuData();
        }
        #endregion
    }
}
