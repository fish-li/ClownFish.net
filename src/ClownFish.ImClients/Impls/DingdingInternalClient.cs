namespace ClownFish.ImClients.Impls;

internal class DingdingInternalClient
{
    private readonly string _appId;
    private readonly string _appSecret;
    private readonly long _agentId;

    public DingdingInternalClient(string appId, string appSecret, long agentId)
    {
        if( appId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(appId));
        if( appSecret.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(appSecret));

        _appId = appId;
        _appSecret = appSecret;
        _agentId = agentId;
    }


    internal HttpOption SetAuthorization(HttpOption httpOption)
    {
        string token = DingdingTokenContainer.Instance.GetAccessToken(_appId, _appSecret);
        if( httpOption.Url.IndexOf('?') < 0 )
            httpOption.Url = httpOption.Url + "?access_token=" + token.UrlEncode();
        else
            httpOption.Url = httpOption.Url + "&access_token=" + token.UrlEncode();

        return httpOption;
    }

    private string GetUrl(int receiveType)
    {
        // 【应用】向【个人】推送消息
        // https://open.dingtalk.com/document/orgapp-server/asynchronous-sending-of-enterprise-session-messages
        if( receiveType == 1 )
            return "https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2";


        // 【应用】向【聊天群】推送消息
        // https://open.dingtalk.com/document/orgapp-server/send-group-messages
        if( receiveType == 2 )
            return "https://oapi.dingtalk.com/chat/send";


        throw new NotSupportedException($"receiveType = {receiveType} is error.");
    }



