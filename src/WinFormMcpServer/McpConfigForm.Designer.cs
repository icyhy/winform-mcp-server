namespace WinFormMcpServer
{
    partial class McpConfigForm
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
            listViewServers = new ListView();
            columnHeaderName = new ColumnHeader();
            columnHeaderType = new ColumnHeader();
            columnHeaderUrl = new ColumnHeader();
            columnHeaderStatus = new ColumnHeader();
            columnHeaderEnabled = new ColumnHeader();
            btnRefresh = new Button();
            btnConnectAll = new Button();
            btnDisconnectAll = new Button();
            btnManualConfig = new Button();
            btnClose = new Button();
            groupBoxServerDetails = new GroupBox();
            lblServerName = new Label();
            txtServerName = new TextBox();
            lblServerType = new Label();
            txtServerType = new TextBox();
            lblServerUrl = new Label();
            txtServerUrl = new TextBox();
            lblServerNote = new Label();
            txtServerNote = new TextBox();
            chkServerEnabled = new CheckBox();
            btnConnect = new Button();
            btnDisconnect = new Button();
            lblConnectionStatus = new Label();
            lblStatusValue = new Label();
            groupBoxTools = new GroupBox();
            listViewTools = new ListView();
            columnHeaderToolName = new ColumnHeader();
            columnHeaderToolDescription = new ColumnHeader();
            btnTestTool = new Button();
            groupBoxServerDetails.SuspendLayout();
            groupBoxTools.SuspendLayout();
            SuspendLayout();
            // 
            // listViewServers
            // 
            listViewServers.Columns.AddRange(new ColumnHeader[] { columnHeaderName, columnHeaderType, columnHeaderUrl, columnHeaderStatus, columnHeaderEnabled });
            listViewServers.FullRowSelect = true;
            listViewServers.GridLines = true;
            listViewServers.Location = new Point(12, 12);
            listViewServers.MultiSelect = false;
            listViewServers.Name = "listViewServers";
            listViewServers.Size = new Size(760, 200);
            listViewServers.TabIndex = 0;
            listViewServers.UseCompatibleStateImageBehavior = false;
            listViewServers.View = View.Details;
            listViewServers.SelectedIndexChanged += listViewServers_SelectedIndexChanged;
            // 
            // columnHeaderName
            // 
            columnHeaderName.Text = "服务器名称";
            columnHeaderName.Width = 120;
            // 
            // columnHeaderType
            // 
            columnHeaderType.Text = "类型";
            columnHeaderType.Width = 80;
            // 
            // columnHeaderUrl
            // 
            columnHeaderUrl.Text = "URL";
            columnHeaderUrl.Width = 300;
            // 
            // columnHeaderStatus
            // 
            columnHeaderStatus.Text = "状态";
            columnHeaderStatus.Width = 100;
            // 
            // columnHeaderEnabled
            // 
            columnHeaderEnabled.Text = "启用";
            columnHeaderEnabled.Width = 60;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(12, 218);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(75, 30);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "刷新";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnConnectAll
            // 
            btnConnectAll.Location = new Point(93, 218);
            btnConnectAll.Name = "btnConnectAll";
            btnConnectAll.Size = new Size(100, 30);
            btnConnectAll.TabIndex = 2;
            btnConnectAll.Text = "连接所有";
            btnConnectAll.UseVisualStyleBackColor = true;
            btnConnectAll.Click += btnConnectAll_Click;
            // 
            // btnDisconnectAll
            // 
            btnDisconnectAll.Location = new Point(199, 218);
            btnDisconnectAll.Name = "btnDisconnectAll";
            btnDisconnectAll.Size = new Size(100, 30);
            btnDisconnectAll.TabIndex = 3;
            btnDisconnectAll.Text = "断开所有";
            btnDisconnectAll.UseVisualStyleBackColor = true;
            btnDisconnectAll.Click += btnDisconnectAll_Click;
            // 
            // btnManualConfig
            // 
            btnManualConfig.Location = new Point(305, 218);
            btnManualConfig.Name = "btnManualConfig";
            btnManualConfig.Size = new Size(100, 30);
            btnManualConfig.TabIndex = 4;
            btnManualConfig.Text = "手动配置";
            btnManualConfig.UseVisualStyleBackColor = true;
            btnManualConfig.Click += btnManualConfig_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(697, 218);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 30);
            btnClose.TabIndex = 5;
            btnClose.Text = "关闭";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // groupBoxServerDetails
            // 
            groupBoxServerDetails.Controls.Add(lblStatusValue);
            groupBoxServerDetails.Controls.Add(lblConnectionStatus);
            groupBoxServerDetails.Controls.Add(btnDisconnect);
            groupBoxServerDetails.Controls.Add(btnConnect);
            groupBoxServerDetails.Controls.Add(chkServerEnabled);
            groupBoxServerDetails.Controls.Add(txtServerNote);
            groupBoxServerDetails.Controls.Add(lblServerNote);
            groupBoxServerDetails.Controls.Add(txtServerUrl);
            groupBoxServerDetails.Controls.Add(lblServerUrl);
            groupBoxServerDetails.Controls.Add(txtServerType);
            groupBoxServerDetails.Controls.Add(lblServerType);
            groupBoxServerDetails.Controls.Add(txtServerName);
            groupBoxServerDetails.Controls.Add(lblServerName);
            groupBoxServerDetails.Location = new Point(12, 254);
            groupBoxServerDetails.Name = "groupBoxServerDetails";
            groupBoxServerDetails.Size = new Size(380, 280);
            groupBoxServerDetails.TabIndex = 5;
            groupBoxServerDetails.TabStop = false;
            groupBoxServerDetails.Text = "服务器详情";
            // 
            // lblServerName
            // 
            lblServerName.AutoSize = true;
            lblServerName.Location = new Point(15, 30);
            lblServerName.Name = "lblServerName";
            lblServerName.Size = new Size(68, 17);
            lblServerName.TabIndex = 0;
            lblServerName.Text = "服务器名称:";
            // 
            // txtServerName
            // 
            txtServerName.Location = new Point(90, 27);
            txtServerName.Name = "txtServerName";
            txtServerName.ReadOnly = true;
            txtServerName.Size = new Size(270, 23);
            txtServerName.TabIndex = 1;
            // 
            // lblServerType
            // 
            lblServerType.AutoSize = true;
            lblServerType.Location = new Point(15, 65);
            lblServerType.Name = "lblServerType";
            lblServerType.Size = new Size(32, 17);
            lblServerType.TabIndex = 2;
            lblServerType.Text = "类型:";
            // 
            // txtServerType
            // 
            txtServerType.Location = new Point(90, 62);
            txtServerType.Name = "txtServerType";
            txtServerType.ReadOnly = true;
            txtServerType.Size = new Size(270, 23);
            txtServerType.TabIndex = 3;
            // 
            // lblServerUrl
            // 
            lblServerUrl.AutoSize = true;
            lblServerUrl.Location = new Point(15, 100);
            lblServerUrl.Name = "lblServerUrl";
            lblServerUrl.Size = new Size(35, 17);
            lblServerUrl.TabIndex = 4;
            lblServerUrl.Text = "URL:";
            // 
            // txtServerUrl
            // 
            txtServerUrl.Location = new Point(90, 97);
            txtServerUrl.Name = "txtServerUrl";
            txtServerUrl.ReadOnly = true;
            txtServerUrl.Size = new Size(270, 23);
            txtServerUrl.TabIndex = 5;
            // 
            // lblServerNote
            // 
            lblServerNote.AutoSize = true;
            lblServerNote.Location = new Point(15, 135);
            lblServerNote.Name = "lblServerNote";
            lblServerNote.Size = new Size(32, 17);
            lblServerNote.TabIndex = 6;
            lblServerNote.Text = "备注:";
            // 
            // txtServerNote
            // 
            txtServerNote.Location = new Point(90, 132);
            txtServerNote.Multiline = true;
            txtServerNote.Name = "txtServerNote";
            txtServerNote.ReadOnly = true;
            txtServerNote.Size = new Size(270, 60);
            txtServerNote.TabIndex = 7;
            // 
            // chkServerEnabled
            // 
            chkServerEnabled.AutoSize = true;
            chkServerEnabled.Location = new Point(90, 205);
            chkServerEnabled.Name = "chkServerEnabled";
            chkServerEnabled.Size = new Size(51, 21);
            chkServerEnabled.TabIndex = 8;
            chkServerEnabled.Text = "启用";
            chkServerEnabled.UseVisualStyleBackColor = true;
            chkServerEnabled.CheckedChanged += chkServerEnabled_CheckedChanged;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(90, 240);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 30);
            btnConnect.TabIndex = 9;
            btnConnect.Text = "连接";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Location = new Point(180, 240);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(75, 30);
            btnDisconnect.TabIndex = 10;
            btnDisconnect.Text = "断开";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // lblConnectionStatus
            // 
            lblConnectionStatus.AutoSize = true;
            lblConnectionStatus.Location = new Point(270, 205);
            lblConnectionStatus.Name = "lblConnectionStatus";
            lblConnectionStatus.Size = new Size(56, 17);
            lblConnectionStatus.TabIndex = 11;
            lblConnectionStatus.Text = "连接状态:";
            // 
            // lblStatusValue
            // 
            lblStatusValue.AutoSize = true;
            lblStatusValue.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            lblStatusValue.ForeColor = Color.Red;
            lblStatusValue.Location = new Point(270, 225);
            lblStatusValue.Name = "lblStatusValue";
            lblStatusValue.Size = new Size(44, 17);
            lblStatusValue.TabIndex = 12;
            lblStatusValue.Text = "未连接";
            // 
            // groupBoxTools
            // 
            groupBoxTools.Controls.Add(btnTestTool);
            groupBoxTools.Controls.Add(listViewTools);
            groupBoxTools.Location = new Point(398, 254);
            groupBoxTools.Name = "groupBoxTools";
            groupBoxTools.Size = new Size(374, 280);
            groupBoxTools.TabIndex = 6;
            groupBoxTools.TabStop = false;
            groupBoxTools.Text = "可用工具";
            // 
            // listViewTools
            // 
            listViewTools.Columns.AddRange(new ColumnHeader[] { columnHeaderToolName, columnHeaderToolDescription });
            listViewTools.FullRowSelect = true;
            listViewTools.GridLines = true;
            listViewTools.Location = new Point(15, 27);
            listViewTools.MultiSelect = false;
            listViewTools.Name = "listViewTools";
            listViewTools.Size = new Size(344, 210);
            listViewTools.TabIndex = 0;
            listViewTools.UseCompatibleStateImageBehavior = false;
            listViewTools.View = View.Details;
            listViewTools.SelectedIndexChanged += listViewTools_SelectedIndexChanged;
            // 
            // columnHeaderToolName
            // 
            columnHeaderToolName.Text = "工具名称";
            columnHeaderToolName.Width = 120;
            // 
            // columnHeaderToolDescription
            // 
            columnHeaderToolDescription.Text = "描述";
            columnHeaderToolDescription.Width = 200;
            // 
            // btnTestTool
            // 
            btnTestTool.Enabled = false;
            btnTestTool.Location = new Point(284, 243);
            btnTestTool.Name = "btnTestTool";
            btnTestTool.Size = new Size(75, 30);
            btnTestTool.TabIndex = 1;
            btnTestTool.Text = "测试工具";
            btnTestTool.UseVisualStyleBackColor = true;
            btnTestTool.Click += btnTestTool_Click;
            // 
            // McpConfigForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 546);
            Controls.Add(groupBoxTools);
            Controls.Add(groupBoxServerDetails);
            Controls.Add(btnClose);
            Controls.Add(btnManualConfig);
            Controls.Add(btnDisconnectAll);
            Controls.Add(btnConnectAll);
            Controls.Add(btnRefresh);
            Controls.Add(listViewServers);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "McpConfigForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "MCP 服务器配置";
            Load += McpConfigForm_Load;
            groupBoxServerDetails.ResumeLayout(false);
            groupBoxServerDetails.PerformLayout();
            groupBoxTools.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListView listViewServers;
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderType;
        private ColumnHeader columnHeaderUrl;
        private ColumnHeader columnHeaderStatus;
        private ColumnHeader columnHeaderEnabled;
        private Button btnRefresh;
        private Button btnConnectAll;
        private Button btnDisconnectAll;
        private Button btnManualConfig;
        private Button btnClose;
        private GroupBox groupBoxServerDetails;
        private Label lblServerName;
        private TextBox txtServerName;
        private Label lblServerType;
        private TextBox txtServerType;
        private Label lblServerUrl;
        private TextBox txtServerUrl;
        private Label lblServerNote;
        private TextBox txtServerNote;
        private CheckBox chkServerEnabled;
        private Button btnConnect;
        private Button btnDisconnect;
        private Label lblConnectionStatus;
        private Label lblStatusValue;
        private GroupBox groupBoxTools;
        private ListView listViewTools;
        private ColumnHeader columnHeaderToolName;
        private ColumnHeader columnHeaderToolDescription;
        private Button btnTestTool;
    }
}