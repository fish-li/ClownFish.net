namespace ClownFish.Base;

/// <summary>
/// 一个简单的线程安全的消息队列，主要特点是可以限制队列长度。
/// </summary>
public sealed class ObjectQueue
{
    private readonly object _lock = new object();
    private readonly Queue<object> _queue = new Queue<object>(1024);

    private readonly int _maxQueueLength;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="maxQueueLength"></param>
    public ObjectQueue(int maxQueueLength)
    {
        _maxQueueLength = maxQueueLength;
    }


    /// <summary>
    /// 将消息压到消息队列
    /// </summary>
    /// <param name="message"></param>
    public bool Enqueue(object message)
    {
        lock( _lock ) {
            if( _queue.Count >= _maxQueueLength )
                return false;

            _queue.Enqueue(message);
            return true;
        }
    }

    /// <summary>
    /// 从消息队列取出一条消息
    /// </summary>
    /// <returns></returns>
    public object Dequeue()
    {
        lock( _lock ) {
            if( _queue.Count == 0 )
                return null;

            return _queue.Dequeue();
        }
    }
}
