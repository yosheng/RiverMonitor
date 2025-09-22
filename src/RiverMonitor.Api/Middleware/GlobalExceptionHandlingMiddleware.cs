namespace RiverMonitor.Api.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "发生未处理的异常");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            ArgumentException => new { error = exception.Message, status = 400 },
            UnauthorizedAccessException => new { error = "未授权访问", status = 401 },
            KeyNotFoundException => new { error = "请求的资源不存在", status = 404 },
            _ => new { error = $"处理请求时发生错误: {exception.Message}", status = 500 }
        };

        context.Response.StatusCode = response.status;
        await context.Response.WriteAsJsonAsync(response);
    }
}