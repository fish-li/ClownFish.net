#if NET6_0_OR_GREATER

using System.Threading.Channels;

namespace ClownFish.MQ.MMQ;

/// <summary>
/// 内存队列的工作模式
/// </summary>
public enum MmqWorkMode
{
    /// <summary>
    /// 同步工作模式
    /// </summary>
    Sync,
    /// <summary>
    /// 异步工作模式
    /// </summary>
    Async
}

/// <summary>
/// 用内存实现的消息队列
/// </summary>
public sealed class MemoryMesssageQueue<T> where T : class
{
    private readonly MmqWorkMode _workMode;

    private readonly BlockingCollection<T> _syncChannel;

    private readonly Channel<T> _asyncChannel;

    private readonly ValueCounter _counter = new ValueCounter();

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="workMode">队列的工作模式，同步或者异步</param>
    /// <param name="capacity">队列中允许最大的消息数量</param>
    public MemoryMesssageQueue(MmqWorkMode workMode, int capacity = 10_0000)
    {
        _workMode = workMode;

        if( workMode == MmqWorkMode.Sync ) {
            _syncChannel = new BlockingCollection<T>(capacity);
            _asyncChannel = null;
        }
        else {
            _syncChannel = null;

            _asyncChannel = Channel.CreateBounded<T>(new BoundedChannelOptions(capacity) {
                FullMode = BoundedChannelFullMode.DropOldest,
            });
        }
    }


    /// <summary>
    /// 获取队列的长度
    /// </summary>
    public long Count => _counter.Get();


    /// <summary>
    /// 将一个数据项添加到队列中
    /// </summary>
    /// <param name="data"></param>
    public void Write(T data)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        if( _syncChannel == null )
            throw new NotSupportedException("当前工作模式下不支持此调用。");

        _syncChannel.Add(data);
        _counter.Increment();
    }


    /// <summary>
    /// 从队列中获取一个数据项并从队列中删除
    /// </summary>
    /// <returns></returns>
    internal T Read(CancellationToken? cancellationToken)
    {
        if( _syncChannel == null )
            throw new NotSupportedException("当前工作模式下不支持此调用。");

        try {
            if( cancellationToken.HasValue == false )
                return _syncChannel.Take();
            else
                return _syncChannel.Take(cancellationToken.Value);
        }
        finally {
            _counter.Decrement();
        }
    }


    /// <summary>
    /// 将一个数据项添加到队列中
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task WriteAsync(T data)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        if( _asyncChannel == null )
            throw new NotSupportedException("当前工作模式下不支持此调用。");

        await _asyncChannel.Writer.WriteAsync(data);
        _counter.Increment();
    }

    /// <summary>
    /// 从队列中获取一个数据项并从队列中删除
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task<T> ReadAsync(CancellationToken? cancellationToken)
    {
        if( _asyncChannel == null )
            throw new NotSupportedException("当前工作模式下不支持此调用。");

        try {
            if( cancellationToken.HasValue == false )
                return await _asyncChannel.Reader.ReadAsync();
            else
                return await _asyncChannel.Reader.ReadAsync(cancellationToken.Value);
        }
        finally {
            _counter.Decrement();
        }
    }

}
#endif
