using Poddle.CommunicationService.DTOs;

namespace Poddle.CommunicationService.Services.Interfaces;

public interface IMessageService
{
    Task<ResponseDto> ReceiveMessageAsync(MessageDto messageDto, CancellationToken cancellationToken = default);
    Task<ResponseDto> SendMessageAsync(MessageDto messageDto, CancellationToken cancellationToken = default);
}
