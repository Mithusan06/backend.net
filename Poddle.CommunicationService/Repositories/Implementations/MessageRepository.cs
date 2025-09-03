using Microsoft.EntityFrameworkCore;
using Poddle.CommunicationService.Data;
using Poddle.CommunicationService.Entities;
using Poddle.CommunicationService.Repositories.Interfaces;

namespace Poddle.CommunicationService.Repositories.Implementations;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _db;

    public MessageRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Message> AddMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _db.Messages.AddAsync(message, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return message;
    }

    public async Task<Message?> GetMessageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Messages.Include(m => m.ConversationLogs)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task UpdateMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        _db.Messages.Update(message);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task LogConversationAsync(ConversationLog log, CancellationToken cancellationToken = default)
    {
        await _db.ConversationLogs.AddAsync(log, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
