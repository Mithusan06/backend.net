using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Poddle.CommunicationService.Configurations;
using Poddle.CommunicationService.DTOs;
using Poddle.CommunicationService.Entities;
using Poddle.CommunicationService.Repositories.Interfaces;
using Poddle.CommunicationService.Services.Interfaces;

namespace Poddle.CommunicationService.Services.Implementations;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IChatbotService _chatbotService;
    private readonly ISupportService _supportService;
    private readonly IOptions<WhatsAppSettings> _whatsAppSettings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MessageService> _logger;

    public MessageService(
        IMessageRepository messageRepository,
        IChatbotService chatbotService,
        ISupportService supportService,
        IOptions<WhatsAppSettings> whatsAppSettings,
        IHttpClientFactory httpClientFactory,
        ILogger<MessageService> logger)
    {
        _messageRepository = messageRepository;
        _chatbotService = chatbotService;
        _supportService = supportService;
        _whatsAppSettings = whatsAppSettings;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ResponseDto> ReceiveMessageAsync(MessageDto messageDto, CancellationToken cancellationToken = default)
    {
        var inbound = new Message
        {
            From = messageDto.From,
            To = messageDto.To,
            Content = messageDto.Content,
            Direction = "Inbound",
            Status = "Received",
            CreatedAt = DateTimeOffset.UtcNow
        };

        inbound = await _messageRepository.AddMessageAsync(inbound, cancellationToken);

        await _messageRepository.LogConversationAsync(new ConversationLog
        {
            MessageId = inbound.Id,
            Role = "user",
            Content = inbound.Content,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        if (await _chatbotService.ShouldEscalateAsync(inbound.Content, cancellationToken))
        {
            await _supportService.EscalateAsync(inbound, "Escalation keywords detected", cancellationToken);
            return ResponseDto.Ok("Message escalated to human support", new { inboundMessageId = inbound.Id });
        }

        var reply = await _chatbotService.GenerateReplyAsync(inbound.Content, cancellationToken);

        await _messageRepository.LogConversationAsync(new ConversationLog
        {
            MessageId = inbound.Id,
            Role = "assistant",
            Content = reply,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        var outboundResponse = await SendMessageAsync(new MessageDto
        {
            From = inbound.To,
            To = inbound.From,
            Content = reply
        }, cancellationToken);

        return ResponseDto.Ok("Inbound message processed", new { inboundMessageId = inbound.Id, outbound = outboundResponse });
    }

    public async Task<ResponseDto> SendMessageAsync(MessageDto messageDto, CancellationToken cancellationToken = default)
    {
        var outbound = new Message
        {
            From = messageDto.From,
            To = messageDto.To,
            Content = messageDto.Content,
            Direction = "Outbound",
            Status = "Pending",
            CreatedAt = DateTimeOffset.UtcNow
        };

        outbound = await _messageRepository.AddMessageAsync(outbound, cancellationToken);

        try
        {
            var success = await SendViaWhatsAppAsync(messageDto, cancellationToken);
            outbound.Status = success ? "Sent" : "Failed";
            await _messageRepository.UpdateMessageAsync(outbound, cancellationToken);
        }
        catch (Exception ex)
        {
            outbound.Status = "Failed";
            await _messageRepository.UpdateMessageAsync(outbound, cancellationToken);
            _logger.LogError(ex, "Failed to send WhatsApp message for MessageId {MessageId}", outbound.Id);
            throw;
        }

        await _messageRepository.LogConversationAsync(new ConversationLog
        {
            MessageId = outbound.Id,
            Role = "assistant",
            Content = outbound.Content,
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        return ResponseDto.Ok("Message sent", new { messageId = outbound.Id, status = outbound.Status });
    }

    private async Task<bool> SendViaWhatsAppAsync(MessageDto messageDto, CancellationToken cancellationToken)
    {
        var settings = _whatsAppSettings.Value;
        var phoneNumberId = settings.PhoneNumberId;
        var client = _httpClientFactory.CreateClient("WhatsApp");

        var payload = new
        {
            messaging_product = "whatsapp",
            to = messageDto.To,
            type = "text",
            text = new { body = messageDto.Content }
        };

        var endpoint = $"/{phoneNumberId}/messages";

        _logger.LogInformation("Sending WhatsApp message to {To}", messageDto.To);

        // Placeholder HTTP call - in production this will call WhatsApp Graph API
        await Task.Delay(50, cancellationToken);
        // var response = await client.PostAsJsonAsync(endpoint, payload, cancellationToken);
        // return response.IsSuccessStatusCode;
        return true;
    }
}
