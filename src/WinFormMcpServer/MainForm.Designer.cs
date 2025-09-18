namespace WinFormMcpServer;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
	///  Required method for Designer support - do not modify
	///  the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		btnStartStop = new Button();
		txtPort = new TextBox();
		lblPort = new Label();
		lblStatus = new Label();
		txtLog = new TextBox();
		lblLog = new Label();
		lblStatusTitle = new Label();
		TestButton = new Button();
		menuStrip = new MenuStrip();
		configToolStripMenuItem = new ToolStripMenuItem();
		mcpConfigToolStripMenuItem = new ToolStripMenuItem();
		menuStrip.SuspendLayout();
		SuspendLayout();
		// 
		// btnStartStop
		// 
		btnStartStop.Location = new Point(238, 45);
		btnStartStop.Name = "btnStartStop";
		btnStartStop.Size = new Size(113, 36);
		btnStartStop.TabIndex = 0;
		btnStartStop.Text = "启动服务器";
		btnStartStop.UseVisualStyleBackColor = true;
		btnStartStop.Click += btnStartStop_Click;
		// 
		// txtPort
		// 
		txtPort.Location = new Point(102, 50);
		txtPort.Name = "txtPort";
		txtPort.Size = new Size(120, 23);
		txtPort.TabIndex = 1;
		txtPort.Text = "3000";
		// 
		// lblPort
		// 
		lblPort.AutoSize = true;
		lblPort.Location = new Point(31, 53);
		lblPort.Name = "lblPort";
		lblPort.Size = new Size(44, 17);
		lblPort.TabIndex = 2;
		lblPort.Text = "端口：";
		// 
		// lblStatus
		// 
		lblStatus.AutoSize = true;
		lblStatus.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
		lblStatus.ForeColor = Color.Red;
		lblStatus.Location = new Point(473, 54);
		lblStatus.Name = "lblStatus";
		lblStatus.Size = new Size(44, 17);
		lblStatus.TabIndex = 3;
		lblStatus.Text = "已停止";
		// 
		// txtLog
		// 
		txtLog.Location = new Point(31, 113);
		txtLog.Multiline = true;
		txtLog.Name = "txtLog";
		txtLog.ReadOnly = true;
		txtLog.ScrollBars = ScrollBars.Vertical;
		txtLog.Size = new Size(737, 315);
		txtLog.TabIndex = 4;
		// 
		// lblLog
		// 
		lblLog.AutoSize = true;
		lblLog.Location = new Point(31, 93);
		lblLog.Name = "lblLog";
		lblLog.Size = new Size(44, 17);
		lblLog.TabIndex = 5;
		lblLog.Text = "日志：";
		// 
		// lblStatusTitle
		// 
		lblStatusTitle.AutoSize = true;
		lblStatusTitle.Location = new Point(402, 54);
		lblStatusTitle.Name = "lblStatusTitle";
		lblStatusTitle.Size = new Size(44, 17);
		lblStatusTitle.TabIndex = 6;
		lblStatusTitle.Text = "状态：";
		// 
		// TestButton
		// 
		TestButton.Enabled = false;
		TestButton.Location = new Point(579, 45);
		TestButton.Name = "TestButton";
		TestButton.Size = new Size(89, 36);
		TestButton.TabIndex = 7;
		TestButton.Text = "Test";
		TestButton.UseVisualStyleBackColor = true;
		TestButton.Click += TestButton_Click;
		// 
		// menuStrip
		// 
		menuStrip.Items.AddRange(new ToolStripItem[] { configToolStripMenuItem });
		menuStrip.Location = new Point(0, 0);
		menuStrip.Name = "menuStrip";
		menuStrip.Size = new Size(800, 24);
		menuStrip.TabIndex = 8;
		menuStrip.Text = "menuStrip1";
		// 
		// configToolStripMenuItem
		// 
		configToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mcpConfigToolStripMenuItem });
		configToolStripMenuItem.Name = "configToolStripMenuItem";
		configToolStripMenuItem.Size = new Size(44, 20);
		configToolStripMenuItem.Text = "配置";
		// 
		// mcpConfigToolStripMenuItem
		// 
		mcpConfigToolStripMenuItem.Name = "mcpConfigToolStripMenuItem";
		mcpConfigToolStripMenuItem.Size = new Size(128, 22);
		mcpConfigToolStripMenuItem.Text = "MCP 配置";
		mcpConfigToolStripMenuItem.Click += mcpConfigToolStripMenuItem_Click;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(7F, 17F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 450);
		Controls.Add(TestButton);
		Controls.Add(lblStatusTitle);
		Controls.Add(lblLog);
		Controls.Add(txtLog);
		Controls.Add(lblStatus);
		Controls.Add(lblPort);
		Controls.Add(txtPort);
		Controls.Add(btnStartStop);
		Controls.Add(menuStrip);
		MainMenuStrip = menuStrip;
		Name = "MainForm";
		Text = "WinForm MCP Server";
		FormClosing += Form1_FormClosing;
		menuStrip.ResumeLayout(false);
		menuStrip.PerformLayout();
		ResumeLayout(false);
		PerformLayout();
	}

	private Button btnStartStop;
    private TextBox txtPort;
    private Label lblPort;
    private Label lblStatus;
    private TextBox txtLog;
    private Label lblLog;
    private Label lblStatusTitle;
    private Button TestButton;
    private MenuStrip menuStrip;
    private ToolStripMenuItem configToolStripMenuItem;
    private ToolStripMenuItem mcpConfigToolStripMenuItem;

	#endregion
}