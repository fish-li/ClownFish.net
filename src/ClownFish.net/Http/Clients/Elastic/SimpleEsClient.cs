using ClownFish.Base.Internals;

namespace ClownFish.Http.Clients.Elastic;


// 参考文档
// https://www.elastic.co/guide/en/elasticsearch/reference/current/docs.html
// https://www.elastic.co/guide/en/elasticsearch/reference/current/search-search.html


/// <summary>
/// 一个简单的elasticsearch客户端
/// </summary>
public sealed class SimpleEsClient
{
    internal const JsonStyle EsJsonStyle = JsonStyle.UtcTime | JsonStyle.CamelCase;

    private readonly EsConnOption _option;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="option"></param>
    public SimpleEsClient(EsConnOption option)
    {
        if( option == null )
            throw new ArgumentNullException(nameof(option));

        option.Validate();
        _option = option;
    }

    private HttpOption SetAuth(HttpOption httpOption)
    {
        // https://www.elastic.co/guide/en/elasticsearch/reference/current/http-clients.html

        if( _option.UserName.HasValue() )
            httpOption.SetBasicAuthorization(_option.UserName, _option.Password);

        return httpOption;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetTimeout()
    {
        if( _option.TimeoutMs.HasValue && _option.TimeoutMs.Value > 0 )
            return _option.TimeoutMs.Value;
        else
            return HttpClientDefaults.EsHttpClientTimeout;
    }

    /// <summary>
    /// 创建一个HttpOption实例
    /// </summary>
    /// <param name="method"></param>
    /// <param name="url"></param>
    /// <returns></returns>
    public HttpOption CreateHttpOption(string method, string url)
    {
        HttpOption httpOption = new HttpOption {
            Method = method,
            Url = _option.Url + url,
            Timeout = GetTimeout()
        };

        return SetAuth(httpOption);
    }


    #region 写数据

    /// <summary>
    /// 获取索引名称，
    /// 默认返回：name-yyyyMMdd
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    private string GetIndexName(Type dataType)
    {
        return dataType.Name.NameToLower() + DateTime.Now.ToString(_option.IndexNameTimeFormat);
    }

    /// <summary>
    /// 获取数据对象在保存时映射的文档ID
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private string GetDocumentId(IMsgObject data)
    {
        return data.GetId();
    }



    /// <summary>
    /// Write one
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="indexName"></param>
    public void WriteOne<T>(T data, string indexName = null) where T : class, IMsgObject
    {
        if( data == null )
            return;

        HttpOption httpOption = GetWriteOneHttpOption(data, indexName);
        httpOption.Send();   //TODO: 没有检查返回结果是否正确
    }

    /// <summary>
    /// Write one
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="indexName"></param>
    public async Task WriteOneAsync<T>(T data, string indexName = null) where T : class, IMsgObject
    {
        if( data == null )
            return;

        HttpOption httpOption = GetWriteOneHttpOption(data, indexName);
        await httpOption.SendAsync();     //TODO: 没有检查返回结果是否正确
    }

    private HttpOption GetWriteOneHttpOption<T>(T info, string indexName) where T : class, IMsgObject
    {
        string index = indexName ?? GetIndexName(typeof(T));
        string id = GetDocumentId(info);

        HttpOption httpOption = new HttpOption {
            Id = "ClownFish_SimpleEsClient_WriteOne",
            Method = "POST",
            Url = _option.Url + $"/{index}/_doc/{id}",
            Data = info.ToJson(EsJsonStyle),
            Format = SerializeFormat.Json,
            Timeout = GetTimeout()
        };

        return SetAuth(httpOption);
    }





    /// <summary>
    /// Write list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="indexName"></param>
    public void WriteList<T>(List<T> list, string indexName = null) where T : class, IMsgObject
    {
        if( list.IsNullOrEmpty() )
            return;

        HttpOption httpOption = GetWriteListHttpOption(list, indexName);
        //httpOption.Send();
        
        string response = httpOption.GetResult();
        CheckWriteListResponse(response);

        //HttpResult<string> result = httpOption.GetResult<HttpResult<string>>();
        //Console2.WriteLine("====================== Write Elasticsearch Request =====================================");
        //Console.WriteLine(httpOption.ToAllText());
        //Console2.WriteLine("====================== Write Elasticsearch Response =====================================");
        //Console.WriteLine(result.ToAllText());
        //Console2.WriteLine("====================== Write Elasticsearch Response END =====================================");
    }


    private void CheckWriteListResponse(string response)
    {
        // 这里为了简单且减少不必要的性能损耗，就直接判断“特征字符串”
        if( response != null && response.Contains("\"errors\":true,") ) {
            throw new EsHttpException("写Elasticsearch失败！", response);
        }
    }

    /// <summary>
    /// Write list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="indexName"></param>
    public async Task WriteListAsync<T>(List<T> list, string indexName = null) where T : class, IMsgObject
    {
        if( list.IsNullOrEmpty() )
            return;

        HttpOption httpOption = GetWriteListHttpOption(list, indexName);
        string response = await httpOption.GetResultAsync();
        CheckWriteListResponse(response);
    }

    private HttpOption GetWriteListHttpOption<T>(List<T> list, string indexName) where T : class, IMsgObject
    {
        var dataList = new List<object>(list.Count * 2);
        foreach( T item in list ) {
            dataList.Add(new { index = new { _id = GetDocumentId(item) } });
            dataList.Add(item);
        }


        string index = indexName ?? GetIndexName(typeof(T));

        HttpOption httpOption = new HttpOption {
            Id = "ClownFish_SimpleEsClient_WriteList",
            Method = "POST",
            Url = _option.Url + $"/{index}/_bulk",
            Data = dataList.ToMultiLineJson(EsJsonStyle),
            Format = SerializeFormat.Json,
            Timeout = GetTimeout()
        };

        return SetAuth(httpOption);
    }

    #endregion


    #region 搜索数据

    internal class SearchResponse<T>
    {
        [JsonProperty("hits")]
        public SearchHit<T> Hits { get; set; }
    }

    internal class SearchHit<T>
    {
        [JsonProperty("hits")]
        public List<HitData<T>> Hits { get; set; }
    }

    internal class HitData<T>
    {
        [JsonProperty("_source")]
        public T Data { get; set; }
    }

    /// <summary>
    /// 搜索数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public List<T> Search<T>(string index, object body)
    {
        HttpOption httpOption = GetSearchHttpOption(index, body);

        SearchResponse<T> response = httpOption.GetResult<SearchResponse<T>>();
        if( response == null || response.Hits == null || response.Hits.Hits.IsNullOrEmpty() )
            return Empty.List<T>();

        return response.Hits.Hits.Select(x => x.Data).ToList();
    }


    /// <summary>
    /// 搜索数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<List<T>> SearchAsync<T>(string index, object body)
    {
        HttpOption httpOption = GetSearchHttpOption(index, body);

        SearchResponse<T> response = await httpOption.GetResultAsync<SearchResponse<T>>();
        if( response == null || response.Hits == null || response.Hits.Hits.IsNullOrEmpty() )
            return Empty.List<T>();

        return response.Hits.Hits.Select(x => x.Data).ToList();
    }


    private HttpOption GetSearchHttpOption(string index, object body)
    {
        HttpOption httpOption = new HttpOption {
            Id = "ClownFish_SimpleEsClient_Search",
            Method = "POST",
            Url = _option.Url + $"/{index.UrlEncode()}/_search?typed_keys=true",
            Format = SerializeFormat.Json,
            Data = body.ToJson(EsJsonStyle),  // 与NEST客户端保持一致
            Timeout = GetTimeout()
        };

        return SetAuth(httpOption);
    }


    #endregion


}
