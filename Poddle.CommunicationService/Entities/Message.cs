using System.ComponentModel.DataAnnotations;

namespace Poddle.CommunicationService.Entities;

public class Message
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(128)]
    public string From { get; set; } = string.Empty;

    [MaxLength(128)]
    public string To { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Content { get; set; } = string.Empty;

    [MaxLength(32)]
    public string Direction { get; set; } = string.Empty; // Inbound | Outbound

    [MaxLength(32)]
    public string Status { get; set; } = "Pending"; // Pending | Received | Sent | Failed

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<ConversationLog> ConversationLogs { get; set; } = new List<ConversationLog>();
}
