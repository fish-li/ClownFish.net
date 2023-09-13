namespace ClownFish.Log;

/// <summary>
/// 日志相关的运行控制参数
/// </summary>
public static class LoggingOptions
{
    /// <summary>
    /// 日志内存缓冲队列的最大长度，默认值：5000
    /// </summary>
    public static int MaxCacheQueueLength { get; set; } = LocalSettings.GetInt("ClownFish_Log_MaxCacheQueueLength", 5000);

    /// <summary>
    /// 每次执行写入日志时的批次大小，默认值：300
    /// </summary>
    public static int WriteListBatchSize { get; set; } = LocalSettings.GetInt("ClownFish_Log_WriteListBatchSize", 300);

    /// <summary>
    /// 是否启用性能监控
    /// </summary>
    public static bool PerformanceEnable { get; set; } = LocalSettings.GetBool("ClownFish_Log_PerformanceEnable", 1);

    /// <summary>
    /// 针对HTTP请求的全局日志开关，默认值：true
    /// </summary>
    public static bool HttpActionEnableLog { get; set; } = LocalSettings.GetBool("ClownFish_Log_HttpActionEnable", 1);

    /// <summary>
    /// 针对MessageHandler的全局日志开关，默认值：true
    /// </summary>
    public static bool MessageHandlerEnableLog { get; set; } = LocalSettings.GetBool("ClownFish_Log_MessageHandlerEnable", 1);

    /// <summary>
    /// 针对BackgroundTask的全局日志开关，默认值：true
    /// </summary>
    public static bool BackgroundTaskEnableLog { get; set; } = LocalSettings.GetBool("ClownFish_Log_BackgroundTaskEnable", 1);

    /// <summary>
    /// 是否需要生成 InvokeLog 日志
    /// </summary>
    public static bool InvokeLogEnable { get; set; } = LocalSettings.GetBool("ClownFish_Log_InvokeLogEnable", 1);

    /// <summary>
    /// OprLog.AppKind 的默认值
    /// </summary>
    public static int AppKindDefaultValue = LocalSettings.GetInt("Default_Oprlog_AppKind", 0);


    /// <summary>
    /// 
    /// </summary>
    public static class Http
    {
        /// <summary>
        /// 是否必须记录 Request（HttpRequest/MqRequest）到日志中，无论当前操作是成功还是失败，否则仅供出现性能慢或者异常时才记录
        /// </summary>
        public static bool MustLogRequest { get; set; } = LocalSettings.GetBool("ClownFish_Log_HttpAll") || LocalSettings.GetBool("ClownFish_Log_MustLogRequest");

        /// <summary>
        /// 是否必须记录 RequestBody 到日志中
        /// </summary>
        public static bool LogRequestBody { get; set; } = LocalSettings.GetBool("ClownFish_Log_HttpAll") || LocalSettings.GetBool("ClownFish_Log_LogRequestBody");


        /// <summary>
        /// 是否必须记录 Response 到日志中
        /// </summary>
        public static bool MustLogResponse { get; set; } = LocalSettings.GetBool("ClownFish_Log_HttpAll") || LocalSettings.GetBool("ClownFish_Log_MustLogResponse");

        /// <summary>
        /// 是否必须记录 ResponseBody 到日志中
        /// </summary>
        public static bool LogResponseBody { get; set; } = LocalSettings.GetBool("ClownFish_Log_HttpAll") || LocalSettings.GetBool("ClownFish_Log_LogResponseBody");        
    }


    /// <summary>
    /// 
    /// </summary>
    public static class HttpClient
    {
        ///// <summary>
        ///// 是否必须记录 HttpClient-HttpRequestMessage 到日志中
        ///// </summary>
        //public static bool MustLogClientRequest { get; set; } = LocalSettings.GetBool("ClownFish_Log_HttpAll") || LocalSettings.GetBool("ClownFish_Log_MustLogClientRequest");

        /// <summary>
        /// 是否必须记录 HttpClient-HttpRequestMessage-Body 到日志中
        /// </summary>
        public static bool LogClientRequestBody { get; set; } = LocalSettings.GetBool("ClownFish_Log_HttpAll") || LocalSettings.GetBool("ClownFish_Log_LogClientRequestBody");



        /// <summary>
        /// 是否必须记录 HttpClient-HttpResponseMessage 到日志中
        /// </summary>
        public static bool MustLogClientResponse { get; set; } = LocalSettings.GetBool("ClownFish_Log_HttpAll") || LocalSettings.GetBool("ClownFish_Log_MustLogClientResponse");
        

        /// <summary>
        /// 是否必须记录 HttpClient-HttpResponseMessage-Body 到日志中
        /// </summary>
        public static bool LogClientResponseBody { get; set; } = LocalSettings.GetBool("ClownFish_Log_HttpAll") || LocalSettings.GetBool("ClownFish_Log_LogClientResponseBody");
    }
}
