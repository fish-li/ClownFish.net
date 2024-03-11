namespace ClownFish.Log;

/// <summary>
/// 日志相关的运行控制参数
/// </summary>
public static class LoggingOptions
{
    /// <summary>
    /// RabbitMQ默认连接配置名称
    /// </summary>
    public static readonly string RabbitSettingName = "ClownFish_Log_Rabbit";

    /// <summary>
    /// Elasticsearch默认连接配置名称
    /// </summary>
    public static readonly string EsSettingName = "ClownFish_Log_Elasticsearch";


    /// <summary>
    /// 日志内存缓冲队列的最大长度，默认值：5000
    /// </summary>
    public static readonly int MaxCacheQueueLength = LocalSettings.GetInt("ClownFish_Log_MaxCacheQueueLength", 5000);

    /// <summary>
    /// 每次执行写入日志时的批次大小，默认值：300
    /// </summary>
    public static readonly int WriteListBatchSize = LocalSettings.GetInt("ClownFish_Log_WriteListBatchSize", 300);

    /// <summary>
    /// 是否启用“链路日志”
    /// </summary>
    public static bool TracingEnabled {
        get {
            // 有些程序对性能要求较高（例如转发操作），可以明确禁止
            if( LoggingOptions.PerformanceEnable == false )
                return false;


            // 如果日志全部被关闭，性能监控也没必要开启了
            bool enableLog = LoggingOptions.HttpActionEnableLog || LoggingOptions.MessageHandlerEnableLog || LoggingOptions.BackgroundTaskEnableLog;
            if( enableLog == false )
                return false;

            return true;
        }
    }

    /// <summary>
    /// 是否启用性能监控
    /// </summary>
    public static readonly bool PerformanceEnable = LocalSettings.GetBool("ClownFish_Log_PerformanceEnable", 1);

    /// <summary>
    /// 针对HTTP请求的全局日志开关，默认值：true
    /// </summary>
    public static readonly bool HttpActionEnableLog = LocalSettings.GetBool("ClownFish_Log_HttpActionEnable", 1);

    /// <summary>
    /// 针对MessageHandler的全局日志开关，默认值：true
    /// </summary>
    public static readonly bool MessageHandlerEnableLog = LocalSettings.GetBool("ClownFish_Log_MessageHandlerEnable", 1);

    /// <summary>
    /// 针对BackgroundTask的全局日志开关，默认值：true
    /// </summary>
    public static readonly bool BackgroundTaskEnableLog = LocalSettings.GetBool("ClownFish_Log_BackgroundTaskEnable", 1);

    /// <summary>
    /// 是否需要生成 InvokeLog 日志
    /// </summary>
    public static readonly bool InvokeLogEnable = LocalSettings.GetBool("ClownFish_Log_InvokeLogEnable", 1);

    /// <summary>
    /// OprLog.AppKind 的默认值
    /// </summary>
    public static int AppKindDefaultValue = LocalSettings.GetInt("Default_Oprlog_AppKind", 0);

    /// <summary>
    /// 是否记录 UserAgent
    /// </summary>
    public static /* 允许程序修改 */ bool LogUserAgent = LocalSettings.GetBool("ClownFish_Log_LogUserAgent", 0);

    /// <summary>
    /// 
    /// </summary>
    public static class Http
    {
        /// <summary>
        /// ClownFish_Log_HttpAll = 1
        /// </summary>
        public static readonly bool IsLogAll = LocalSettings.GetBool("ClownFish_Log_HttpAll");

        /// <summary>
        /// 是否必须记录 Request（HttpRequest/MqRequest）到日志中，无论当前操作是成功还是失败，否则仅供出现性能慢或者异常时才记录
        /// </summary>
        public static readonly bool MustLogRequest = IsLogAll || LocalSettings.GetBool("ClownFish_Log_MustLogRequest");

        /// <summary>
        /// 是否必须记录 RequestBody 到日志中
        /// </summary>
        public static readonly bool LogRequestBody = IsLogAll || LocalSettings.GetBool("ClownFish_Log_LogRequestBody", 1);


        /// <summary>
        /// 是否必须记录 Response 到日志中
        /// </summary>
        public static readonly bool MustLogResponse = IsLogAll || LocalSettings.GetBool("ClownFish_Log_MustLogResponse");

        /// <summary>
        /// 是否必须记录 ResponseBody 到日志中
        /// </summary>
        public static readonly bool LogResponseBody = IsLogAll || LocalSettings.GetBool("ClownFish_Log_LogResponseBody");
    }


    /// <summary>
    /// 
    /// </summary>
    public static class HttpClient
    {
        ///// <summary>
        ///// 是否必须记录 HttpClient-HttpRequestMessage 到日志中
        ///// </summary>
        //public static readonly bool MustLogClientRequest = Http.IsLogAll || LocalSettings.GetBool("ClownFish_Log_MustLogClientRequest");

        /// <summary>
        /// 是否必须记录 HttpClient-HttpRequestMessage-Body 到日志中
        /// </summary>
        public static readonly bool LogClientRequestBody = Http.IsLogAll || LocalSettings.GetBool("ClownFish_Log_LogClientRequestBody");



        /// <summary>
        /// 是否必须记录 HttpClient-HttpResponseMessage 到日志中
        /// </summary>
        public static readonly bool MustLogClientResponse = Http.IsLogAll || LocalSettings.GetBool("ClownFish_Log_MustLogClientResponse");


        /// <summary>
        /// 是否必须记录 HttpClient-HttpResponseMessage-Body 到日志中
        /// </summary>
        public static readonly bool LogClientResponseBody = Http.IsLogAll || LocalSettings.GetBool("ClownFish_Log_LogClientResponseBody");
    }
}
