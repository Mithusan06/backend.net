using Microsoft.AspNetCore.Mvc;
using Poddle.CommunicationService.DTOs;
using Poddle.CommunicationService.Services.Interfaces;

namespace Poddle.CommunicationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(IMessageService messageService, ILogger<MessagesController> logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    [HttpPost("receive")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReceiveMessage([FromBody] MessageDto dto, CancellationToken cancellationToken)
    {
        var result = await _messageService.ReceiveMessageAsync(dto, cancellationToken);
        return Ok(result);
    }

    [HttpPost("send")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendMessage([FromBody] MessageDto dto, CancellationToken cancellationToken)
    {
        var result = await _messageService.SendMessageAsync(dto, cancellationToken);
        return Ok(result);
    }
}
