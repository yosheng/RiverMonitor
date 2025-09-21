namespace RiverMonitor.Bll;

public class ApiKeyHandler : DelegatingHandler
{
    private readonly string _apiKey;

    public ApiKeyHandler(string apiKey)
    {
        _apiKey = apiKey;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 取得現有的 URI
        if (request.RequestUri != null)
        {
            var uriBuilder = new System.UriBuilder(request.RequestUri);
        
            // 檢查查詢字串是否已存在
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

            // 動態加入 api_key
            query["api_key"] = _apiKey;

            // 重建 URI
            uriBuilder.Query = query.ToString();
            request.RequestUri = uriBuilder.Uri;
        }

        // 將請求傳遞給下一個處理器
        return await base.SendAsync(request, cancellationToken);
    }
}