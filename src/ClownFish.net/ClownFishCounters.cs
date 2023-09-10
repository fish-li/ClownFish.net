namespace ClownFish.Base;

internal static class ClownFishCounters
{
    public static void ResetAll()
    {
        ResetAllCounter(typeof(ClownFishCounters.ExecuteTimes));
        ResetAllCounter(typeof(ClownFishCounters.Console2));
        ResetAllCounter(typeof(ClownFishCounters.Logging));
    }

    internal static void ResetAllCounter(Type type)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
        fields = fields.Where(x => x.FieldType == typeof(ValueCounter)).ToArray();
        foreach( var x in fields ) {
            ValueCounter counter = (ValueCounter)x.GetValue(null);
            counter.Reset();
        }
    }


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
    }

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


    public static class Logging
    {
        public static readonly ValueCounter WriteCount = new ValueCounter("WriteCount");
        public static readonly ValueCounter InQueueCount = new ValueCounter("InQueueCount");
        public static readonly ValueCounter GiveupCount = new ValueCounter("GiveupCount");
        public static readonly ValueCounter MaxBatchSize = new ValueCounter("MaxBatchSize");

        public static readonly ValueCounter QueueFlushCount = new ValueCounter("QueueFlushCount");
        public static readonly ValueCounter WriterErrorCount = new ValueCounter("WriterErrorCount");
        public static readonly ValueCounter FatalErrorCount = new ValueCounter("FatalErrorCount");

        public static readonly ValueCounter XmlWriterCount = new ValueCounter("XmlWriterCount");
        public static readonly ValueCounter JsonWriterCount = new ValueCounter("JsonWriterCount");
        public static readonly ValueCounter Json2WriterCount = new ValueCounter("Json2WriterCount");
        public static readonly ValueCounter HttpJsonWriterCount = new ValueCounter("HttpJsonWriterCount");
    }

    public static class RealtimeStatus
    {
        public static readonly ValueCounter HttpCallCount = new ValueCounter("HttpCallCount");
        public static readonly ValueCounter SqlConnCount = new ValueCounter("SqlConnCount");
    }

}
