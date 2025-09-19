# WinForm MCP Server

一个功能完整的基于 Windows Forms 的 MCP (Model Context Protocol) 服务器应用，集成了 LLM API 调用、聊天界面、工具管理等企业级功能。

## 🚀 核心功能

### 🤖 LLM API 集成
- **可配置的 LLM 服务**：支持 Mock API 和真实 API 之间的无缝切换
- **OpenAI 兼容接口**：支持所有 OpenAI 兼容的 API 端点
- **智能配置管理**：配置自动保存到 JSON 文件，支持热更新
- **连接测试**：内置 API 连接测试功能，确保配置正确

### 💬 现代化聊天界面
- **Web 聊天界面**：基于现代 Web 技术的响应式聊天界面
- **实时消息处理**：支持实时消息发送和接收
- **美观的 UI 设计**：采用蓝灰色调的现代化设计风格
- **移动端适配**：完全响应式设计，支持各种屏幕尺寸

### 🔧 MCP 工具系统
- **工具管理**：完整的 MCP 工具注册和管理系统
- **远程工具调用**：支持通过 HTTP 端点调用 MCP 工具
- **工具测试**：内置工具测试界面，方便调试和验证
- **扩展性**：支持自定义工具开发和集成

### ⚙️ 配置管理
- **MCP 服务器配置**：支持多个 MCP 服务器的配置和管理
- **JSON 配置编辑**：可视化的 JSON 配置编辑器
- **配置持久化**：所有配置自动保存，应用重启后保持

## 🛠 技术栈

- **目标框架**：.NET 8 (net8.0-windows)
- **UI 框架**：Windows Forms + 现代 Web 界面
- **Web 服务**：ASP.NET Core Minimal API + Kestrel
- **协议支持**：ModelContextProtocol 0.3.0-preview.4
- **日志系统**：Serilog（滚动文件日志）
- **前端技术**：HTML5 + CSS3 + JavaScript
- **HTTP 客户端**：HttpClient with OpenAI 兼容接口

## 📋 前置条件
- .NET SDK 8.0+
- 可选：Visual Studio 2022（建议 Community 版）

## 🚀 快速开始

### 1. 构建与运行

**使用 Visual Studio：**
- 打开解决方案 `src\WinFormMcpServer.sln`
- 选择构建配置（Debug/Release），直接启动调试（F5）或运行（Ctrl+F5）

**使用 .NET CLI：**
```bash
# 进入项目目录
cd src/WinFormMcpServer

# 构建项目
dotnet build

# 启动应用
dotnet run
```

### 2. 应用配置

**启动服务器：**
1. 在主窗体中输入端口（默认 3000）
2. 点击"启动服务器"按钮
3. 状态栏显示"运行中"即表示服务已启动

**配置 LLM API：**
1. 点击菜单栏 "配置" → "LLM API配置"
2. 选择使用 Mock API 或真实 API
3. 如使用真实 API，填入相应的配置信息：
   - API 基础 URL（如：https://api.openai.com/v1）
   - API 密钥
   - 模型名称（如：gpt-3.5-turbo）
   - 其他参数（超时时间、最大 tokens、温度等）
4. 点击"测试连接"验证配置
5. 保存配置

### 3. 使用聊天界面

访问 `http://localhost:3000` 打开现代化的 Web 聊天界面：
- 支持实时消息发送和接收
- 响应式设计，适配各种设备
- 支持 MCP 工具调用和结果展示

## 🔌 API 端点

### MCP 协议端点
- **基础地址**：`http://localhost:{port}/mcp`
- **SSE 连接**：`http://localhost:{port}/sse`
- **聊天 API**：`http://localhost:{port}/api/chat`

### 静态资源
- **聊天界面**：`http://localhost:{port}/index`
- **静态文件**：`http://localhost:{port}/wwwroot/`

## 📁 项目结构

```
src/WinFormMcpServer/
├── Controllers/           # API 控制器
│   └── ChatController.cs  # 聊天 API 控制器
├── McpServer/            # MCP 服务器核心
│   ├── HttpMcpServer.cs  # HTTP MCP 服务器
│   ├── McpServerHost.cs  # MCP 服务器主机
│   └── Tools/            # MCP 工具集合
├── Models/               # 数据模型
│   ├── ChatModels.cs     # 聊天相关模型
│   ├── LlmApiConfig.cs   # LLM API 配置模型
│   └── McpClientConfig.cs # MCP 客户端配置模型
├── Services/             # 业务服务
│   ├── ChatService.cs    # 聊天服务
│   ├── ConfigurableLlmService.cs # 可配置 LLM 服务
│   ├── LlmApiConfigService.cs    # LLM API 配置服务
│   ├── LlmApiServiceFactory.cs   # LLM API 服务工厂
│   ├── RealLlmApiService.cs      # 真实 LLM API 服务
│   ├── MockLlmApiService.cs      # Mock LLM API 服务
│   └── McpClientService.cs       # MCP 客户端服务
├── wwwroot/              # Web 静态资源
│   ├── index.html        # 聊天界面
│   ├── chat.js          # 聊天脚本
│   └── styles.css       # 样式文件
├── MainForm.cs          # 主窗体
├── LlmApiConfigForm.cs  # LLM API 配置窗体
├── McpConfigForm.cs     # MCP 配置窗体
└── Program.cs           # 程序入口
```

## 🔧 配置文件

### LLM API 配置 (`llm-api-config.json`)
```json
{
  "UseMockApi": false,
  "BaseUrl": "https://api.openai.com/v1",
  "ApiKey": "your-api-key-here",
  "ModelName": "gpt-3.5-turbo",
  "TimeoutSeconds": 30,
  "MaxTokens": 1000,
  "Temperature": 0.7
}
```

### MCP 服务器配置 (`mcp-servers.json`)
```json
{
  "mcpServers": {
    "example-server": {
      "type": "sse",
      "url": "http://localhost:3001/sse",
      "name": "示例服务器"
    }
  }
}
```

## 📊 日志系统
- **日志框架**：Serilog
- **日志位置**：`logs/WinFormMcpServer_.log`
- **滚动策略**：每日滚动
- **日志级别**：Verbose（开发环境）

## 🧪 测试

### 端到端测试
测试脚本位于 `test/` 目录：
- 使用 Playwright 进行前端自动化测试
- 支持 API 端点测试
- 包含聊天功能集成测试

### MCP Inspector 测试
```bash
# 安装 MCP Inspector
npx @modelcontextprotocol/inspector

# 访问测试界面
http://localhost:6247/
```

## 📦 发布部署

### 框架依赖部署
```bash
dotnet publish src/WinFormMcpServer -c Release -r win-x64 --self-contained false
```

### 自包含部署
```bash
dotnet publish src/WinFormMcpServer -c Release -r win-x64 --self-contained true
```

发布产物位于：`bin/Release/net8.0-windows/win-x64/publish/`

## 🔗 MCP 客户端集成

在您的 MCP 客户端配置中添加：

```json
{
  "mcpServers": {
    "winform-mcp-server": {
      "type": "sse",
      "url": "http://localhost:3000/sse",
      "name": "WinForm MCP Server"
    }
  }
}
```

## 🤝 贡献指南

1. Fork 本仓库
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🆘 支持

如果您遇到问题或有疑问，请：
1. 查看 [Issues](../../issues) 页面
2. 创建新的 Issue 描述问题
3. 提供详细的错误信息和复现步骤

---

**注意**：本项目为企业级 MCP 服务器实现，集成了完整的 LLM API 调用、现代化聊天界面和工具管理功能，适用于生产环境部署。