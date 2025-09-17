# WinForm MCP Server Demo

一个基于 Windows Forms 的示例应用，内置最小化 ASP.NET Core 主机，通过 Model Context Protocol (MCP) 暴露 HTTP 端点，演示在 WinForms 桌面程序中集成并远程调用 MCP Server。

- 目标框架：.NET 8 (net8.0-windows)
- UI 框架：Windows Forms
- Web/Hosting：ASP.NET Core Minimal Hosting + Kestrel
- 协议与依赖：ModelContextProtocol 0.3.0-preview.4 系列包
- 日志：Serilog（滚动文件日志）

## 前置条件
- .NET SDK 8.0+
- 可选：Visual Studio 2022（建议 Community 版）

## 构建与运行

1) 使用 Visual Studio
- 打开解决方案 <mcfile name="WinFormMcpServer.sln" path="src\WinFormMcpServer.sln"></mcfile>
- 选择构建配置（Debug/Release），直接启动调试（F5）或运行（Ctrl+F5）

2) 使用 .NET CLI
- 在项目目录执行：
  - 进入 src/WinFormMcpServer 目录
  - 执行 `dotnet build` 进行构建
  - 执行 `dotnet run` 启动应用

应用启动后：
- 在主窗体中输入端口（默认 3000），点击“启动服务器”
- 状态栏显示“运行中”即表示服务已启动

## HTTP 端点（MCP）
- 基础地址：`http://localhost:{port}/mcp`
- 服务器启动日志中会打印类似：`MCP Server started on http://localhost:{port}/mcp`
- 应用内部通过 <mcsymbol name="MapMcp" filename="HttpMcpServer.cs" path="src\WinFormMcpServer\McpServer\HttpMcpServer.cs" startline="74" type="function"></mcsymbol> 注册 MCP 路由
- 代码中还包含对“/stateless”处理的注释与挂载逻辑，用于在未匹配到其它路由时处理无状态调用

提示：这是用于演示的最小功能服务器，Capabilities 中暂未注册实际工具、资源与提示，仅用于通路验证。

## 日志
- Serilog 以每日滚动方式输出到应用目录下的 logs 文件夹
- 文件名模式：`logs/WinFormMcpServer_.log`
- 日志级别：Verbose（记录最详尽）

相关实现：
- <mcsymbol name="ConfigureSerilog" filename="HttpMcpServer.cs" path="src\WinFormMcpServer\McpServer\HttpMcpServer.cs" startline="24" type="function"></mcsymbol>

## 发布
- 示例（框架依赖部署）：
  - `dotnet publish src/WinFormMcpServer -c Release -r win-x64 --self-contained false`
- 发布产物位于 `bin/Release/net8.0-windows/win-x64/publish/`

## 测试
- 端到端测试脚本请放置在仓库根目录的 `test/` 目录中
- 如需对 HTTP 端点进行自动化验证，可结合 Playwright 发起请求或集成 UI 自动化（本仓库暂未包含示例）
- 使用Inspector进行测试
  - 安装Inspector：npx @modelcontextprotocol/inspector node build/index.js
  - 访问Inspector UI：`http://localhost:6247/`

## MCP集成
```
{
    "mcpServers": {
        "default-server": {
            "type": "sse",
            "url": "http://localhost:3000/sse",
            "note": "For SSE connections, add this URL directly in your MCP Client"
        }
    }
}
```
## 许可证
- 本仓库用于演示目的，按需添加许可证文件