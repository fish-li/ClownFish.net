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

    #region 基础方法

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


    /// <summary>
    /// 获取索引名称，
    /// 默认返回：name-yyyyMMdd
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    private string GetIndexName(Type dataType)
    {
        // IndexNameTimeFormat 这个参数应该跟随 dataType 才是最恰当的，但是这样不太好设计…………
        // 如果采用最简单的做法 [Attribute]，就会让这个参数失去灵活性。
        // 因为 IndexNameTimeFormat 是要和 ES 的 Index Lifecycle Policies 配合使用的，
        // 例如 如果希望OprLog保留 6小时，那么 IndexNameTimeFormat 就应该设置为 "-yyyyMMdd-HH"
        // 如果希望保留 30天，最好将 IndexNameTimeFormat 就应该设置为 "-yyyyMMdd"
        // 反之，如果设置为保留 N小时，IndexNameTimeFormat 就【不能】设置为 "-yyyyMM"

        // 现在这个做法就将 【灵活性】转移到类型之外了，由调用方来控制，
        // 此外，当前设计并不完美，_option.IndexNameTimeFormat 将影响所有的 dataType 调用参数，
        // 会导致不同的 日志数据类型 使用相同的 IndexNameTimeFormat ！
        // 因此如果希望能独立控制 IndexNameTimeFormat，可以创建多个 SimpleEsClient 实例，传递不同的 _option 参数

        if( _option.IndexNameTimeFormat.IsNullOrEmpty() )
            return dataType.Name.NameToLower();   // 这种情况使用一个索引
        else
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

    #endregion

    #region 写数据 One

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
        string response = httpOption.GetResult();
        CheckCreateResponse(response);
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
        string response = await httpOption.GetResultAsync();
        CheckCreateResponse(response);

        //HttpResult<string> result = await httpOption.GetResultAsync<HttpResult<string>>();
        //Console2.WriteLine("====================== Write Elasticsearch Request =====================================");
        //Console.WriteLine(httpOption.ToAllText());
        //Console2.WriteLine("====================== Write Elasticsearch Response =====================================");
        //Console.WriteLine(result.ToAllText());
        //Console2.WriteLine("====================== Write Elasticsearch Response END =====================================");
    }

    private HttpOption GetWriteOneHttpOption<T>(T info, string indexName) where T : class, IMsgObject
    {
        string index = indexName ?? GetIndexName(typeof(T));
        string id = GetDocumentId(info);

        // https://www.elastic.co/guide/en/elasticsearch/reference/7.17/docs-index_.html

        HttpOption httpOption = new HttpOption {
            Id = "ClownFish_SimpleEsClient_WriteOne",
            Method = "POST",
            Url = _option.Url + $"/{index}/_create/{id}",
            Data = info.ToJson(EsJsonStyle),
            Format = SerializeFormat.Json,
            Timeout = GetTimeout()
        };

        return SetAuth(httpOption);
    }

    private void CheckCreateResponse(string response)
    {
        if( response.HasValue() ) {
            CreateResponse resp = response.FromJson<CreateResponse>();
            if( resp != null && resp.Shards != null ) {
                if( resp.Shards.Failed > 0 )
                    throw new EsHttpException("写Elasticsearch失败！", response);
            }
        }
    }

    internal class CreateResponse
    {
        public CreateResponseShards Shards { get; set; }
    }

    internal class CreateResponseShards
    {
        //public long Total {  get; set; }
        //public long Successful { get; set; }
        public long Failed { get; set; }
    }

    #endregion

    #region 写数据 List/Bulk

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
        string response = httpOption.GetResult();
        CheckBulkResponse(response);

        //HttpResult<string> result = httpOption.GetResult<HttpResult<string>>();
        //Console2.WriteLine("====================== Write Elasticsearch Request =====================================");
        //Console.WriteLine(httpOption.ToAllText());
        //Console2.WriteLine("====================== Write Elasticsearch Response =====================================");
        //Console.WriteLine(result.ToAllText());
        //Console2.WriteLine("====================== Write Elasticsearch Response END =====================================");
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
        CheckBulkResponse(response);
    }

    private HttpOption GetWriteListHttpOption<T>(List<T> list, string indexName) where T : class, IMsgObject
    {
        var dataList = new List<object>(list.Count * 2);
        foreach( T item in list ) {
            dataList.Add(new { index = new { _id = GetDocumentId(item) } });
            dataList.Add(item);
        }


        string index = indexName ?? GetIndexName(typeof(T));

        // https://www.elastic.co/guide/en/elasticsearch/reference/7.17/docs-bulk.html

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

    private void CheckBulkResponse(string response)
    {
        // ES 这个傻屌不使用状态码，只能JSON反序列化 ResponseBody 读取里面的内容才能知道是否调用成功，
        // 但是Bulk的 ResponseBody 会比较大，所以这样做的成本太高…………
        // 绝大多数情况下，写入都会成功，除非 ES 挂了，磁盘空间不够这，网络不通，这些特殊情况发生。

        // 这里为了简单且减少不必要的性能损耗，就直接判断“特征字符串”
        if( response != null && response.Contains("\"errors\":true,") ) {
            throw new EsHttpException("写Elasticsearch失败！", response);
        }
    }

    //internal class BulkResponse
    //{
    //    public bool Errors { get; set; }
    //}

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
