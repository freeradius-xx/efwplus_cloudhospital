using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using EFWCoreLib.CoreFrame.Business;
using EFWCoreLib.CoreFrame.SSO;

namespace EFWCoreLib.WebFrame.WebAPI
{
    public class UserTokenActionFilter : ActionFilterAttribute
    {
        private const string Key = "__user_token__";
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (EFWCoreLib.WcfFrame.ServerController.WcfServerManage.IsDebug == false)
            {
                string token = null;
                string[] qs = actionContext.Request.RequestUri.Query.ToLower().Split(new char[] { '?', '&' });
                foreach (var s in qs)
                {
                    string[] kv = s.Split(new char[] { '=' });
                    if (kv.Length == 2 && kv[0] == "token")
                    {
                        token = kv[1];
                        break;
                    }
                }

                if (token == null)
                    throw new Exception("no token");

                AuthResult result = SsoHelper.ValidateToken(token);
                if (result.ErrorMsg != null)
                    throw new Exception(result.ErrorMsg);


                SysLoginRight loginInfo = new SysLoginRight();
                loginInfo.UserId = Convert.ToInt32(result.User.UserId);
                loginInfo.EmpName = result.User.UserName;

                actionContext.Request.Properties[Key] = loginInfo;
            }
        }
    }
}
