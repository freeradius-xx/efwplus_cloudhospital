using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EFWCoreLib.CoreFrame.Init;

namespace WCFHosting
{
    public partial class FrmSetting : Form
    {
        private string entlibconfig = AppGlobal.AppRootPath + "Config\\EntLib.config";
        private string appconfig = AppGlobal.AppRootPath + "efwplusServer.exe.config";
        private XmlDocument xmldoc_entlib;
        private XmlDocument xmldoc_app;
        public bool isOk = false;
        public FrmSetting()
        {
            InitializeComponent();
            appconfig = AppGlobal.AppRootPath + System.IO.Path.GetFileName(Application.ExecutablePath) + ".config";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            HostSettingConfig.SetValue("hostname", txthostname.Text);
            HostSettingConfig.SetValue("debug", ckdebug.Checked ? "1" : "0");
            HostSettingConfig.SetValue("wcfservice", ckwcf.Checked ? "1" : "0");
            HostSettingConfig.SetValue("router", ckrouter.Checked ? "1" : "0");
            HostSettingConfig.SetValue("filetransfer", ckfile.Checked ? "1" : "0");
            HostSettingConfig.SetValue("webapi", ckWebapi.Checked ? "1" : "0");
            HostSettingConfig.SetValue("heartbeat", ckheartbeat.Checked ? "1" : "0");
            HostSettingConfig.SetValue("heartbeattime", txtheartbeattime.Text);
            HostSettingConfig.SetValue("message", ckmessage.Checked ? "1" : "0");
            HostSettingConfig.SetValue("messagetime", txtmessagetime.Text);
            HostSettingConfig.SetValue("compress", ckJsoncompress.Checked ? "1" : "0");
            HostSettingConfig.SetValue("encryption", ckEncryption.Checked ? "1" : "0");
            HostSettingConfig.SetValue("overtime", ckovertime.Checked ? "1" : "0");
            HostSettingConfig.SetValue("overtimetime", txtovertime.Text);
            HostSettingConfig.SaveConfig();

            XmlNode node;
            node = xmldoc_app.DocumentElement.SelectSingleNode("system.serviceModel/services/service[@name='EFWCoreLib.WcfFrame.WcfService.WCFHandlerService']/host/baseAddresses/add");
            if (node != null)
            {
                node.Attributes["baseAddress"].Value = txtwcf.Text;
            }
            node = xmldoc_app.DocumentElement.SelectSingleNode("system.serviceModel/services/service[@name='EFWCoreLib.WcfFrame.WcfService.RouterHandlerService']/host/baseAddresses/add");
            if (node != null)
            {
                node.Attributes["baseAddress"].Value = txtrouter.Text;
            }
            node = xmldoc_app.DocumentElement.SelectSingleNode("system.serviceModel/services/service[@name='EFWCoreLib.WcfFrame.WcfService.FileTransferHandlerService']/host/baseAddresses/add");
            if (node != null)
            {
                node.Attributes["baseAddress"].Value = txtfile.Text;
            }
            node = xmldoc_app.DocumentElement.SelectSingleNode("appSettings/add[@key='WebApiUri']");
            if (node != null)
            {
                node.Attributes["value"].Value = txtweb.Text;
            }


            node = xmldoc_entlib.DocumentElement.SelectSingleNode("connectionStrings");
            if (node != null)
            {
                node.InnerXml = txtconnstr.Text;
            }
            xmldoc_app.Save(appconfig);
            xmldoc_entlib.Save(entlibconfig);

            isOk = true;
            MessageBox.Show("保存参数后，需重启程序才会生效！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmSetting_Load(object sender, EventArgs e)
        {
            txthostname.Text = HostSettingConfig.GetValue("hostname");
            ckdebug.Checked = HostSettingConfig.GetValue("debug") == "1" ? true : false;
            ckwcf.Checked = HostSettingConfig.GetValue("wcfservice") == "1" ? true : false;
            ckrouter.Checked = HostSettingConfig.GetValue("router") == "1" ? true : false;
            ckfile.Checked = HostSettingConfig.GetValue("filetransfer") == "1" ? true : false;
            ckWebapi.Checked = HostSettingConfig.GetValue("webapi") == "1" ? true : false;
            ckheartbeat.Checked = HostSettingConfig.GetValue("heartbeat") == "1" ? true : false;
            txtheartbeattime.Text = HostSettingConfig.GetValue("heartbeattime");
            ckmessage.Checked = HostSettingConfig.GetValue("message") == "1" ? true : false;
            txtmessagetime.Text = HostSettingConfig.GetValue("messagetime");
            ckJsoncompress.Checked = HostSettingConfig.GetValue("compress") == "1" ? true : false;
            ckEncryption.Checked = HostSettingConfig.GetValue("encryption") == "1" ? true : false;
            ckovertime.Checked = HostSettingConfig.GetValue("overtime") == "1" ? true : false;
            txtovertime.Text = HostSettingConfig.GetValue("overtimetime");
            
            
            //读取配置文件
            xmldoc_entlib = new XmlDocument();
            xmldoc_app = new XmlDocument();
            xmldoc_entlib.Load(entlibconfig);
            xmldoc_app.Load(appconfig);

            XmlNode node;
            node = xmldoc_app.DocumentElement.SelectSingleNode("system.serviceModel/services/service[@name='EFWCoreLib.WcfFrame.WcfService.WCFHandlerService']/host/baseAddresses/add");
            if (node != null)
            {
                string address = node.Attributes["baseAddress"].Value;
                txtwcf.Text = address;
            }

            node = xmldoc_app.DocumentElement.SelectSingleNode("system.serviceModel/services/service[@name='EFWCoreLib.WcfFrame.WcfService.RouterHandlerService']/host/baseAddresses/add");
            if (node != null)
            {
                string address = node.Attributes["baseAddress"].Value;
                txtrouter.Text = address;
            }

            node = xmldoc_app.DocumentElement.SelectSingleNode("system.serviceModel/services/service[@name='EFWCoreLib.WcfFrame.WcfService.FileTransferHandlerService']/host/baseAddresses/add");
            if (node != null)
            {
                string address = node.Attributes["baseAddress"].Value;
                txtfile.Text = address;
            }

            node = xmldoc_app.DocumentElement.SelectSingleNode("appSettings/add[@key='WebApiUri']");
            if (node != null)
            {
                string address = node.Attributes["value"].Value;
                txtweb.Text = address;
            }

            node = xmldoc_entlib.DocumentElement.SelectSingleNode("connectionStrings");
            if (node != null)
            {
                string connectionString = node.InnerXml;
                txtconnstr.Text = connectionString;
            }
        }


    }
}
