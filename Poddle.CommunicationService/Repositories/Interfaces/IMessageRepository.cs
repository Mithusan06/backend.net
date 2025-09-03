using Poddle.CommunicationService.Entities;

namespace Poddle.CommunicationService.Repositories.Interfaces;

public interface IMessageRepository
{
    Task<Message> AddMessageAsync(Message message, CancellationToken cancellationToken = default);
    Task<Message?> GetMessageAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateMessageAsync(Message message, CancellationToken cancellationToken = default);
    Task LogConversationAsync(ConversationLog log, CancellationToken cancellationToken = default);
}
