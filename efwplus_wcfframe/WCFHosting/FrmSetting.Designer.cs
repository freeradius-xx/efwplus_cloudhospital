﻿namespace WCFHosting
{
    partial class FrmSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSetting));
            this.ckdebug = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ckrouter = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txthostname = new System.Windows.Forms.TextBox();
            this.ckwcf = new System.Windows.Forms.CheckBox();
            this.ckheartbeat = new System.Windows.Forms.CheckBox();
            this.ckJsoncompress = new System.Windows.Forms.CheckBox();
            this.ckEncryption = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtheartbeattime = new System.Windows.Forms.TextBox();
            this.txtmessagetime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ckmessage = new System.Windows.Forms.CheckBox();
            this.txtovertime = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ckovertime = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ckWebapi = new System.Windows.Forms.CheckBox();
            this.ckfile = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtfilerouter = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtweb = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtfile = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtrouter = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtwcf = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.txtconnstr = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.txtfileurl = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtwcfurl = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckdebug
            // 
            this.ckdebug.AutoSize = true;
            this.ckdebug.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckdebug.Location = new System.Drawing.Point(87, 51);
            this.ckdebug.Name = "ckdebug";
            this.ckdebug.Size = new System.Drawing.Size(99, 21);
            this.ckdebug.TabIndex = 0;
            this.ckdebug.Text = "显示调试信息";
            this.ckdebug.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(251, 21);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(343, 21);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ckrouter
            // 
            this.ckrouter.AutoSize = true;
            this.ckrouter.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckrouter.Location = new System.Drawing.Point(238, 73);
            this.ckrouter.Name = "ckrouter";
            this.ckrouter.Size = new System.Drawing.Size(99, 21);
            this.ckrouter.TabIndex = 3;
            this.ckrouter.Text = "开启路由服务";
            this.ckrouter.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(6, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "中间件名称：";
            // 
            // txthostname
            // 
            this.txthostname.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txthostname.ForeColor = System.Drawing.Color.Blue;
            this.txthostname.Location = new System.Drawing.Point(87, 11);
            this.txthostname.Name = "txthostname";
            this.txthostname.Size = new System.Drawing.Size(322, 23);
            this.txthostname.TabIndex = 5;
            // 
            // ckwcf
            // 
            this.ckwcf.AutoSize = true;
            this.ckwcf.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckwcf.Location = new System.Drawing.Point(87, 73);
            this.ckwcf.Name = "ckwcf";
            this.ckwcf.Size = new System.Drawing.Size(99, 21);
            this.ckwcf.TabIndex = 6;
            this.ckwcf.Text = "开启基础服务";
            this.ckwcf.UseVisualStyleBackColor = true;
            // 
            // ckheartbeat
            // 
            this.ckheartbeat.AutoSize = true;
            this.ckheartbeat.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckheartbeat.Location = new System.Drawing.Point(87, 139);
            this.ckheartbeat.Name = "ckheartbeat";
            this.ckheartbeat.Size = new System.Drawing.Size(123, 21);
            this.ckheartbeat.TabIndex = 7;
            this.ckheartbeat.Text = "开启心跳检测功能";
            this.ckheartbeat.UseVisualStyleBackColor = true;
            // 
            // ckJsoncompress
            // 
            this.ckJsoncompress.AutoSize = true;
            this.ckJsoncompress.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckJsoncompress.Location = new System.Drawing.Point(87, 205);
            this.ckJsoncompress.Name = "ckJsoncompress";
            this.ckJsoncompress.Size = new System.Drawing.Size(101, 21);
            this.ckJsoncompress.TabIndex = 8;
            this.ckJsoncompress.Text = "开启Json压缩";
            this.ckJsoncompress.UseVisualStyleBackColor = true;
            // 
            // ckEncryption
            // 
            this.ckEncryption.AutoSize = true;
            this.ckEncryption.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckEncryption.Location = new System.Drawing.Point(87, 227);
            this.ckEncryption.Name = "ckEncryption";
            this.ckEncryption.Size = new System.Drawing.Size(99, 21);
            this.ckEncryption.TabIndex = 9;
            this.ckEncryption.Text = "开启数据加密";
            this.ckEncryption.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(235, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "间隔时间(秒)";
            // 
            // txtheartbeattime
            // 
            this.txtheartbeattime.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtheartbeattime.Location = new System.Drawing.Point(315, 138);
            this.txtheartbeattime.Name = "txtheartbeattime";
            this.txtheartbeattime.Size = new System.Drawing.Size(56, 23);
            this.txtheartbeattime.TabIndex = 11;
            this.txtheartbeattime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtmessagetime
            // 
            this.txtmessagetime.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtmessagetime.Location = new System.Drawing.Point(315, 162);
            this.txtmessagetime.Name = "txtmessagetime";
            this.txtmessagetime.Size = new System.Drawing.Size(56, 23);
            this.txtmessagetime.TabIndex = 14;
            this.txtmessagetime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(235, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "间隔时间(秒)";
            // 
            // ckmessage
            // 
            this.ckmessage.AutoSize = true;
            this.ckmessage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckmessage.Location = new System.Drawing.Point(87, 161);
            this.ckmessage.Name = "ckmessage";
            this.ckmessage.Size = new System.Drawing.Size(99, 21);
            this.ckmessage.TabIndex = 12;
            this.ckmessage.Text = "开启消息发送";
            this.ckmessage.UseVisualStyleBackColor = true;
            // 
            // txtovertime
            // 
            this.txtovertime.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtovertime.Location = new System.Drawing.Point(315, 186);
            this.txtovertime.Name = "txtovertime";
            this.txtovertime.Size = new System.Drawing.Size(56, 23);
            this.txtovertime.TabIndex = 17;
            this.txtovertime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(235, 187);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 17);
            this.label4.TabIndex = 16;
            this.label4.Text = "超过时间(秒)";
            // 
            // ckovertime
            // 
            this.ckovertime.AutoSize = true;
            this.ckovertime.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckovertime.Location = new System.Drawing.Point(87, 183);
            this.ckovertime.Name = "ckovertime";
            this.ckovertime.Size = new System.Drawing.Size(147, 21);
            this.ckovertime.TabIndex = 15;
            this.ckovertime.Text = "开启日志记录耗时方法";
            this.ckovertime.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(430, 282);
            this.tabControl1.TabIndex = 18;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ckWebapi);
            this.tabPage1.Controls.Add(this.ckfile);
            this.tabPage1.Controls.Add(this.ckdebug);
            this.tabPage1.Controls.Add(this.txtovertime);
            this.tabPage1.Controls.Add(this.ckrouter);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.ckovertime);
            this.tabPage1.Controls.Add(this.txthostname);
            this.tabPage1.Controls.Add(this.txtmessagetime);
            this.tabPage1.Controls.Add(this.ckwcf);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.ckheartbeat);
            this.tabPage1.Controls.Add(this.ckmessage);
            this.tabPage1.Controls.Add(this.ckJsoncompress);
            this.tabPage1.Controls.Add(this.txtheartbeattime);
            this.tabPage1.Controls.Add(this.ckEncryption);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(422, 256);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "基本参数";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ckWebapi
            // 
            this.ckWebapi.AutoSize = true;
            this.ckWebapi.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckWebapi.Location = new System.Drawing.Point(238, 95);
            this.ckWebapi.Name = "ckWebapi";
            this.ckWebapi.Size = new System.Drawing.Size(121, 21);
            this.ckWebapi.TabIndex = 19;
            this.ckWebapi.Text = "开启WebAPI服务";
            this.ckWebapi.UseVisualStyleBackColor = true;
            // 
            // ckfile
            // 
            this.ckfile.AutoSize = true;
            this.ckfile.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckfile.Location = new System.Drawing.Point(87, 95);
            this.ckfile.Name = "ckfile";
            this.ckfile.Size = new System.Drawing.Size(123, 21);
            this.ckfile.TabIndex = 18;
            this.ckfile.Text = "开启文件传输服务";
            this.ckfile.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtfilerouter);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.txtweb);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.txtfile);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.txtrouter);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.txtwcf);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(422, 256);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "发布服务地址";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtfilerouter
            // 
            this.txtfilerouter.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtfilerouter.ForeColor = System.Drawing.Color.Blue;
            this.txtfilerouter.Location = new System.Drawing.Point(10, 170);
            this.txtfilerouter.Name = "txtfilerouter";
            this.txtfilerouter.Size = new System.Drawing.Size(404, 23);
            this.txtfilerouter.TabIndex = 9;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(8, 152);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(116, 17);
            this.label10.TabIndex = 8;
            this.label10.Text = "路由文件服务地址：";
            // 
            // txtweb
            // 
            this.txtweb.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtweb.ForeColor = System.Drawing.Color.Blue;
            this.txtweb.Location = new System.Drawing.Point(9, 214);
            this.txtweb.Name = "txtweb";
            this.txtweb.Size = new System.Drawing.Size(404, 23);
            this.txtweb.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(7, 196);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(114, 17);
            this.label8.TabIndex = 6;
            this.label8.Text = "WebAPI服务地址：";
            // 
            // txtfile
            // 
            this.txtfile.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtfile.ForeColor = System.Drawing.Color.Blue;
            this.txtfile.Location = new System.Drawing.Point(10, 82);
            this.txtfile.Name = "txtfile";
            this.txtfile.Size = new System.Drawing.Size(404, 23);
            this.txtfile.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(8, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(116, 17);
            this.label7.TabIndex = 4;
            this.label7.Text = "文件传输服务地址：";
            // 
            // txtrouter
            // 
            this.txtrouter.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtrouter.ForeColor = System.Drawing.Color.Blue;
            this.txtrouter.Location = new System.Drawing.Point(10, 126);
            this.txtrouter.Name = "txtrouter";
            this.txtrouter.Size = new System.Drawing.Size(404, 23);
            this.txtrouter.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(8, 108);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 17);
            this.label6.TabIndex = 2;
            this.label6.Text = "路由基础服务地址：";
            // 
            // txtwcf
            // 
            this.txtwcf.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtwcf.ForeColor = System.Drawing.Color.Blue;
            this.txtwcf.Location = new System.Drawing.Point(10, 38);
            this.txtwcf.Name = "txtwcf";
            this.txtwcf.Size = new System.Drawing.Size(404, 23);
            this.txtwcf.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(8, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "基础数据服务地址：";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.txtconnstr);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(422, 256);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "数据库连接";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // txtconnstr
            // 
            this.txtconnstr.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtconnstr.ForeColor = System.Drawing.Color.Blue;
            this.txtconnstr.Location = new System.Drawing.Point(9, 36);
            this.txtconnstr.Multiline = true;
            this.txtconnstr.Name = "txtconnstr";
            this.txtconnstr.Size = new System.Drawing.Size(404, 206);
            this.txtconnstr.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(7, 18);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 17);
            this.label9.TabIndex = 2;
            this.label9.Text = "连接字符串：";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.txtfileurl);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Controls.Add(this.txtwcfurl);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(422, 256);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "通讯连接地址";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // txtfileurl
            // 
            this.txtfileurl.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtfileurl.Location = new System.Drawing.Point(9, 92);
            this.txtfileurl.Name = "txtfileurl";
            this.txtfileurl.Size = new System.Drawing.Size(405, 23);
            this.txtfileurl.TabIndex = 8;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(6, 72);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(92, 17);
            this.label11.TabIndex = 7;
            this.label11.Text = "文件传输地址：";
            // 
            // txtwcfurl
            // 
            this.txtwcfurl.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtwcfurl.Location = new System.Drawing.Point(9, 40);
            this.txtwcfurl.Name = "txtwcfurl";
            this.txtwcfurl.Size = new System.Drawing.Size(405, 23);
            this.txtwcfurl.TabIndex = 6;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(6, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(92, 17);
            this.label12.TabIndex = 5;
            this.label12.Text = "业务请求地址：";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 282);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(430, 61);
            this.panel1.TabIndex = 19;
            // 
            // FrmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 343);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSetting";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "系统设置";
            this.Load += new System.EventHandler(this.FrmSetting_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox ckdebug;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox ckrouter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txthostname;
        private System.Windows.Forms.CheckBox ckwcf;
        private System.Windows.Forms.CheckBox ckheartbeat;
        private System.Windows.Forms.CheckBox ckJsoncompress;
        private System.Windows.Forms.CheckBox ckEncryption;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtheartbeattime;
        private System.Windows.Forms.TextBox txtmessagetime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ckmessage;
        private System.Windows.Forms.TextBox txtovertime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox ckovertime;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox txtwcf;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtweb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtfile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtrouter;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtconnstr;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox ckfile;
        private System.Windows.Forms.CheckBox ckWebapi;
        private System.Windows.Forms.TextBox txtfilerouter;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox txtfileurl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtwcfurl;
        private System.Windows.Forms.Label label12;
    }
}