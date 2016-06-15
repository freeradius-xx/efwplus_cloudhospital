using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management;
using System.Text;
using EFWCoreLib.CoreFrame.Business;
using EFWCoreLib.CoreFrame.Business.AttributeInfo;
using EFWCoreLib.CoreFrame.Common;
using EFWCoreLib.WcfFrame.ServerController;
using EFWCoreLib.WinformFrame.Controller;
using WinMainUIFrame.Entity;
using WinMainUIFrame.ObjectModel.RightManager;
using WinMainUIFrame.ObjectModel.UserLogin;
using EFWCoreLib.CoreFrame.SSO;
using EFWCoreLib.WcfFrame.DataSerialize;

namespace WinMainUIFrame.WcfController
{
    [WCFController]
    public class LoginController : WcfServerController
    {
        [WCFMethod(IsAuthentication=false)]
        public ServiceResponseData UserLogin()
        {
            string usercode = requestData.GetData<string>(0);
            string password = requestData.GetData<string>(1);

            User user = NewObject<User>();
            bool islogin = user.UserLogin(usercode, password);

            if (islogin)
            {
                BaseUser EbaseUser = user.GetUser(usercode);
                SysLoginRight right = new SysLoginRight();
                right.UserId = EbaseUser.UserId;
                right.EmpId = EbaseUser.EmpId;
                right.WorkId = EbaseUser.WorkId;

                Dept dept = NewObject<Dept>();
                BaseDept EbaseDept = dept.GetDefaultDept(EbaseUser.EmpId);
                if (EbaseDept != null)
                {
                    right.DeptId = EbaseDept.DeptId;
                    right.DeptName = EbaseDept.Name;
                }

                BaseEmployee EbaseEmp = (BaseEmployee)NewObject<BaseEmployee>().getmodel(EbaseUser.EmpId);
                right.EmpName = EbaseEmp.Name;

                BaseWorkers EbaseWork = (BaseWorkers)NewObject<BaseWorkers>().getmodel(EbaseUser.WorkId);
                right.WorkName = EbaseWork.WorkName;

                if (EbaseWork.DelFlag == 0)
                {
                    string regkey = EbaseWork.RegKey;
                    DESEncryptor des = new DESEncryptor();
                    des.InputString = regkey;
                    des.DesDecrypt();
                    string[] ret = (des.OutString == null ? "" : des.OutString).Split(new char[] { '|' });
                    if (ret.Length == 2 && ret[0] == EbaseWork.WorkName && Convert.ToDateTime(ret[1]) > DateTime.Now)
                    {
                        //ClientInfo.LoginRight = right;//缓存登录用户信息
                        //单点登录注册
                        Guid token = Guid.Empty;
                        SsoHelper.SignIn(usercode, new UserInfo() { UserId = usercode, UserName = right.EmpName, DeptName = right.DeptName, WorkName = right.WorkName }, out token);


                        responseData.AddData(right.EmpName);
                        responseData.AddData(right.DeptName);
                        responseData.AddData(right.WorkName);
                        responseData.AddData(NewObject<Module>().GetModuleList(right.UserId).OrderBy(x => x.SortId).ToList());
                        responseData.AddData(NewObject<Menu>().GetMenuList(right.UserId));
                        responseData.AddData(NewObject<Dept>().GetHaveDept(right.EmpId));
                        responseData.AddData(right);
                        
                        return responseData;
                    }
                    else
                    {
                        throw new Exception("登录用户的当前机构注册码不正确！");
                    }
                }
                else
                {
                    throw new Exception("登录用户的当前机构还未启用！");
                }
            }
            else
            {
                throw new Exception("输入的用户名密码不正确！");
            }
        }
         [WCFMethod]
        public ServiceResponseData SaveReDept()
        {
            string deptId = requestData.GetData<string>(0);
            string deptName = requestData.GetData<string>(1);
            //ClientInfo.LoginRight.DeptId = Convert.ToInt32(deptId);
            //ClientInfo.LoginRight.DeptName = deptName;

            ServiceResponseData response = new ServiceResponseData();
            response.AddData(true);

            return response;
        }
         [WCFMethod]
        public ServiceResponseData AlterPass()
        {
            string userId = requestData.GetData<string>(0);
            string oldpass = requestData.GetData<string>(1);
            string newpass = requestData.GetData<string>(2);
            bool b = NewObject<User>().AlterPassWrod(Convert.ToInt32(userId), oldpass, newpass);

            responseData.AddData(b);
            return responseData;
        }
         [WCFMethod]
         public ServiceResponseData GetNotReadMessages()
         {
             List<BaseMessage> listmsg;
             string strsql = @"select * from BaseMessage where (Limittime>getdate()) and MessageType in (
																		select Code from BaseMessageType a where  (select count(1) from BaseGroupUser  where GroupId in (a.ReceiveGroup) and userId={0})>0
																		)
                                and (id not in (select messageid from BaseMessageRead where userid={0})) 
                                and (ReceiveWork={2} or ReceiveWork=0)
                                and (ReceiveDept={1} or ReceiveDept=0)
                                and (ReceiveUser={0} or ReceiveUser=0)";
             strsql = string.Format(strsql, LoginUserInfo.UserId, LoginUserInfo.DeptId, LoginUserInfo.WorkId);

             DataTable dt = oleDb.GetDataTable(strsql);
             if (dt.Rows.Count > 0)
                 listmsg = ConvertExtend.ToList<BaseMessage>(dt, oleDb, _container, _cache, _pluginName, null);
             else
                 listmsg = new List<BaseMessage>();


            responseData.AddData(listmsg);
            return responseData;
        }
         [WCFMethod]
         public ServiceResponseData MessageRead()
         {
             int messageId = requestData.GetData<int>(0);

            string strsql = "select count(*) from BaseMessageRead where messageid={0} and userid={1}";
             strsql = string.Format(strsql, messageId, LoginUserInfo.UserId);
             if (Convert.ToInt32(oleDb.GetDataResult(strsql)) == 0)
             {
                 strsql = "insert into BaseMessageRead(messageid,userid,readtime) values(" + messageId + "," + LoginUserInfo.UserId + ",'" + DateTime.Now.Date.ToString() + "')";
                 oleDb.DoCommand(strsql);
             }


            responseData.AddData(true);
            return responseData;
        }
    }
}
