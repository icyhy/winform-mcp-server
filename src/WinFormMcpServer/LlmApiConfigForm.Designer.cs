namespace WinFormMcpServer;

partial class LlmApiConfigForm
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
        groupBoxApiType = new GroupBox();
        radioButtonRealApi = new RadioButton();
        radioButtonMockApi = new RadioButton();
        groupBoxApiSettings = new GroupBox();
        numericUpDownTemperature = new NumericUpDown();
        labelTemperature = new Label();
        numericUpDownMaxTokens = new NumericUpDown();
        labelMaxTokens = new Label();
        numericUpDownTimeout = new NumericUpDown();
        labelTimeout = new Label();
        textBoxModelName = new TextBox();
        labelModelName = new Label();
        textBoxApiKey = new TextBox();
        labelApiKey = new Label();
        textBoxBaseUrl = new TextBox();
        labelBaseUrl = new Label();
        buttonOK = new Button();
        buttonCancel = new Button();
        buttonTest = new Button();
        buttonReset = new Button();
        groupBoxApiType.SuspendLayout();
        groupBoxApiSettings.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDownTemperature).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericUpDownMaxTokens).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericUpDownTimeout).BeginInit();
        SuspendLayout();
        // 
        // groupBoxApiType
        // 
        groupBoxApiType.Controls.Add(radioButtonRealApi);
        groupBoxApiType.Controls.Add(radioButtonMockApi);
        groupBoxApiType.Location = new Point(12, 12);
        groupBoxApiType.Name = "groupBoxApiType";
        groupBoxApiType.Size = new Size(460, 60);
        groupBoxApiType.TabIndex = 0;
        groupBoxApiType.TabStop = false;
        groupBoxApiType.Text = "API类型";
        // 
        // radioButtonRealApi
        // 
        radioButtonRealApi.AutoSize = true;
        radioButtonRealApi.Location = new Point(150, 25);
        radioButtonRealApi.Name = "radioButtonRealApi";
        radioButtonRealApi.Size = new Size(86, 21);
        radioButtonRealApi.TabIndex = 1;
        radioButtonRealApi.Text = "真实LLM API";
        radioButtonRealApi.UseVisualStyleBackColor = true;
        radioButtonRealApi.CheckedChanged += radioButtonRealApi_CheckedChanged;
        // 
        // radioButtonMockApi
        // 
        radioButtonMockApi.AutoSize = true;
        radioButtonMockApi.Checked = true;
        radioButtonMockApi.Location = new Point(20, 25);
        radioButtonMockApi.Name = "radioButtonMockApi";
        radioButtonMockApi.Size = new Size(78, 21);
        radioButtonMockApi.TabIndex = 0;
        radioButtonMockApi.TabStop = true;
        radioButtonMockApi.Text = "Mock API";
        radioButtonMockApi.UseVisualStyleBackColor = true;
        // 
        // groupBoxApiSettings
        // 
        groupBoxApiSettings.Controls.Add(numericUpDownTemperature);
        groupBoxApiSettings.Controls.Add(labelTemperature);
        groupBoxApiSettings.Controls.Add(numericUpDownMaxTokens);
        groupBoxApiSettings.Controls.Add(labelMaxTokens);
        groupBoxApiSettings.Controls.Add(numericUpDownTimeout);
        groupBoxApiSettings.Controls.Add(labelTimeout);
        groupBoxApiSettings.Controls.Add(textBoxModelName);
        groupBoxApiSettings.Controls.Add(labelModelName);
        groupBoxApiSettings.Controls.Add(textBoxApiKey);
        groupBoxApiSettings.Controls.Add(labelApiKey);
        groupBoxApiSettings.Controls.Add(textBoxBaseUrl);
        groupBoxApiSettings.Controls.Add(labelBaseUrl);
        groupBoxApiSettings.Enabled = false;
        groupBoxApiSettings.Location = new Point(12, 78);
        groupBoxApiSettings.Name = "groupBoxApiSettings";
        groupBoxApiSettings.Size = new Size(460, 280);
        groupBoxApiSettings.TabIndex = 1;
        groupBoxApiSettings.TabStop = false;
        groupBoxApiSettings.Text = "API设置";
        // 
        // numericUpDownTemperature
        // 
        numericUpDownTemperature.DecimalPlaces = 1;
        numericUpDownTemperature.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        numericUpDownTemperature.Location = new Point(120, 240);
        numericUpDownTemperature.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
        numericUpDownTemperature.Name = "numericUpDownTemperature";
        numericUpDownTemperature.Size = new Size(120, 23);
        numericUpDownTemperature.TabIndex = 11;
        numericUpDownTemperature.Value = new decimal(new int[] { 7, 0, 0, 65536 });
        // 
        // labelTemperature
        // 
        labelTemperature.AutoSize = true;
        labelTemperature.Location = new Point(20, 242);
        labelTemperature.Name = "labelTemperature";
        labelTemperature.Size = new Size(68, 17);
        labelTemperature.TabIndex = 10;
        labelTemperature.Text = "温度参数：";
        // 
        // numericUpDownMaxTokens
        // 
        numericUpDownMaxTokens.Location = new Point(120, 200);
        numericUpDownMaxTokens.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
        numericUpDownMaxTokens.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericUpDownMaxTokens.Name = "numericUpDownMaxTokens";
        numericUpDownMaxTokens.Size = new Size(120, 23);
        numericUpDownMaxTokens.TabIndex = 9;
        numericUpDownMaxTokens.Value = new decimal(new int[] { 1000, 0, 0, 0 });
        // 
        // labelMaxTokens
        // 
        labelMaxTokens.AutoSize = true;
        labelMaxTokens.Location = new Point(20, 202);
        labelMaxTokens.Name = "labelMaxTokens";
        labelMaxTokens.Size = new Size(80, 17);
        labelMaxTokens.TabIndex = 8;
        labelMaxTokens.Text = "最大Tokens：";
        // 
        // numericUpDownTimeout
        // 
        numericUpDownTimeout.Location = new Point(120, 160);
        numericUpDownTimeout.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
        numericUpDownTimeout.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericUpDownTimeout.Name = "numericUpDownTimeout";
        numericUpDownTimeout.Size = new Size(120, 23);
        numericUpDownTimeout.TabIndex = 7;
        numericUpDownTimeout.Value = new decimal(new int[] { 30, 0, 0, 0 });
        // 
        // labelTimeout
        // 
        labelTimeout.AutoSize = true;
        labelTimeout.Location = new Point(20, 162);
        labelTimeout.Name = "labelTimeout";
        labelTimeout.Size = new Size(80, 17);
        labelTimeout.TabIndex = 6;
        labelTimeout.Text = "超时时间(秒)：";
        // 
        // textBoxModelName
        // 
        textBoxModelName.Location = new Point(120, 120);
        textBoxModelName.Name = "textBoxModelName";
        textBoxModelName.Size = new Size(320, 23);
        textBoxModelName.TabIndex = 5;
        textBoxModelName.Text = "gpt-3.5-turbo";
        // 
        // labelModelName
        // 
        labelModelName.AutoSize = true;
        labelModelName.Location = new Point(20, 123);
        labelModelName.Name = "labelModelName";
        labelModelName.Size = new Size(68, 17);
        labelModelName.TabIndex = 4;
        labelModelName.Text = "模型名称：";
        // 
        // textBoxApiKey
        // 
        textBoxApiKey.Location = new Point(120, 80);
        textBoxApiKey.Name = "textBoxApiKey";
        textBoxApiKey.PasswordChar = '*';
        textBoxApiKey.Size = new Size(320, 23);
        textBoxApiKey.TabIndex = 3;
        // 
        // labelApiKey
        // 
        labelApiKey.AutoSize = true;
        labelApiKey.Location = new Point(20, 83);
        labelApiKey.Name = "labelApiKey";
        labelApiKey.Size = new Size(60, 17);
        labelApiKey.TabIndex = 2;
        labelApiKey.Text = "API密钥：";
        // 
        // textBoxBaseUrl
        // 
        textBoxBaseUrl.Location = new Point(120, 40);
        textBoxBaseUrl.Name = "textBoxBaseUrl";
        textBoxBaseUrl.Size = new Size(320, 23);
        textBoxBaseUrl.TabIndex = 1;
        textBoxBaseUrl.Text = "https://api.openai.com/v1";
        // 
        // labelBaseUrl
        // 
        labelBaseUrl.AutoSize = true;
        labelBaseUrl.Location = new Point(20, 43);
        labelBaseUrl.Name = "labelBaseUrl";
        labelBaseUrl.Size = new Size(80, 17);
        labelBaseUrl.TabIndex = 0;
        labelBaseUrl.Text = "API基础URL：";
        // 
        // buttonOK
        // 
        buttonOK.Location = new Point(232, 374);
        buttonOK.Name = "buttonOK";
        buttonOK.Size = new Size(75, 30);
        buttonOK.TabIndex = 2;
        buttonOK.Text = "确定";
        buttonOK.UseVisualStyleBackColor = true;
        buttonOK.Click += buttonOK_Click;
        // 
        // buttonCancel
        // 
        buttonCancel.DialogResult = DialogResult.Cancel;
        buttonCancel.Location = new Point(323, 374);
        buttonCancel.Name = "buttonCancel";
        buttonCancel.Size = new Size(75, 30);
        buttonCancel.TabIndex = 3;
        buttonCancel.Text = "取消";
        buttonCancel.UseVisualStyleBackColor = true;
        // 
        // buttonTest
        // 
        buttonTest.Location = new Point(50, 374);
        buttonTest.Name = "buttonTest";
        buttonTest.Size = new Size(75, 30);
        buttonTest.TabIndex = 4;
        buttonTest.Text = "测试连接";
        buttonTest.UseVisualStyleBackColor = true;
        buttonTest.Click += buttonTest_Click;
        // 
        // buttonReset
        // 
        buttonReset.Location = new Point(141, 374);
        buttonReset.Name = "buttonReset";
        buttonReset.Size = new Size(75, 30);
        buttonReset.TabIndex = 5;
        buttonReset.Text = "重置";
        buttonReset.UseVisualStyleBackColor = true;
        buttonReset.Click += buttonReset_Click;
        // 
        // LlmApiConfigForm
        // 
        AcceptButton = buttonOK;
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = buttonCancel;
        ClientSize = new Size(484, 421);
        Controls.Add(buttonReset);
        Controls.Add(buttonTest);
        Controls.Add(buttonCancel);
        Controls.Add(buttonOK);
        Controls.Add(groupBoxApiSettings);
        Controls.Add(groupBoxApiType);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "LlmApiConfigForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "LLM API 配置";
        groupBoxApiType.ResumeLayout(false);
        groupBoxApiType.PerformLayout();
        groupBoxApiSettings.ResumeLayout(false);
        groupBoxApiSettings.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDownTemperature).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericUpDownMaxTokens).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericUpDownTimeout).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private GroupBox groupBoxApiType;
    private RadioButton radioButtonRealApi;
    private RadioButton radioButtonMockApi;
    private GroupBox groupBoxApiSettings;
    private TextBox textBoxBaseUrl;
    private Label labelBaseUrl;
    private TextBox textBoxApiKey;
    private Label labelApiKey;
    private TextBox textBoxModelName;
    private Label labelModelName;
    private NumericUpDown numericUpDownTimeout;
    private Label labelTimeout;
    private NumericUpDown numericUpDownMaxTokens;
    private Label labelMaxTokens;
    private NumericUpDown numericUpDownTemperature;
    private Label labelTemperature;
    private Button buttonOK;
    private Button buttonCancel;
    private Button buttonTest;
    private Button buttonReset;
}