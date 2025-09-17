using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.Text.Json;
using System.Windows.Forms;

namespace WinFormMcpServer.McpServer.Tools;

public class InvokeTestTool : IMcpTool, IGuiTool
{
    private MainForm _mainForm;

    public void SetForm(MainForm mainForm)
    {
        _mainForm = mainForm;
    }

    public string Name => "InvokeTestTool";

    public Tool Descriptor => new Tool
    {
        Name = Name,
        Description = "Invokes the Test button click event on the main form.",
        InputSchema = JsonSerializer.SerializeToElement(new { type = "object" })
    };

    public Task<CallToolResult> CallAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken)
    {
        _mainForm.InvokeTestButtonClick();
        
        return Task.FromResult(new CallToolResult
        {
            Content = [new TextContentBlock { Text = "Test button clicked successfully" }]
        });
    }
}