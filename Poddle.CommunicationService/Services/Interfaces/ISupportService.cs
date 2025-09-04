using Poddle.CommunicationService.Entities;

namespace Poddle.CommunicationService.Services.Interfaces;

public interface ISupportService
{
    Task EscalateAsync(Message message, string reason, CancellationToken cancellationToken = default);
}
