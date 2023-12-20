namespace ClownFish.Log;

internal static class CacheQueueManager
{
    /// <summary>
    /// 保存 【Type / ICacehQueue】的映射字典
    /// </summary>
    private static readonly Dictionary<Type, ICacheQueue> s_queueDict = new Dictionary<Type, ICacheQueue>(128);

    internal static void Start(List<DataTypeWriterMap> list)
    {
        if( list.IsNullOrEmpty() )
            return;

        foreach( var x in list ) {
            Type type = typeof(CacheQueue<>).MakeGenericType(x.DataType);
            ICacheQueue queue = (ICacheQueue)Activator.CreateInstance(type);
            s_queueDict[x.DataType] = queue;
        }


        ThreadUtils.Run2("CacheQueueManager_Flush", "ClownFishLogWriter", ThreadWorker);
    }


    private static void ThreadWorker()
    {
        int period = LogConfig.Instance?.TimerPeriod ?? 0;

        if( period <= 0 )
            period = 500;

        while( true ) {

            Thread.Sleep(period);

            try {
                Flush();
            }
            catch( Exception ex ) {
                //后台线程只能吃掉异常，否则程序就崩溃了，
                //不过，理论上不应该出现异常，这里只是一种预防措施
                LogHelper.RaiseErrorEvent(ex);
            }

            ClownFishCounters.Logging.QueueFlushCount.Increment();
        }
    }


    private static void Flush()
    {
        foreach( var kv in s_queueDict )
            kv.Value.Flush();
    }


    public static ICacheQueue GetCacheQueue<T>() where T : class, IMsgObject
    {
        Type t = typeof(T);

        if( s_queueDict.TryGetValue(t, out ICacheQueue queue) )
            return queue;
        else
            throw new NotSupportedException("不支持使用未注册的日志数据类型：" + t.FullName);
    }




}
