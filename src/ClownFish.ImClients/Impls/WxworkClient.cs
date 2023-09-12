namespace ClownFish.ImClients.Impls;

/// <summary>
/// 推送消息的类别
/// </summary>
public enum PushMsgType
{
    /// <summary>
    /// 应用给用户推送消息
    /// </summary>
    UserMsg,

    /// <summary>
    /// 应用往聊天群推送消息
    /// </summary>
    GroupMsg
}

/// <summary>
/// 企业微信【基础】客户端。
/// 只提供了几个基础功能（用户认证/发送请求/获取用户信息），并没有提供各种消息的发送功能。
/// </summary>
public class WxworkClient
{
    private readonly string _corpId;
    private readonly string _appSecret;
    private readonly long _agentId;

    /// <summary>
    /// AgentId
    /// </summary>
    public long AgentId => _agentId;

    internal WxworkClient(string corpId, string appSecret, long agentId)
    {
        if( corpId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(corpId));
        if( appSecret.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(appSecret));

        _corpId = corpId;
        _appSecret = appSecret;
        _agentId = agentId;
    }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="settingName"></param>
    public WxworkClient(string settingName)
    {
        if( settingName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(settingName));

        ImAppAuthConfig opt = Settings.GetSetting<ImAppAuthConfig>(settingName, true);
        opt.Validate();

        _corpId = opt.AppId;
        _appSecret = opt.AppSecret;
        _agentId = opt.AgentId;
    }


    private HttpOption SetAuthorization(HttpOption httpOption)
    {
        string token = WxworkTokenContainer.Instance.GetAccessToken(_corpId, _appSecret);
        if( httpOption.Url.IndexOf('?') < 0 )
            httpOption.Url = httpOption.Url + "?access_token=" + token.UrlEncode();
        else
            httpOption.Url = httpOption.Url + "&access_token=" + token.UrlEncode();

        return httpOption;
    }


    internal string GetUrl(PushMsgType receiveType)
    {
        // 【应用】向【个人】推送消息
        // https://developer.work.weixin.qq.com/document/path/90236
        if( receiveType == PushMsgType.UserMsg )
            return "https://qyapi.weixin.qq.com/cgi-bin/message/send";


        // 【应用】向【聊天群】推送消息
        // https://developer.work.weixin.qq.com/document/path/90248
        if( receiveType == PushMsgType.GroupMsg )
            return "https://qyapi.weixin.qq.com/cgi-bin/appchat/send";

        throw new NotSupportedException($"receiveType = {receiveType} is error.");
    }


    /// <summary>
    /// 发送HTTP请求，并验证服务端返回结果是否成功
    /// </summary>
    /// <param name="httpOption"></param>
    public T SendRequest<T>(HttpOption httpOption) where T : IShitResult
    {
        return SetAuthorization(httpOption).ExecRPC<T>();
    }


    /// <summary>
    /// 发送HTTP请求，并验证服务端返回结果是否成功
    /// </summary>
    /// <param name="httpOption"></param>
    /// <returns></returns>
    public async Task<T> SendRequestAsync<T>(HttpOption httpOption) where T : IShitResult
    {
        return await SetAuthorization(httpOption).ExecRPCAsync<T>();
    }


    /// <summary>
    /// 发送一条消息数据
    /// </summary>
    /// <param name="data"></param>
    public T SendData<T>(object data) where T : IShitResult
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        PropertyInfo prop = data.GetType().GetProperty("chatid");
        PushMsgType receiveType = prop?.GetValue(null) != null ? PushMsgType.GroupMsg : PushMsgType.UserMsg;

        HttpOption httpOption = new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = data
        };
        return SendRequest<T>(httpOption);
    }


    /// <summary>
    /// 发送一条消息数据
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<T> SendDataAsync<T>(object data) where T : IShitResult
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        PropertyInfo prop = data.GetType().GetProperty("chatid");
        PushMsgType receiveType = prop?.GetValue(null) != null ? PushMsgType.GroupMsg : PushMsgType.UserMsg;

        HttpOption httpOption = new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = data
        };
        return await SendRequestAsync<T>(httpOption);
    }


    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    /// <param name="mediaType">取值范围：image, voice, video, file</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public string UploadMedia(byte[] fileBody, string fileName, string mediaType)
    {
        if( fileBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() || mediaType.IsNullOrEmpty() )
            throw new ArgumentNullException("fileBody or fileName or mediaType");

        HttpOption httpOption = UploadMedia0(fileBody, fileName, mediaType);
        UploadResult result = SetAuthorization(httpOption).ExecRPC<UploadResult>();

        string mediaId = result.MediaId;
        if( mediaId.IsNullOrEmpty() )
            throw new InvalidOperationException("企业微信的文件上传接口没有返回期望的数据：media_id 不存在！");

        return mediaId;
    }


    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    /// <param name="mediaType">取值范围：image, voice, video, file</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<string> UploadMediaAsync(byte[] fileBody, string fileName, string mediaType)
    {
        if( fileBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() || mediaType.IsNullOrEmpty() )
            throw new ArgumentNullException("fileBody or fileName or mediaType");

        HttpOption httpOption = UploadMedia0(fileBody, fileName, mediaType);
        UploadResult result = await SetAuthorization(httpOption).ExecRPCAsync<UploadResult>();

        string mediaId = result.MediaId;
        if( mediaId.IsNullOrEmpty() )
            throw new InvalidOperationException("企业微信的文件上传接口没有返回期望的数据：media_id 不存在！");

        return mediaId;
    }

    private HttpOption UploadMedia0(byte[] fileBody, string fileName, string mediaType)
    {
        // https://developer.work.weixin.qq.com/document/path/90253
        return new HttpOption {
            Method = "POST",
            Url = "https://qyapi.weixin.qq.com/cgi-bin/media/upload?type=" + mediaType,
            Format = SerializeFormat.Multipart,
            Data = new {
                media = new HttpFile { FileName = fileName, FileBody = fileBody },
            }
        };
    }

    internal sealed class UploadResult : ImSbResult
    {
        [JsonProperty("media_id")]
        public string MediaId { get; set; }
    }


    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public WxworkUserInfo GetUserInfo(string userId)
    {
        if( userId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(userId));

        HttpOption request = new HttpOption {
            Url = "https://qyapi.weixin.qq.com/cgi-bin/user/get",
            Data = new {
                userid = userId
            }
        };

        WxworkUserInfo userinfo = this.SendRequest<ImShitResult>(request);
        return userinfo;
    }


    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<WxworkUserInfo> GetUserInfoAsync(string userId)
    {
        if( userId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(userId));

        HttpOption request = new HttpOption {
            Url = "https://qyapi.weixin.qq.com/cgi-bin/user/get",
            Data = new {
                userid = userId
            }
        };

        WxworkUserInfo userinfo = await this.SendRequestAsync<ImShitResult>(request);
        return userinfo;
    }
}
