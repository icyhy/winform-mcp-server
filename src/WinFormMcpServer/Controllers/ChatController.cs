using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WinFormMcpServer.Models;
using WinFormMcpServer.Services;

namespace WinFormMcpServer.Controllers;

[ApiController]
[Route("api")]
public class ChatController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;
    private readonly IChatService _chatService;

    public ChatController(ILogger<ChatController> logger, IChatService chatService)
    {
        _logger = logger;
        _chatService = chatService;
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { error = "消息不能为空" });
            }

            _logger.LogInformation("收到聊天请求: {Message}", request.Message);

            var response = await _chatService.ProcessMessageAsync(request.Message);

            return Ok(new ChatResponse
            {
                Response = response.Message,
                ToolCalls = response.ToolCalls,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理聊天请求时发生错误");
            return StatusCode(500, new { error = "服务器内部错误", details = ex.Message });
        }
    }
}