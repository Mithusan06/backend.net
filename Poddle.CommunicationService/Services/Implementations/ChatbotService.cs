using Microsoft.Extensions.Options;
using Poddle.CommunicationService.Configurations;
using Poddle.CommunicationService.Services.Interfaces;

namespace Poddle.CommunicationService.Services.Implementations;

public class ChatbotService : IChatbotService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AiChatbotSettings> _settings;
    private readonly ILogger<ChatbotService> _logger;

    public ChatbotService(IHttpClientFactory httpClientFactory, IOptions<AiChatbotSettings> settings, ILogger<ChatbotService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings;
        _logger = logger;
    }

    public async Task<string> GenerateReplyAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userMessage)) return "";

        try
        {
            _logger.LogInformation("Generating AI reply using model {Model}", _settings.Value.Model);

            // Placeholder for actual AI provider call using HttpClient
            await Task.Delay(50, cancellationToken);
            var reply = $"Thanks for your message. You said: '{userMessage}'. How can I assist further?";
            return reply;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating AI reply");
            throw;
        }
    }

    public Task<bool> ShouldEscalateAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        var text = (userMessage ?? string.Empty).ToLowerInvariant();
        var escalate = text.Contains("agent") || text.Contains("human") || text.Contains("escalate") || text.Contains("support") || text.Contains("complaint") || text.Contains("refund");
        return Task.FromResult(escalate);
    }
}
