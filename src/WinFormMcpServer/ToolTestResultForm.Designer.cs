namespace WinFormMcpServer
{
    partial class ToolTestResultForm
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
            lblToolName = new Label();
            txtResult = new TextBox();
            btnCopy = new Button();
            btnSave = new Button();
            btnClose = new Button();
            SuspendLayout();
            // 
            // lblToolName
            // 
            lblToolName.AutoSize = true;
            lblToolName.Font = new Font("Microsoft YaHei UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point);
            lblToolName.Location = new Point(12, 15);
            lblToolName.Name = "lblToolName";
            lblToolName.Size = new Size(54, 24);
            lblToolName.TabIndex = 0;
            lblToolName.Text = "工具:";
            // 
            // txtResult
            // 
            txtResult.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            txtResult.Location = new Point(12, 50);
            txtResult.Multiline = true;
            txtResult.Name = "txtResult";
            txtResult.ReadOnly = true;
            txtResult.ScrollBars = ScrollBars.Both;
            txtResult.Size = new Size(660, 400);
            txtResult.TabIndex = 1;
            // 
            // btnCopy
            // 
            btnCopy.Location = new Point(400, 470);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(80, 35);
            btnCopy.TabIndex = 2;
            btnCopy.Text = "复制";
            btnCopy.UseVisualStyleBackColor = true;
            btnCopy.Click += btnCopy_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(500, 470);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(80, 35);
            btnSave.TabIndex = 3;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(592, 470);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(80, 35);
            btnClose.TabIndex = 4;
            btnClose.Text = "关闭";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // ToolTestResultForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(684, 521);
            Controls.Add(btnClose);
            Controls.Add(btnSave);
            Controls.Add(btnCopy);
            Controls.Add(txtResult);
            Controls.Add(lblToolName);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ToolTestResultForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "测试结果";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblToolName;
        private TextBox txtResult;
        private Button btnCopy;
        private Button btnSave;
        private Button btnClose;
    }
}