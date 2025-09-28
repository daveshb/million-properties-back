using MyApp.Application.Dtos;
using System.Net;
using System.Text.Json;

namespace MyApp.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ErrorDtos.ErrorResponse("An error occurred while processing your request.");
        var traceId = context.TraceIdentifier;

        switch (exception)
        {
            case ArgumentNullException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorDtos.ErrorResponse("Required parameter is missing.", exception.Message, traceId);
                break;

            case ArgumentException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorDtos.ErrorResponse("Invalid request parameters.", exception.Message, traceId);
                break;

            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new ErrorDtos.ErrorResponse(exception.Message, null, traceId);
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new ErrorDtos.ErrorResponse("Unauthorized access.", null, traceId);
                break;

            case TimeoutException:
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                response = new ErrorDtos.ErrorResponse("Request timeout.", null, traceId);
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorDtos.ErrorResponse("An internal server error occurred.", null, traceId);
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
