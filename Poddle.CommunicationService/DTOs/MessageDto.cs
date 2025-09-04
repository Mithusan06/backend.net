using System.ComponentModel.DataAnnotations;

namespace Poddle.CommunicationService.DTOs;

public class MessageDto
{
    [Required]
    [MaxLength(128)]
    public string From { get; set; } = string.Empty;

    [Required]
    [MaxLength(128)]
    public string To { get; set; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string Content { get; set; } = string.Empty;
}
