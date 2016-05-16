using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using EFWCoreLib.CoreFrame.Init;

namespace EFWWeb
{
    public class Global : System.Web.HttpApplication
    {
        //efwplusApi/demo/1
        protected void Application_Start(object sender, EventArgs e)
        {
            //Web系统启动初始化
            AppGlobal.AppRootPath = Context.Server.MapPath("~/");
            AppGlobal.AppStart();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            AppGlobal.AppEnd();
        }

        public override void Init()
        {
            //webapi开启Session
            this.PostAuthenticateRequest += (sender, e) => HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            base.Init();
        }
    }

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //格式化时期
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
                new Newtonsoft.Json.Converters.IsoDateTimeConverter()
                {
                    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
                }
            );

            config.Routes.MapHttpRoute(
                name: "efwplusApi",
                routeTemplate: "efwplusApi/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}