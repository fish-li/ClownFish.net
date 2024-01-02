namespace ClownFish.ImClients.Impls;

internal class WxworkTokenContainer
{
    private readonly CacheDictionary<string> _cache = new CacheDictionary<string>(300, false, false);

    public static readonly WxworkTokenContainer Instance = new WxworkTokenContainer();

    public string GetAccessToken(string corpId, string appSecret)
    {
        string cacheKey = corpId + "\n" + appSecret;

        string token = _cache.Get(cacheKey);
        if( token != null )
            return token;

        // https://developer.work.weixin.qq.com/document/path/91039
        HttpOption httpOption = new HttpOption {
            Method = "GET",
            Url = "https://qyapi.weixin.qq.com/cgi-bin/gettoken",
            Data = new {
                corpid = corpId,
                corpsecret = appSecret
            }
        };

        XResult result = httpOption.ExecRPC<XResult>();

        token = result.Token;

        _cache.Set(cacheKey, token, DateTime.Now.AddMinutes(50));
        Console2.Debug("obtain a new WxWork token: " + token);

        return token;
    }

    public sealed class XResult : IShitResult
    {
        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        [JsonProperty("access_token")]
        public string Token { get; set; }
    }
}
