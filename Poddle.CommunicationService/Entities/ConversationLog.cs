using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Poddle.CommunicationService.Entities;

public class ConversationLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(Message))]
    public Guid MessageId { get; set; }

    [MaxLength(32)]
    public string Role { get; set; } = string.Empty; // user | assistant | system

    [MaxLength(4000)]
    public string Content { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Message? Message { get; set; }
}
