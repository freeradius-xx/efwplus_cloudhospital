﻿using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Linq;
using Microsoft.Practices.Unity;
using System.Collections;
using EFWCoreLib.CoreFrame.Common;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using EFWCoreLib.CoreFrame.EntLib;
using System.Configuration;
using Microsoft.Practices.Unity.Configuration;
using EFWCoreLib.CoreFrame.DbProvider;
using EFWCoreLib.CoreFrame.Init.AttributeManager;
using EFWCoreLib.CoreFrame.Plugin;
using System.Windows.Forms;
using System.Reflection;
using EFWCoreLib.WinformFrame.Controller;


namespace EFWCoreLib.CoreFrame.Init
{
    /// <summary>
    /// 系统启动前初始化环境
    /// </summary>
    public class AppGlobal
    {
        /// <summary>
        /// 程序类型
        /// </summary>
        public static AppType appType;
        /// <summary>
        /// 应用程序根目录，后面不需要跟\\
        /// </summary>
        public static string AppRootPath;

        public static ILoading winfromMain;

        /// <summary>
        /// 是否启动成功
        /// </summary>
        public static bool IsRun = false;
        /// <summary>
        /// 是否Saas模式，where条件是否加workid
        /// </summary>
        public static bool IsSaas = false;

        /// <summary>
        /// 默认Unity对象容器
        /// </summary>
        public static IUnityContainer container;
        /// <summary>
        /// 默认企业库缓存
        /// </summary>
        public static ICacheManager cache;

        /// <summary>
        /// 默认数据库对象
        /// </summary>
        public static AbstractDatabase database;

        /// <summary>
        /// 定制任务
        /// </summary>
        public static List<TimingTask> taskList;

        /// <summary>
        /// 委托代码
        /// </summary>
        public static List<FunClass> codeList;

        /// <summary>
        /// 缺失的程序集dll
        /// </summary>
        public static List<string> missingDll;

        private static bool _isCalled = false;

        private static object locker = new object();

        public static void AppStart()
        {
            lock (locker)
            {
                if (_isCalled == false)
                {
                    try
                    {
                        WriterLog("--------------------------------");
                        WriterLog("应用开始启动！");

                        string  ClientType=System.Configuration.ConfigurationManager.AppSettings["ClientType"];
                        if (ClientType == "Web")
                        {
                            appType = AppType.Web;
                            
                        }
                        else if (ClientType == "Winform")
                        {
                            appType = AppType.Winform;
                            AppRootPath = System.Windows.Forms.Application.StartupPath + "\\";
                        }
                        else if (ClientType == "WCF")
                        {
                            appType = AppType.WCF;
                            AppRootPath = System.Windows.Forms.Application.StartupPath + "\\";
                        }
                        else if (ClientType == "WCFClient")
                        {
                            appType = AppType.WCFClient;
                            AppRootPath = System.Windows.Forms.Application.StartupPath + "\\";
                        }


                        IsSaas = System.Configuration.ConfigurationManager.AppSettings["IsSaas"] == "true" ? true : false;

                        container = ZhyContainer.CreateUnity();
                        cache = ZhyContainer.CreateCache();
                        database = FactoryDatabase.GetDatabase();
                        taskList = new List<TimingTask>();
                        codeList = new List<FunClass>();
                        missingDll = new List<string>();

                        
                        //加载插件
                        AppPluginManage.LoadAllPlugin();
                        //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

                        //下面三个需要配置unity.config
                        //初始化Web定制任务
                        MultiTask.Init(container, taskList);//任务                       
                        //是否开启Web控制器请求权限认证
                        //扩展Global，网站程序启动、停止可自定义代码
                        GlobalExtend.StartInit();
                        //初始化委托代码
                        BaseDelegateCode.Init(container, codeList);//执行函数

                        _isCalled = true;
                      
                        IsRun = true;

                        if (missingDll.Count > 0)
                        {
                            string msg = "缺失的程序集：";
                            WriterLog(msg);
                            for (int i = 0; i < missingDll.Count; i++)
                            {
                                msg = missingDll[i];
                                WriterLog(msg);
                            }
                            //MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        WriterLog("应用启动成功！");
                        WriterLog("--------------------------------");
                        //AppMain();
                    }
                    catch(Exception err)
                    {
                        AppGlobal.WriterLog("应用启动失败！");
                        AppGlobal.WriterLog(err.Message);
                        AppGlobal.WriterLog("--------------------------------");
                        throw err;
                    }
                }
            }
        }

        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);
            string dllname = assemblyName.Name + ".dll";
            string dllpath = null;
            foreach (var p in AppPluginManage.PluginDic)
            {
                if (p.Value.plugin.businessinfoDllList.FindIndex(x => x.name == dllname) != -1)
                {
                    dllpath = p.Value.assemblyPath;
                    break;
                }
            }
            if (dllpath != null)
            {
                FileStream fs = new FileStream(Path.Combine(dllpath, dllname), FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] bFile = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
                return Assembly.Load(bFile);
                //return Assembly.ReflectionOnlyLoadFrom();
            }
            else
                return Assembly.ReflectionOnlyLoad(args.Name);
        }

