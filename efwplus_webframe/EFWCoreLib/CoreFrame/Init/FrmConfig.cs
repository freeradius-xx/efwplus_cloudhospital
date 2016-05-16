﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace EFWCoreLib.CoreFrame.Init
{
    public partial class FrmConfig : Form
    {
        public bool isOk = false;
        private string entlibconfig = AppGlobal.AppRootPath + "Config\\EntLib.config";
        private string appconfig = AppGlobal.AppRootPath + "EFWWin.exe.config";
        private XmlDocument xmldoc_entlib;
        private XmlDocument xmldoc_app;

        public FrmConfig()
        {
            InitializeComponent();

            appconfig = AppGlobal.AppRootPath + System.IO.Path.GetFileName(Application.ExecutablePath) + ".config";
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (basetabControl.SelectedIndex == 0)
            {
                btnTestConn.Enabled = true;
            }
            else
            {
                btnTestConn.Enabled = false;
            }
        }

        private void btnTestConn_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtconnectionStrings.Text.Trim() == "")
                return;

            XmlNode node = xmldoc_app.DocumentElement.SelectSingleNode("appSettings/add[@key='UpdaterUrl']");
            if (node != null)
            {
                node.Attributes["value"].Value = txtupdate.Text;
            }

            node = xmldoc_app.DocumentElement.SelectSingleNode("system.serviceModel/client/endpoint[@name='myendpoint']");
            if (node != null)
            {
                node.Attributes["address"].Value = txtwcfurl.Text;
            }

            node = xmldoc_app.DocumentElement.SelectSingleNode("system.serviceModel/client/endpoint[@name='fileendpoint']");
            if (node != null)
            {
                node.Attributes["address"].Value = txtfileurl.Text;
            }

            node = xmldoc_entlib.DocumentElement.SelectSingleNode("connectionStrings");
            if (node != null)
            {
                node.InnerXml = txtconnectionStrings.Text;
            }

            xmldoc_app.Save(appconfig);
            xmldoc_entlib.Save(entlibconfig);

            if (MessageBox.Show("修改配置成功，是否重新启动系统？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                isOk = true;
                Application.Restart();
            }
            this.Close();
        }

        private void btnCannel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmConfig_Load(object sender, EventArgs e)
        {
            xmldoc_entlib = new XmlDocument();
            xmldoc_app = new XmlDocument();

            xmldoc_entlib.Load(entlibconfig);
            xmldoc_app.Load(appconfig);

            XmlNode node = xmldoc_app.DocumentElement.SelectSingleNode("appSettings/add[@key='UpdaterUrl']");
            if (node != null)
            {
                string UpdaterUrl = node.Attributes["value"].Value;
                txtupdate.Text = UpdaterUrl;
            }

            node = xmldoc_app.DocumentElement.SelectSingleNode("system.serviceModel/client/endpoint[@name='myendpoint']");
            if (node != null)
            {
                string address = node.Attributes["address"].Value;
                txtwcfurl.Text = address;
            }

            node = xmldoc_app.DocumentElement.SelectSingleNode("system.serviceModel/client/endpoint[@name='fileendpoint']");
            if (node != null)
            {
                string address = node.Attributes["address"].Value;
                txtfileurl.Text = address;
            }

            node = xmldoc_entlib.DocumentElement.SelectSingleNode("connectionStrings");
            if (node != null)
            {
                string connectionString = node.InnerXml;
                txtconnectionStrings.Text = connectionString;
            }
        }
    }
}
