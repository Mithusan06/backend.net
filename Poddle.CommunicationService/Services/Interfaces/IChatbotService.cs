namespace Poddle.CommunicationService.Services.Interfaces;

public interface IChatbotService
{
    Task<string> GenerateReplyAsync(string userMessage, CancellationToken cancellationToken = default);
    Task<bool> ShouldEscalateAsync(string userMessage, CancellationToken cancellationToken = default);
}
