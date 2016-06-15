﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EFWCoreLib.WcfFrame.ServerController;
using EFWCoreLib.CoreFrame.Business.AttributeInfo;
using WinMainUIFrame.Entity;
using WinMainUIFrame.ObjectModel.RightManager;
using EFWCoreLib.WcfFrame.DataSerialize;

namespace WinMainUIFrame.WcfController
{
    [WCFController]
    public class RightController : WcfServerController
    {
        [WCFMethod]
        public ServiceResponseData InitMenuData()
        {
            List<BaseModule> modulelist = NewObject<BaseModule>().getlist<BaseModule>();
            List<BaseMenu> menulist = NewObject<BaseMenu>().getlist<BaseMenu>();

            responseData.AddData(modulelist);
            responseData.AddData(menulist);
            return responseData;
        }
        [WCFMethod]
        public ServiceResponseData NewMenu()
        {
            int selectMenuId = requestData.GetData<int>(0);
            int selectModuleId = requestData.GetData<int>(1);

            BaseMenu menu = NewObject<BaseMenu>();
            menu.PMenuId = selectMenuId;
            menu.ModuleId = selectModuleId;

            responseData.AddData(menu);
            return responseData;
        }
        [WCFMethod]
        public ServiceResponseData SaveMenu()
        {
            BaseMenu menu = requestData.GetData<BaseMenu>(0);
            menu.BindDb(oleDb, _container, _cache, _pluginName);
            menu.save();

            responseData.AddData(true);
            return responseData;
        }
        [WCFMethod]
        public ServiceResponseData DeleteMenu()
        {
            int menuId = requestData.GetData<int>(0);
            NewObject<BaseMenu>().delete(menuId);

            responseData.AddData(true);
            return responseData;
        }

        [WCFMethod]
        public ServiceResponseData InitGroupData()
        {
            List<BaseGroup> grouplist = NewObject<BaseGroup>().getlist<BaseGroup>();
            responseData.AddData(grouplist);
            return responseData;
        }
        [WCFMethod]
        public ServiceResponseData LoadGroupMenuData()
        {
            int groupId = requestData.GetData<int>(0);

            List<BaseModule> modulelist = NewObject<BaseModule>().getlist<BaseModule>();
            List<BaseMenu> menulist = NewObject<BaseMenu>().getlist<BaseMenu>();
            List<BaseMenu> groupmenulist = NewObject<Menu>().GetGroupMenuList(groupId);

            responseData.AddData(modulelist);
            responseData.AddData(menulist);
            responseData.AddData(groupmenulist);
            return responseData;
        }
        [WCFMethod]
        public ServiceResponseData SetGroupMenu()
        {
            int groupId= requestData.GetData<int>(0);
            int[] menuIds = requestData.GetData<int[]>(1);
            NewObject<Group>().SetGroupMenu(groupId, menuIds);

            responseData.AddData(true);
            return responseData;
        }


        #region 页面权限
        [WCFMethod]
        public ServiceResponseData GetPageMenuData()
        {
            int currGroupId = requestData.GetData<int>(0);
            int MenuId = requestData.GetData<int>(1);

            string strsql = @"SELECT Id,Code,Name,
                                        (CASE WHEN (SELECT COUNT(*) FROM dbo.BaseGroupPage WHERE GroupId={0} AND PageId=BasePageMenu.Id)>0 THEN 1 ELSE 0 END) IsUse
                                         FROM BasePageMenu
                                         WHERE menuid={1}";
            strsql = string.Format(strsql, currGroupId, MenuId);
            DataTable dt = oleDb.GetDataTable(strsql);

            requestData.AddData(dt);
            return responseData;
        }

        [WCFMethod]
        public ServiceResponseData SavePageMenu()
        {
            int moduleId = requestData.GetData<int>(0);
            int menuId = requestData.GetData<int>(1);
            string code = requestData.GetData<string>(2);
            string name = requestData.GetData<string>(3);

            string strsql = @" INSERT INTO dbo.BasePageMenu
                                         ( ModuleId ,MenuId ,Code ,Name)
                                         VALUES  ({0},{1},'{2}','{3}')";
            strsql = string.Format(strsql, moduleId, menuId, code, name);
            oleDb.DoCommand(strsql);

            responseData.AddData(true);
            return responseData;
        }
        [WCFMethod]
        public ServiceResponseData DeletePageMenu()
        {
            int pageId = requestData.GetData<int>(0);

            string strsql = @"delete from basepagemenu where id=" + pageId;
            oleDb.DoCommand(strsql);

            responseData.AddData(true);
            return responseData;
        }
        [WCFMethod]
        public ServiceResponseData SetGroupPage()
        {
            int groupId= requestData.GetData<int>(0);
            int pageId = requestData.GetData<int>(1);

            string strsql = @"select count(*) from BaseGroupPage where GroupId={0} and PageId={1}";
            strsql = string.Format(strsql, groupId, pageId);
            if (Convert.ToInt32(oleDb.GetDataResult(strsql)) > 0)
            {
                strsql = @"delete from BaseGroupPage where GroupId={0} and PageId={1}";
                strsql = string.Format(strsql, groupId, pageId);
                oleDb.DoCommand(strsql);
            }
            else
            {
                strsql = @"INSERT INTO BaseGroupPage(GroupId,PageId) VALUES({0},{1})";
                strsql = string.Format(strsql, groupId, pageId);
                oleDb.DoCommand(strsql);
            }

            responseData.AddData(true);
            return responseData;
        }
        #endregion
    }
}
