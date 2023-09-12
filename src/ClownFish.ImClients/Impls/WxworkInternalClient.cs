namespace ClownFish.ImClients.Impls;

internal class WxworkInternalClient : WxworkClient
{
    public WxworkInternalClient(string corpId, string appSecret, long agentId) 
        : base(corpId, appSecret, agentId)
    {
    }

    public void SendText(PushMsgType receiveType, string receiveId, string text)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendText0(receiveType, receiveId, text);
        SendRequest<ImSbResult>(httpOption);
    }


    public async Task SendTextAsync(PushMsgType receiveType, string receiveId, string text)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendText0(receiveType, receiveId, text);
        await SendRequestAsync<ImSbResult>(httpOption);
    }


    private HttpOption SendText0(PushMsgType receiveType, string receiveId, string text)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                agentid = this.AgentId,
                touser = (receiveType == PushMsgType.UserMsg ? receiveId : null),
                chatid = (receiveType == PushMsgType.GroupMsg ? receiveId : null),
                msgtype = "text",
                text = new {
                    content = text
                }
            }
        };
    }

    public void SendMarkdown(PushMsgType receiveType, string receiveId, string text)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendMarkdown0(receiveType, receiveId, text);
        SendRequest<ImSbResult>(httpOption);
    }


    public async Task SendMarkdownAsync(PushMsgType receiveType, string receiveId, string text)
    {
        if( receiveId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendMarkdown0(receiveType, receiveId, text);
        await SendRequestAsync<ImSbResult>(httpOption);
    }


    private HttpOption SendMarkdown0(PushMsgType receiveType, string receiveId, string text)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                agentid = this.AgentId,
                touser = (receiveType == PushMsgType.UserMsg ? receiveId : null),
                chatid = (receiveType == PushMsgType.GroupMsg ? receiveId : null),
                msgtype = "markdown",
                markdown = new {
                    content = text
                }
            }
        };
    }

    public void SendImage(PushMsgType receiveType, string receiveId, string filePath)
    {
        if( receiveId.IsNullOrEmpty() || filePath.IsNullOrEmpty() )
            return;

        byte[] imageBody = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);
        SendImage(receiveType, receiveId, imageBody, fileName);
    }


    public async Task SendImageAsync(PushMsgType receiveType, string receiveId, string filePath)
    {
        if( receiveId.IsNullOrEmpty() || filePath.IsNullOrEmpty() )
            return;

        byte[] imageBody = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);
        await SendImageAsync(receiveType, receiveId, imageBody, fileName);
    }


    public void SendImage(PushMsgType receiveType, string receiveId, byte[] imageBody, string fileName)
    {
        if( receiveId.IsNullOrEmpty() || imageBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() )
            return;

        string mediaId = UploadMedia(imageBody, fileName, "image");

        HttpOption httpOption = SendImage0(receiveType, receiveId, mediaId);
        SendRequest<ImSbResult>(httpOption);
    }

    public async Task SendImageAsync(PushMsgType receiveType, string receiveId, byte[] imageBody, string fileName)
    {
        if( receiveId.IsNullOrEmpty() || imageBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() )
            return;

        string mediaId = await UploadMediaAsync(imageBody, fileName, "image");

        HttpOption httpOption = SendImage0(receiveType, receiveId, mediaId);
        await SendRequestAsync<ImSbResult>(httpOption);
    }


    private HttpOption SendImage0(PushMsgType receiveType, string receiveId, string mediaId)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                agentid = this.AgentId,
                touser = (receiveType == PushMsgType.UserMsg ? receiveId : null),
                chatid = (receiveType == PushMsgType.GroupMsg ? receiveId : null),
                msgtype = "image",
                image = new {
                    media_id = mediaId
                }
            }
        };
    }

    public void SendFile(PushMsgType receiveType, string receiveId, byte[] fileBody, string fileName)
    {
        if( receiveId.IsNullOrEmpty() || fileBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() )
            return;

        string mediaId = UploadMedia(fileBody, fileName, "file");

        HttpOption httpOption = SendFile0(receiveType, receiveId, mediaId);
        SendRequest<ImSbResult>(httpOption);
    }


    public async Task SendFileAsync(PushMsgType receiveType, string receiveId, byte[] fileBody, string fileName)
    {
        if( receiveId.IsNullOrEmpty() || fileBody.IsNullOrEmpty() || fileName.IsNullOrEmpty() )
            return;

        string mediaId = await UploadMediaAsync(fileBody, fileName, "file");

        HttpOption httpOption = SendFile0(receiveType, receiveId, mediaId);
        await SendRequestAsync<ImSbResult>(httpOption);
    }


    private HttpOption SendFile0(PushMsgType receiveType, string receiveId, string mediaId)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                agentid = this.AgentId,
                touser = (receiveType == PushMsgType.UserMsg ? receiveId : null),
                chatid = (receiveType == PushMsgType.GroupMsg ? receiveId : null),
                msgtype = "file",
                file = new {
                    media_id = mediaId
                }
            }
        };
    }


    public void SendFile(PushMsgType receiveType, string receiveId, string filePath)
    {
        if( receiveId.IsNullOrEmpty() || filePath.IsNullOrEmpty() )
            return;

        byte[] fileBody = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        SendFile(receiveType, receiveId, fileBody, fileName);
    }

    public async Task SendFileAsync(PushMsgType receiveType, string receiveId, string filePath)
    {
        if( receiveId.IsNullOrEmpty() || filePath.IsNullOrEmpty() )
            return;

        byte[] fileBody = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        await SendFileAsync(receiveType, receiveId, fileBody, fileName);
    }


    public void SendCard(PushMsgType receiveType, string receiveId, string title, string text, string href)
    {
        if( receiveId.IsNullOrEmpty() || title.IsNullOrEmpty() || text.IsNullOrEmpty() || href.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendCard0(receiveType, receiveId, title, text, href);
        SendRequest<ImSbResult>(httpOption);
    }


    public async Task SendCardAsync(PushMsgType receiveType, string receiveId, string title, string text, string href)
    {
        if( receiveId.IsNullOrEmpty() || title.IsNullOrEmpty() || text.IsNullOrEmpty() || href.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendCard0(receiveType, receiveId, title, text, href);
        await SendRequestAsync<ImSbResult>(httpOption);
    }


    private HttpOption SendCard0(PushMsgType receiveType, string receiveId, string title, string text, string href)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(receiveType),
            Format = SerializeFormat.Json,
            Data = new {
                agentid = this.AgentId,
                touser = (receiveType == PushMsgType.UserMsg ? receiveId : null),
                chatid = (receiveType == PushMsgType.GroupMsg ? receiveId : null),
                msgtype = "textcard",
                textcard = new {
                    title = title,
                    description = text,
                    url = href,
                    btntxt = "查看详情"
                }
            }
        };
    }

}
