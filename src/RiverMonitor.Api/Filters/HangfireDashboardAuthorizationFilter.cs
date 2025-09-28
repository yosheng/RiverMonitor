using Hangfire.Dashboard;

namespace RiverMonitor.Api.Filters;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly string _username;
    private readonly string _password;

    public HangfireDashboardAuthorizationFilter(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // 檢查是否已經有認證標記
        if (httpContext.Items.ContainsKey("HangfireAuthenticated"))
        {
            return true;
        }

        // 從 Header 中獲取 Basic Authentication
        var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Basic "))
        {
            var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            var decodedBytes = Convert.FromBase64String(encodedUsernamePassword);
            var decodedUsernamePassword = System.Text.Encoding.UTF8.GetString(decodedBytes);
            
            var credentials = decodedUsernamePassword.Split(':', 2);
            if (credentials.Length == 2)
            {
                var username = credentials[0];
                var password = credentials[1];

                if (username == _username && password == _password)
                {
                    httpContext.Items["HangfireAuthenticated"] = true;
                    return true;
                }
            }
        }

        // 如果沒有認證或認證失敗，返回 401
        httpContext.Response.StatusCode = 401;
        httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
        return false;
    }
}