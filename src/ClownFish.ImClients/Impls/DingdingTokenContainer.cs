namespace ClownFish.ImClients.Impls;

internal class DingdingTokenContainer
{
    private readonly CacheDictionary<string> _cache = new CacheDictionary<string>(300, false, false);

    public static readonly DingdingTokenContainer Instance = new DingdingTokenContainer();

    public string GetAccessToken(string appId, string appSecret)
    {
        string token = _cache.Get(appId);
        if( token != null )
            return token;

        // https://open.dingtalk.com/document/orgapp-server/obtain-orgapp-token
        HttpOption httpOption = new HttpOption {
            Method = "GET",
            Url = "https://oapi.dingtalk.com/gettoken",
            Data = new {
                appkey = appId,
                appsecret = appSecret
            }
        };
         
        XResult result = httpOption.ExecRPC<XResult>();

        token = result.Token;

        _cache.Set(appId, token, DateTime.Now.AddMinutes(50));
        Console2.Info("obtain a new Dingding token: " + token);

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
