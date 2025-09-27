namespace MyApp.Application.Dtos;

public class ErrorDtos
{
    public record ErrorResponse(string Message, string? Details = null, string? TraceId = null);
    public record ValidationErrorResponse(string Message, Dictionary<string, string[]> Errors, string? TraceId = null);
}
