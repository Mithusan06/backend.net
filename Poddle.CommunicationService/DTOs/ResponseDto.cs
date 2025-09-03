namespace Poddle.CommunicationService.DTOs;

public class ResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }

    public static ResponseDto Ok(string message, object? data = null) => new() { Success = true, Message = message, Data = data };
    public static ResponseDto Fail(string message, object? data = null) => new() { Success = false, Message = message, Data = data };
}
