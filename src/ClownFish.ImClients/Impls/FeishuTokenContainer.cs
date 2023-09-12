namespace ClownFish.ImClients.Impls;

internal sealed class FeishuTokenContainer
{
    private readonly CacheDictionary<string> _cache = new CacheDictionary<string>(300, false, false);

    public static readonly FeishuTokenContainer Instance = new FeishuTokenContainer();

    public string GetAccessToken(string appId, string appSecret)
    {
        string token = _cache.Get(appId);
        if( token != null )
            return token;

        // https://open.feishu.cn/document/ukTMukTMukTM/ukDNz4SO0MjL5QzM/g
        HttpOption httpOption = new HttpOption {
            Method = "POST",
            Url = "https://open.feishu.cn/open-apis/auth/v3/tenant_access_token/internal",
            Format = SerializeFormat.Json,
            Data = new {
                app_id = appId,
                app_secret = appSecret
            }
        };

        XResult result = httpOption.ExecRPC<XResult>();

        token = "Bearer " + result.Token;

        _cache.Set(appId, token, DateTime.Now.AddMinutes(50));
        Console2.Info("obtain a new Feishu token: " + token);

        return token;
    }

    public sealed class XResult : IShitResult
    {
        [JsonProperty("code")]
        public int ErrCode { get; set; }

        [JsonProperty("msg")]
        public string ErrMsg { get; set; }

        [JsonProperty("tenant_access_token")]
        public string Token { get; set; }
    }

}