        public static void AppEnd()
        {
            GlobalExtend.EndInit();

            switch (appType)
            {
                case AppType.Winform:

                    break;
                case AppType.WCFClient:
#if WcfFrame
                    EFWCoreLib.WcfFrame.ClientLinkManage.UnAllConnection();
#endif
                    break;
            }
        }

        public static void WriterLog(string info)
        {
            info = "时间：" + DateTime.Now.ToString() + "\t\t" + "内容：" + info + "\r\n";
            File.AppendAllText(AppRootPath + "startlog.txt", info);
        }

        public static void AppMain()
        {
            FrmSplash frmSplash = new FrmSplash(AppGlobal_Init);
            AppGlobal.winfromMain = frmSplash as ILoading;
            Application.Run(frmSplash);
        }

        public static void AppConfig()
        {
            FrmConfig config = new FrmConfig();
            config.ShowDialog();
        }

        public static void AppExit()
        {
            (winfromMain as Form).Dispose();
        }

        static bool AppGlobal_Init()
        {
            try
            {

                AppGlobal.AppStart();

                if (missingDll.Count > 0)
                {
                    string msg = "缺失的程序集：\r";
                    for (int i = 0; i < missingDll.Count; i++)
                    {
                        msg += missingDll[i] + "\r";
                    }
                    MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                string entryplugin;
                string entrycontroller;
                switch (appType)
                {
                    case AppType.Winform:
#if WinfromFrame
                        PluginSysManage.GetWinformEntry(out entryplugin, out entrycontroller);
                        EFWCoreLib.WinformFrame.Controller.WinformController controller = EFWCoreLib.WinformFrame.Controller.ControllerHelper.CreateController(entryplugin + "@" + entrycontroller);
                        //controller.Init();
                        if (controller == null)
                            throw new Exception("插件配置的启动项（插件名或控制器名称）不正确！");
                        ((System.Windows.Forms.Form)controller.DefaultView).Show();
                        winfromMain.MainForm = ((System.Windows.Forms.Form)controller.DefaultView);
#endif
                        break;
                    case AppType.WCFClient:
#if WcfFrame
                        PluginSysManage.GetWcfClientEntry(out entryplugin, out entrycontroller);
                        WinformController wcfcontroller = ControllerHelper.CreateController(entryplugin + "@" + entrycontroller);
                        if (wcfcontroller == null)
                            throw new Exception("插件配置的启动项（插件名或控制器名称）不正确！！");

                        //EFWCoreLib.WcfFrame.ClientController.ReplyClientCallBack callback = new WcfFrame.ClientController.ReplyClientCallBack();
                        //EFWCoreLib.WcfFrame.ClientController.WcfClientManage.CreateConnection();
                        //wcfcontroller.Init();
                        ((System.Windows.Forms.Form)wcfcontroller.DefaultView).Show();
                        winfromMain.MainForm = ((System.Windows.Forms.Form)wcfcontroller.DefaultView);
#endif
                        break;
                }

                return true;
                
            }
            catch (Exception err)
            {
                //记录错误日志
                ZhyContainer.CreateException().HandleException(err, "HISPolicy");
                //Application.Exit();
                //throw new Exception(err.Message + "\n\n请联系管理员！");
                MessageBox.Show(err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                //AppExit();
                return false;
            }
        }
    }

    public enum AppType
    {
        Web,Winform,WCF,WCFClient
    }

    public interface ILoading
    {
        Form MainForm { get; set; }
    }
}
