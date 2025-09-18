namespace WinFormMcpServer
{
    partial class ToolsListForm
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
            listViewTools = new ListView();
            columnHeaderToolName = new ColumnHeader();
            columnHeaderDescription = new ColumnHeader();
            groupBoxToolDetails = new GroupBox();
            lblToolName = new Label();
            txtToolName = new TextBox();
            lblToolDescription = new Label();
            txtToolDescription = new TextBox();
            lblServerName = new Label();
            txtServerName = new TextBox();
            lblInputSchema = new Label();
            txtInputSchema = new TextBox();
            btnTestTool = new Button();
            btnRefresh = new Button();
            btnClose = new Button();
            lblStatus = new Label();
            groupBoxToolDetails.SuspendLayout();
            SuspendLayout();
            // 
            // listViewTools
            // 
            listViewTools.Columns.AddRange(new ColumnHeader[] { columnHeaderToolName, columnHeaderDescription });
            listViewTools.FullRowSelect = true;
            listViewTools.GridLines = true;
            listViewTools.Location = new Point(12, 12);
            listViewTools.MultiSelect = false;
            listViewTools.Name = "listViewTools";
            listViewTools.Size = new Size(560, 200);
            listViewTools.TabIndex = 0;
            listViewTools.UseCompatibleStateImageBehavior = false;
            listViewTools.View = View.Details;
            listViewTools.SelectedIndexChanged += listViewTools_SelectedIndexChanged;
            // 
            // columnHeaderToolName
            // 
            columnHeaderToolName.Text = "工具名称";
            columnHeaderToolName.Width = 150;
            // 
            // columnHeaderDescription
            // 
            columnHeaderDescription.Text = "描述";
            columnHeaderDescription.Width = 400;
            // 
            // groupBoxToolDetails
            // 
            groupBoxToolDetails.Controls.Add(lblToolName);
            groupBoxToolDetails.Controls.Add(txtToolName);
            groupBoxToolDetails.Controls.Add(lblToolDescription);
            groupBoxToolDetails.Controls.Add(txtToolDescription);
            groupBoxToolDetails.Controls.Add(lblServerName);
            groupBoxToolDetails.Controls.Add(txtServerName);
            groupBoxToolDetails.Controls.Add(lblInputSchema);
            groupBoxToolDetails.Controls.Add(txtInputSchema);
            groupBoxToolDetails.Location = new Point(12, 230);
            groupBoxToolDetails.Name = "groupBoxToolDetails";
            groupBoxToolDetails.Size = new Size(560, 280);
            groupBoxToolDetails.TabIndex = 1;
            groupBoxToolDetails.TabStop = false;
            groupBoxToolDetails.Text = "工具详情";
            // 
            // lblToolName
            // 
            lblToolName.AutoSize = true;
            lblToolName.Location = new Point(15, 30);
            lblToolName.Name = "lblToolName";
            lblToolName.Size = new Size(68, 20);
            lblToolName.TabIndex = 0;
            lblToolName.Text = "工具名称:";
            // 
            // txtToolName
            // 
            txtToolName.Location = new Point(90, 27);
            txtToolName.Name = "txtToolName";
            txtToolName.ReadOnly = true;
            txtToolName.Size = new Size(450, 27);
            txtToolName.TabIndex = 1;
            // 
            // lblToolDescription
            // 
            lblToolDescription.AutoSize = true;
            lblToolDescription.Location = new Point(15, 70);
            lblToolDescription.Name = "lblToolDescription";
            lblToolDescription.Size = new Size(68, 20);
            lblToolDescription.TabIndex = 2;
            lblToolDescription.Text = "工具描述:";
            // 
            // txtToolDescription
            // 
            txtToolDescription.Location = new Point(90, 67);
            txtToolDescription.Multiline = true;
            txtToolDescription.Name = "txtToolDescription";
            txtToolDescription.ReadOnly = true;
            txtToolDescription.ScrollBars = ScrollBars.Vertical;
            txtToolDescription.Size = new Size(450, 60);
            txtToolDescription.TabIndex = 3;
            // 
            // lblServerName
            // 
            lblServerName.AutoSize = true;
            lblServerName.Location = new Point(15, 140);
            lblServerName.Name = "lblServerName";
            lblServerName.Size = new Size(68, 20);
            lblServerName.TabIndex = 4;
            lblServerName.Text = "服务器名:";
            // 
            // txtServerName
            // 
            txtServerName.Location = new Point(90, 137);
            txtServerName.Name = "txtServerName";
            txtServerName.ReadOnly = true;
            txtServerName.Size = new Size(450, 27);
            txtServerName.TabIndex = 5;
            // 
            // lblInputSchema
            // 
            lblInputSchema.AutoSize = true;
            lblInputSchema.Location = new Point(15, 180);
            lblInputSchema.Name = "lblInputSchema";
            lblInputSchema.Size = new Size(68, 20);
            lblInputSchema.TabIndex = 6;
            lblInputSchema.Text = "输入架构:";
            // 
            // txtInputSchema
            // 
            txtInputSchema.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            txtInputSchema.Location = new Point(90, 177);
            txtInputSchema.Multiline = true;
            txtInputSchema.Name = "txtInputSchema";
            txtInputSchema.ReadOnly = true;
            txtInputSchema.ScrollBars = ScrollBars.Both;
            txtInputSchema.Size = new Size(450, 90);
            txtInputSchema.TabIndex = 7;
            // 
            // btnTestTool
            // 
            btnTestTool.Enabled = false;
            btnTestTool.Location = new Point(590, 230);
            btnTestTool.Name = "btnTestTool";
            btnTestTool.Size = new Size(90, 35);
            btnTestTool.TabIndex = 2;
            btnTestTool.Text = "测试工具";
            btnTestTool.UseVisualStyleBackColor = true;
            btnTestTool.Click += btnTestTool_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(590, 280);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(90, 35);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "刷新";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(590, 475);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(90, 35);
            btnClose.TabIndex = 4;
            btnClose.Text = "关闭";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(12, 525);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(39, 20);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "就绪";
            // 
            // ToolsListForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 560);
            Controls.Add(lblStatus);
            Controls.Add(btnClose);
            Controls.Add(btnRefresh);
            Controls.Add(btnTestTool);
            Controls.Add(groupBoxToolDetails);
            Controls.Add(listViewTools);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ToolsListForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "工具列表";
            groupBoxToolDetails.ResumeLayout(false);
            groupBoxToolDetails.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView listViewTools;
        private ColumnHeader columnHeaderToolName;
        private ColumnHeader columnHeaderDescription;
        private GroupBox groupBoxToolDetails;
        private Label lblToolName;
        private TextBox txtToolName;
        private Label lblToolDescription;
        private TextBox txtToolDescription;
        private Label lblServerName;
        private TextBox txtServerName;
        private Label lblInputSchema;
        private TextBox txtInputSchema;
        private Button btnTestTool;
        private Button btnRefresh;
        private Button btnClose;
        private Label lblStatus;
    }
}