namespace ClownFish.ImClients.Impls;

internal class FeishuInternalClient
{
    private readonly string _appId;
    private readonly string _appSecret;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="appSecret"></param>
    public FeishuInternalClient(string appId, string appSecret)
    {
        if( appId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(appId));
        if( appSecret.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(appSecret));

        _appId = appId;
        _appSecret = appSecret;
    }


    internal HttpOption SetAuthorization(HttpOption httpOption)
    {
        string token = FeishuTokenContainer.Instance.GetAccessToken(_appId, _appSecret);
        httpOption.Headers["Authorization"] = token;
        return httpOption;
    }

    private string GetUrl(int receiveType)
    {
        // https://open.feishu.cn/document/uAjLw4CM/ukTMukTMukTM/reference/im-v1/message/create

        // 【应用】向【个人】推送消息
        if( receiveType == 1 )
            return "https://open.feishu.cn/open-apis/im/v1/messages?receive_id_type=user_id";


        // 【应用】向【聊天群】推送消息
        if( receiveType == 2 )
            return "https://open.feishu.cn/open-apis/im/v1/messages?receive_id_type=chat_id";


        throw new NotSupportedException($"receiveType = {receiveType} is error.");
    }

    public void SendText(int receiveType, string receiveId, string text)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendText0(receiveType, receiveId, text);
        SetAuthorization(httpOption).ExecRPC<FsSbResult>();
    }


    public async Task SendTextAsync(int receiveType, string receiveId, string text)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendText0(receiveType, receiveId, text);
        await SetAuthorization(httpOption).ExecRPCAsync<FsSbResult>();
    }

    private HttpOption SendText0(int receiveType, string receiveId, string text)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                receive_id = receiveId,
                content = (new { text }).ToJson(),   // 飞书个SB的设计，要求这里是一个JSON字符串！
                msg_type = "text"
            }
        };
    }

    public void SendMarkdown(int receiveType, string receiveId, string text)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendMarkdown0(receiveType, receiveId, text);
        SetAuthorization(httpOption).ExecRPC<FsSbResult>();
    }


    public async Task SendMarkdownAsync(int receiveType, string receiveId, string text)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendMarkdown0(receiveType, receiveId, text);
        await SetAuthorization(httpOption).ExecRPCAsync<FsSbResult>();
    }


    private HttpOption SendMarkdown0(int receiveType, string receiveId, string text)
    {
        string content = new {
                            config = new {
                                wide_screen_mode = true,
                                enable_forward = true,
                            },
                            elements = new object[] {
                                                        new {
                                                            tag = "markdown",
                                                            content = text
                                                        },
                                                    }
                        }.ToJson();

        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                receive_id = receiveId,
                content = content,
                msg_type = "interactive"
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
        if( receiveId.IsNullOrEmpty() || fileName.IsNullOrEmpty() || fileBody.IsNullOrEmpty() )
            return;

        string fileId = UploadFile(fileBody, fileName, "stream");
        //Console2.WriteLine("Upload File OK, fileId: " + fileId);

        HttpOption httpOption = SendFile0(receiveType, receiveId, fileId);
        SetAuthorization(httpOption).ExecRPC<FsSbResult>();
    }



    public async Task SendFileAsync(int receiveType, string receiveId, byte[] fileBody, string fileName)
    {
        if( receiveId.IsNullOrEmpty() || fileName.IsNullOrEmpty() || fileBody.IsNullOrEmpty() )
            return;

        string fileId = await UploadFileAsync(fileBody, fileName, "stream");

        HttpOption httpOption = SendFile0(receiveType, receiveId, fileId);
        await SetAuthorization(httpOption).ExecRPCAsync<FsSbResult>();
    }

    private HttpOption SendFile0(int receiveType, string receiveId, string fileId)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                receive_id = receiveId,
                content = (new { file_key = fileId }).ToJson(),   // 飞书个SB的设计，要求这里是一个JSON字符串！
                msg_type = "file"
            }
        };
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    /// <param name="mediaType">范围：opus, mp4, pdf, doc, xls, ppt, stream</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string UploadFile(byte[] fileBody, string fileName, string mediaType)
    {
        if( fileBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() || mediaType.IsNullOrEmpty() )
            throw new ArgumentNullException();

        HttpOption httpOption = UploadFile0(fileBody, fileName, mediaType);
        UploadResult result = SetAuthorization(httpOption).ExecRPC<UploadResult>();

        string imageId = result.Data["file_key"]?.ToString();
        if( imageId.IsNullOrEmpty() )
            throw new InvalidOperationException("飞书的文件上传接口没有返回期望的数据：file_key 不存在！");

        return imageId;
    }


    public async Task<string> UploadFileAsync(byte[] fileBody, string fileName, string mediaType)
    {
        if( fileBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() || mediaType.IsNullOrEmpty() )
            throw new ArgumentNullException();

        HttpOption httpOption = UploadFile0(fileBody, fileName, mediaType);
        UploadResult result = await SetAuthorization(httpOption).ExecRPCAsync<UploadResult>();

        string imageId = result.Data["file_key"]?.ToString();
        if( imageId.IsNullOrEmpty() )
            throw new InvalidOperationException("飞书的文件上传接口没有返回期望的数据：file_key 不存在！");

        return imageId;
    }


    private HttpOption UploadFile0(byte[] fileBody, string fileName, string mediaType)
    {
        // https://open.feishu.cn/document/uAjLw4CM/ukTMukTMukTM/reference/im-v1/file/create
        return new HttpOption {
            Method = "POST",
            Url = "https://open.feishu.cn/open-apis/im/v1/files",
            Format = SerializeFormat.Multipart,
            Data = new {
                file_type = mediaType,
                file_name = fileName,
                file = new HttpFile { FileName = fileName, FileBody = fileBody },
            }
        };
    }


    public void SendImage(int receiveType, string receiveId, string filePath)
    {
        byte[] imageBody = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);
        SendImage(receiveType, receiveId, imageBody, fileName);
    }


    public async Task SendImageAsync(int receiveType, string receiveId, string filePath)
    {
        byte[] imageBody = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);
        await SendImageAsync(receiveType, receiveId, imageBody, fileName);
    }


    public void SendImage(int receiveType, string receiveId, byte[] imageBody, string filename)
    {
        if( receiveId.IsNullOrEmpty() || imageBody.IsNullOrEmpty() )
            return;

        string imageId = UploadImage(imageBody, filename);
        //Console2.WriteLine("Upload Image OK, imageId: " + imageId);

        HttpOption httpOption = SendImage0(receiveType, receiveId, imageId);
        SetAuthorization(httpOption).ExecRPC<FsSbResult>();
    }

    public async Task SendImageAsync(int receiveType, string receiveId, byte[] imageBody, string filename)
    {
        if( receiveId.IsNullOrEmpty() || imageBody.IsNullOrEmpty() )
            return;

        string imageId = await UploadImageAsync(imageBody, filename);

        HttpOption httpOption = SendImage0(receiveType, receiveId, imageId);
        await SetAuthorization(httpOption).ExecRPCAsync<FsSbResult>();
    }


    private HttpOption SendImage0(int receiveType, string receiveId, string imageId)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                receive_id = receiveId,
                content = (new { image_key = imageId }).ToJson(),   // 飞书个SB的设计，要求这里是一个JSON字符串！
                msg_type = "image"
            }
        };
    }


    public string UploadImage(byte[] imageBody, string filename)
    {
        HttpOption httpOption = UploadImage0(imageBody, filename);
        UploadResult result = SetAuthorization(httpOption).ExecRPC<UploadResult>();

        string imageId = result.Data["image_key"]?.ToString();
        if( imageId.IsNullOrEmpty() )
            throw new InvalidOperationException("飞书的文件上传接口没有返回期望的数据：image_key 不存在！");

        return imageId;
    }


    public async Task<string> UploadImageAsync(byte[] imageBody, string filename)
    {
        HttpOption httpOption = UploadImage0(imageBody, filename);
        UploadResult result = await SetAuthorization(httpOption).ExecRPCAsync<UploadResult>();

        string imageId = result.Data["image_key"]?.ToString();
        if( imageId.IsNullOrEmpty() )
            throw new InvalidOperationException("飞书的文件上传接口没有返回期望的数据：image_key 不存在！");

        return imageId;
    }

    private HttpOption UploadImage0(byte[] imageBody, string filename)
    {
        // https://open.feishu.cn/document/uAjLw4CM/ukTMukTMukTM/reference/im-v1/image/create
        return new HttpOption {
            Method = "POST",
            Url = "https://open.feishu.cn/open-apis/im/v1/images",
            Format = SerializeFormat.Multipart,
            Data = new {
                image_type = "message",
                image = new HttpFile { FileName = filename, FileBody = imageBody },
            }
        };
    }

    internal sealed class UploadResult : FsSbResult
    {
        public JObject Data { get; set; }
    }


    public void SendCard(int receiveType, string receiveId, string title, string text, string href)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendCard0(receiveType, receiveId, title, text, href);
        SetAuthorization(httpOption).ExecRPC<FsSbResult>();
    }


    public async Task SendCardAsync(int receiveType, string receiveId, string title, string text, string href)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendCard0(receiveType, receiveId, title, text, href);
        await SetAuthorization(httpOption).ExecRPCAsync<FsSbResult>();
    }

    private HttpOption SendCard0(int receiveType, string receiveId, string title, string text, string href)
    {
        string content = new {
            config = new {
                wide_screen_mode = true
            },
            elements = new object[] {
                new {
                    tag = "markdown",content = text
                },
                new {
                    tag = "action",
                    actions = new object[] {
                        new {
                            tag = "button",
                            text = new {
                                tag = "lark_md", content = "查看详情"
                            },
                            type = "primary",
                            url = href
                        }
                    }
                }
            }
        }.ToJson();

        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                receive_id = receiveId,
                content = content,
                msg_type = "interactive"
            }
        };
    }



}