    public void SendText(int receiveType, string receiveId, string text)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendText0(receiveType, receiveId, text);
        SetAuthorization(httpOption).ExecRPC<ImSbResult>();
    }

    public async Task SendTextAsync(int receiveType, string receiveId, string text)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendText0(receiveType, receiveId, text);
        await SetAuthorization(httpOption).ExecRPCAsync<ImSbResult>();
    }

    private HttpOption SendText0(int receiveType, string receiveId, string text)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                agent_id = _agentId,
                userid_list = (receiveType == 1 ? receiveId : null),
                chatid = (receiveType == 2 ? receiveId : null),
                msg = new {
                    msgtype = "text",
                    text = new {
                        content = text
                    }
                }
            }
        };
    }


    public void SendMarkdown(int receiveType, string receiveId, string text)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendMarkdown0(receiveType, receiveId, text);
        SetAuthorization(httpOption).ExecRPC<ImSbResult>();
    }

    public async Task SendMarkdownAsync(int receiveType, string receiveId, string text)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendMarkdown0(receiveType, receiveId, text);
        await SetAuthorization(httpOption).ExecRPCAsync<ImSbResult>();
    }

    private HttpOption SendMarkdown0(int receiveType, string receiveId, string text)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                agent_id = _agentId,
                userid_list = (receiveType == 1 ? receiveId : null),
                chatid = (receiveType == 2 ? receiveId : null),
                msg = new {
                    msgtype = "markdown",
                    markdown = new {
                        title = DingdingUtils.GetMarkdonwTitle(text),
                        text = text
                    }
                }
            }
        };
    }

    public void SendImage(int receiveType, string receiveId, string filePath)
    {
        if( receiveId.IsNullOrEmpty() || filePath.IsNullOrEmpty() )
            return;

        byte[] imageBody = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);
        SendImage(receiveType, receiveId, imageBody, fileName);
    }

    public async Task SendImageAsync(int receiveType, string receiveId, string filePath)
    {
        if( receiveId.IsNullOrEmpty() || filePath.IsNullOrEmpty() )
            return;

        byte[] imageBody = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);
        await SendImageAsync(receiveType, receiveId, imageBody, fileName);
    }


    public void SendImage(int receiveType, string receiveId, byte[] imageBody, string fileName)
    {
        if( receiveId.IsNullOrEmpty() || imageBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() )
            return;

        string mediaId = UploadMedia(imageBody, fileName, "image");

        HttpOption httpOption = SendImage0(receiveType, receiveId, mediaId);
        SetAuthorization(httpOption).ExecRPC<ImSbResult>();
    }

    public async Task SendImageAsync(int receiveType, string receiveId, byte[] imageBody, string fileName)
    {
        if( receiveId.IsNullOrEmpty() || imageBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() )
            return;

        string mediaId = await UploadMediaAsync(imageBody, fileName, "image");

        HttpOption httpOption = SendImage0(receiveType, receiveId, mediaId);
        await SetAuthorization(httpOption).ExecRPCAsync<ImSbResult>();
    }

    private HttpOption SendImage0(int receiveType, string receiveId, string mediaId)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                agent_id = _agentId,
                userid_list = (receiveType == 1 ? receiveId : null),
                chatid = (receiveType == 2 ? receiveId : null),
                msg = new {
                    msgtype = "image",
                    image = new {
                        media_id = mediaId
                    }
                }
            }
        };
    }


    public void SendFile(int receiveType, string receiveId, string filePath)
    {
        if( receiveId.IsNullOrEmpty() || filePath.IsNullOrEmpty() )
            return;

        byte[] fileBody = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        SendFile(receiveType, receiveId, fileBody, fileName);
    }


    public async Task SendFileAsync(int receiveType, string receiveId, string filePath)
    {
        if( receiveId.IsNullOrEmpty() || filePath.IsNullOrEmpty() )
            return;

        byte[] fileBody = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        await SendFileAsync(receiveType, receiveId, fileBody, fileName);
    }

    public void SendFile(int receiveType, string receiveId, byte[] fileBody, string fileName)
    {
        if( receiveId.IsNullOrEmpty() || fileBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() )
            return;

        string mediaId = UploadMedia(fileBody, fileName, "file");

        HttpOption httpOption = SendFile0(receiveType, receiveId, mediaId);
        SetAuthorization(httpOption).ExecRPC<ImSbResult>();
    }

    public async Task SendFileAsync(int receiveType, string receiveId, byte[] fileBody, string fileName)
    {
        if( receiveId.IsNullOrEmpty() || fileBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() )
            return;

        string mediaId = await UploadMediaAsync(fileBody, fileName, "file");

        HttpOption httpOption = SendFile0(receiveType, receiveId, mediaId);
        await SetAuthorization(httpOption).ExecRPCAsync<ImSbResult>();
    }


    private HttpOption SendFile0(int receiveType, string receiveId, string mediaId)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                agent_id = _agentId,
                userid_list = (receiveType == 1 ? receiveId : null),
                chatid = (receiveType == 2 ? receiveId : null),
                msg = new {
                    msgtype = "file",
                    file = new {
                        media_id = mediaId
                    }
                }
            }
        };
    }

    


    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    /// <param name="mediaType">范围：image, voice, video, file</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string UploadMedia(byte[] fileBody, string fileName, string mediaType)
    {
        HttpOption httpOption = UploadMedia0(fileBody, fileName, mediaType);
        UploadResult result = SetAuthorization(httpOption).ExecRPC<UploadResult>();

        string mediaId = result.MediaId;
        if( mediaId.IsNullOrEmpty() )
            throw new InvalidOperationException("钉钉的文件上传接口没有返回期望的数据：media_id 不存在！");

        return mediaId;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    /// <param name="mediaType">范围：image, voice, video, file</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<string> UploadMediaAsync(byte[] fileBody, string fileName, string mediaType)
    {
        HttpOption httpOption = UploadMedia0(fileBody, fileName, mediaType);
        UploadResult result = await SetAuthorization(httpOption).ExecRPCAsync<UploadResult>();

        string mediaId = result.MediaId;
        if( mediaId.IsNullOrEmpty() )
            throw new InvalidOperationException("钉钉的文件上传接口没有返回期望的数据：media_id 不存在！");

        return mediaId;
    }


    private HttpOption UploadMedia0(byte[] fileBody, string fileName, string mediaType)
    {
        if( fileBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() || mediaType.IsNullOrEmpty() )
            throw new ArgumentNullException();

        // https://open.dingtalk.com/document/orgapp-server/upload-media-files
        return new HttpOption {
            Method = "POST",
            Url = "https://oapi.dingtalk.com/media/upload",
            Format = SerializeFormat.Multipart,
            Data = new {
                type = mediaType,
                media = new HttpFile { FileName = fileName, FileBody = fileBody },
            }
        };
    }

    internal sealed class UploadResult : ImSbResult
    {
        [JsonProperty("media_id")]
        public string MediaId { get; set; }
    }


    public void SendCard(int receiveType, string receiveId, string title, string text, string href)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendCard0(receiveType, receiveId, title, text, href);
        SetAuthorization(httpOption).ExecRPC<ImSbResult>();
    }

    public async Task SendCardAsync(int receiveType, string receiveId, string title, string text, string href)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendCard0(receiveType, receiveId, title, text, href);
        await SetAuthorization(httpOption).ExecRPCAsync<ImSbResult>();
    }

    private HttpOption SendCard0(int receiveType, string receiveId, string title, string text, string href)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                agent_id = _agentId,
                userid_list = (receiveType == 1 ? receiveId : null),
                chatid = (receiveType == 2 ? receiveId : null),
                msg = new {
                    msgtype = "action_card",
                    action_card = new {
                        title = title,
                        markdown = text,
                        single_title = "查看详情",
                        single_url = href
                    }
                }
            }
        };
    }



}
