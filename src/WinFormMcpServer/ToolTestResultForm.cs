using System.Text.Json;

namespace WinFormMcpServer
{
    /// <summary>
    /// 工具测试结果显示窗体
    /// </summary>
    public partial class ToolTestResultForm : Form
    {
        public ToolTestResultForm(string toolName, string result)
        {
            InitializeComponent();
            
            this.Text = $"测试结果 - {toolName}";
            lblToolName.Text = $"工具: {toolName}";
            txtResult.Text = result;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtResult.Text))
                {
                    Clipboard.SetText(txtResult.Text);
                    MessageBox.Show("结果已复制到剪贴板", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"复制失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using var saveDialog = new SaveFileDialog
                {
                    Filter = "JSON文件 (*.json)|*.json|文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*",
                    DefaultExt = "json",
                    FileName = $"tool_result_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveDialog.FileName, txtResult.Text);
                    MessageBox.Show("结果已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}