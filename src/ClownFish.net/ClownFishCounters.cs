namespace ClownFish.Base;

/// <summary>
/// ClownFish定义的一些计数器
/// </summary>
public static class ClownFishCounters
{
    /// <summary>
    /// 重置所有计数器
    /// </summary>
    public static void ResetAll()
    {
        ResetCounters(typeof(ClownFishCounters.ExecuteTimes));
        ResetCounters(typeof(ClownFishCounters.Console2));
        ResetCounters(typeof(ClownFishCounters.Logging));
    }

    internal static void ResetCounters(Type type)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
        fields = fields.Where(x => x.FieldType == typeof(ValueCounter)).ToArray();
        foreach( var x in fields ) {
            ValueCounter counter = (ValueCounter)x.GetValue(null);
            counter.Reset();
        }
    }

    /// <summary>
    /// 获取所有计数器的数据
    /// </summary>
    /// <returns></returns>
    public static List<NameInt64> GetAllValues()
    {
        List<NameInt64> list = new List<NameInt64>(64);

        FillValues(typeof(ClownFishCounters.ExecuteTimes), list);
        FillValues(typeof(ClownFishCounters.Console2), list);
        FillValues(typeof(ClownFishCounters.Logging), list);

        return list;
    }

    internal static void FillValues(Type type, List<NameInt64> list)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
        fields = fields.Where(x => x.FieldType == typeof(ValueCounter)).ToArray();
        foreach( var x in fields ) {
            
            string name = type.Name + "." + x.Name;

            ValueCounter counter = (ValueCounter)x.GetValue(null);
            NameInt64 kv = new NameInt64(name, counter.Get());
            list.Add(kv);
        }
    }


    internal static DebugReportBlock GetReportBlock()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Logging Counters" };

        block.AppendLine("MaxCacheQueueLength: " + ClownFish.Log.LoggingOptions.MaxCacheQueueLength.ToString());

        block.AppendLine("WriteCount: " + ClownFishCounters.Logging.WriteCount.Get().ToWString());
        block.AppendLine("InQueueCount: " + ClownFishCounters.Logging.InQueueCount.Get().ToWString());
        block.AppendLine("GiveupCount: " + ClownFishCounters.Logging.GiveupCount.Get().ToWString());

        block.AppendLine("QueueFlushCount: " + ClownFishCounters.Logging.QueueFlushCount.Get().ToWString());
        block.AppendLine("WriterErrorCount: " + ClownFishCounters.Logging.WriterErrorCount.Get().ToWString());
        block.AppendLine("FatalErrorCount: " + ClownFishCounters.Logging.FatalErrorCount.Get().ToWString());

        block.AppendLine("XmlWriteCount: " + ClownFishCounters.Logging.XmlWriteCount.Get().ToWString());
        block.AppendLine("JsonWriteCount: " + ClownFishCounters.Logging.JsonWriteCount.Get().ToWString());
        block.AppendLine("Json2WriteCount: " + ClownFishCounters.Logging.Json2WriteCount.Get().ToWString());
        block.AppendLine("EsWriteCount: " + ClownFishCounters.Logging.EsWriteCount.Get().ToWString());
        block.AppendLine("Rabbit2WriteCount: " + ClownFishCounters.Logging.Rabbit2WriteCount.Get().ToWString());
        block.AppendLine("RabbitWriteCount: " + ClownFishCounters.Logging.RabbitWriteCount.Get().ToWString());
        block.AppendLine("KafkaWriteCount: " + ClownFishCounters.Logging.KafkaWriteCount.Get().ToWString());

        return block;
    }

    /// <summary>
    /// 运行次数相关的计数器
    /// </summary>
    public static class ExecuteTimes
    {
        /// <summary>
        /// 处理HTTP请求的次数
        /// </summary>
        public static readonly ValueCounter HttpCount = new ValueCounter("HttpCount");

        /// <summary>
        /// 处理HTTP请求过程中失败的次数
        /// </summary>
        public static readonly ValueCounter HttpError = new ValueCounter("HttpError");

        /// <summary>
        /// 处理MQ消息的次数
        /// </summary>
        public static readonly ValueCounter MessageCount = new ValueCounter("MessageCount");

        /// <summary>
        /// 处理MQ消息时失败的次数
        /// </summary>
        public static readonly ValueCounter MessageError = new ValueCounter("MessageError");

        /// <summary>
        /// 执行后台任务的次数
        /// </summary>
        public static readonly ValueCounter BgTaskCount = new ValueCounter("BgTaskCount");

        /// <summary>
        /// 执行后台任务时失败的次数
        /// </summary>
        public static readonly ValueCounter BgTaskError = new ValueCounter("BgTaskError");

    }


    /// <summary>
    /// 并发执行相关的计数器
    /// </summary>
    public static class Concurrents
    {
        /// <summary>
        /// 正在执行的HTTP请求数量
        /// </summary>
        public static readonly ValueCounter HttpConcurrent = new ValueCounter("HttpConcurrent");

        /// <summary>
        /// 正在执行的MQ请求数量
        /// </summary>
        public static readonly ValueCounter MessageConcurrent = new ValueCounter("MessageConcurrent");

        /// <summary>
        /// 正在执行的后台任务数量
        /// </summary>
        public static readonly ValueCounter BgTaskConcurrent = new ValueCounter("BgTaskConcurrent");

        /// <summary>
        /// 正在执行的HttpClient调用次数
        /// </summary>
        public static readonly ValueCounter HttpCallCount = new ValueCounter("HttpCallCount");

        /// <summary>
        /// 已打开的SQL连接数量
        /// </summary>
        public static readonly ValueCounter SqlConnCount = new ValueCounter("SqlConnCount");
    }

    /// <summary>
    /// Console2专属的计数器
    /// </summary>
    public static class Console2
    {
        /// <summary>
        /// 调用 Console2.Error(...) 的次数
        /// </summary>
        public static readonly ValueCounter Error = new ValueCounter("Console2Error");

        /// <summary>
        /// 调用 Console2.Warnning(...) 的次数
        /// </summary>
        public static readonly ValueCounter Warnning = new ValueCounter("Console2Warnning");
    }


    /// <summary>
    /// 日志相关计数器
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// 写入总次数，即调用  LogHelper.Write 的次数
        /// </summary>
        public static readonly ValueCounter WriteCount = new ValueCounter("WriteCount");

        /// <summary>
        /// 日志数据进入队列的总次数
        /// </summary>
        public static readonly ValueCounter InQueueCount = new ValueCounter("InQueueCount");
        /// <summary>
        /// 无法进入队列而被丢弃的日志数量，丢弃原因：队列超过最大长度。
        /// </summary>
        public static readonly ValueCounter GiveupCount = new ValueCounter("GiveupCount");
        /// <summary>
        /// 日志写入时，一个批次要写入的最大长度
        /// </summary>
        public static readonly ValueCounter MaxBatchSize = new ValueCounter("MaxBatchSize");

        /// <summary>
        /// 日志批次写入的次数
        /// </summary>
        public static readonly ValueCounter QueueFlushCount = new ValueCounter("QueueFlushCount");
        /// <summary>
        /// 写日志时遇到的异常次数
        /// </summary>
        public static readonly ValueCounter WriterErrorCount = new ValueCounter("WriterErrorCount");
        /// <summary>
        /// 记录“致命异常”的次数
        /// </summary>
        public static readonly ValueCounter FatalErrorCount = new ValueCounter("FatalErrorCount");

        /// <summary>
        /// XmlWriter 处理的日志数量
        /// </summary>
        public static readonly ValueCounter XmlWriteCount = new ValueCounter("XmlWriteCount");
        /// <summary>
        /// JsonWriter 处理的日志数量
        /// </summary>
        public static readonly ValueCounter JsonWriteCount = new ValueCounter("JsonWriteCount");
        /// <summary>
        /// Json2Writer 处理的日志数量
        /// </summary>
        public static readonly ValueCounter Json2WriteCount = new ValueCounter("Json2WriteCount");
        /// <summary>
        /// HttpJsonWriter 处理的日志数量
        /// </summary>
        public static readonly ValueCounter HttpJsonWriteCount = new ValueCounter("HttpJsonWriteCount");
        /// <summary>
        /// RabbitHttpWriter 处理的日志数量
        /// </summary>
        public static readonly ValueCounter Rabbit2WriteCount = new ValueCounter("Rabbit2WriteCount");
        /// <summary>
        /// ElasticsearchWriter 处理的日志数量
        /// </summary>
        public static readonly ValueCounter EsWriteCount = new ValueCounter("ElasticsearchWriterCount");

        /// <summary>
        /// RabbitWriter 处理的日志数量
        /// </summary>
        public static readonly ValueCounter RabbitWriteCount = new ValueCounter("RabbitWriteCount");
        /// <summary>
        /// KafkaWriter 处理的日志数量
        /// </summary>
        public static readonly ValueCounter KafkaWriteCount = new ValueCounter("KafkaWriteCount");
    }


}
