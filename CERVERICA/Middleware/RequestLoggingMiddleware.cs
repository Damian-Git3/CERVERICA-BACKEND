public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        var logMessage = $"{request.Protocol} {request.Method} {request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        _logger.LogInformation(logMessage);

        await _next(context);
    }
}