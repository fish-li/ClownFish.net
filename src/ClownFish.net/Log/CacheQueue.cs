using ClownFish.Log.Writers;

namespace ClownFish.Log;

/// <summary>
/// 为了方便而定义的一个弱类型接口
/// </summary>
internal interface ICacheQueue
{
    int Add(object msg);

    int Flush();
}


/// <summary>
/// 缓存日志信息的写入队列
/// </summary>
/// <typeparam name="T"></typeparam>
internal class CacheQueue<T> : ICacheQueue where T : class, IMsgObject
{
    /// <summary>
    /// 队列锁
    /// </summary>
    private readonly object _lock = new object();

    /// <summary>
    /// 静态缓冲队列
    /// </summary>
    private readonly List<T> _list = new List<T>(LoggingOptions.MaxCacheQueueLength);

    /// <summary>
    /// 写入器实例数组，注意：允许一个类型配置多个写入方式
    /// </summary>
#pragma warning disable IDE0044 // 下面这个字段会在单元测试中修改，所以不能设置为“只读”！
    private ILogWriter[] _writers = WriterFactory.GetWriters(typeof(T));
#pragma warning restore IDE0044 // 添加只读修饰符


    #region ICacehQueue 成员

    public int Add(object msg)
    {
        // 将弱类型变成强类型
        T msg2 = (T)msg;
        return Add(msg2);
    }

    #endregion

    /// <summary>
    /// 写入一条日志消息到缓冲队列
    /// </summary>
    /// <param name="msg"></param>
    public int Add(T msg)
    {
        // 外部调用写日志时，其实只是将日志消息写入静态队列，用于缓冲写入压力
        lock( _lock ) {

            // 先判断列表长度，避免Writer遇到故障导致消息大量堆积，占用大量内存
            if( _list.Count < LoggingOptions.MaxCacheQueueLength ) {
                _list.Add(msg);
                ClownFishCounters.Logging.InQueueCount.Increment();
                return 1;
            }
            else {
                ClownFishCounters.Logging.GiveupCount.Increment();
                return 0;
            }
        }
    }


    /// <summary>
    /// 供外部定时器调用，一次性写入所有等待消息
    /// 此方法由定时器线程调用。
    /// </summary>
    public int Flush()
    {
        List<T> tempList = null;

        lock( _lock ) {
            if( _list.Count > 0 ) {
                // 将静态队列的数据转移到临时队列，避免在后面写操作时长时间占用锁
                tempList = (from x in _list select x).ToList();

                // 清空静态队列
                _list.Clear();
            }
        }

        if( tempList.IsNullOrEmpty() )
            return 0;         // 没有需要写入的日志信息

        if( ClownFishInit.AppExitToken.IsCancellationRequested ) {
            return -2;
        }

        if( tempList.Count > ClownFishCounters.Logging.MaxBatchSize.Get() ) 
            ClownFishCounters.Logging.MaxBatchSize.Set(tempList.Count);
        

        // 如果类型没有配置日志序列化器，就忽略
        if( _writers.IsNullOrEmpty() )
            return -1;

        // 一次写入大量的日志可能会耗费大量内存，甚至会出现OOM，所以这里要分批处理
        if( tempList.Count > LoggingOptions.WriteListBatchSize ) {
            List<List<T>> batchList = tempList.SplitList(int.MaxValue, LoggingOptions.WriteListBatchSize);
            foreach( var batch in batchList )
                WriteList(batch);
            return 2;
        }
        else {
            WriteList(tempList);
            return 1;
        }
    }


    private void WriteList(List<T> tempList)
    {
        foreach( var writer in _writers ) {
            try {
                writer.WriteList(tempList);
            }
            catch( Exception ex ) {
                LogHelper.RaiseErrorEvent(ex);
            }
        }
    }




}
