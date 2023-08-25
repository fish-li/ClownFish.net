namespace ClownFish.Base;

/// <summary>
/// 计数器
/// </summary>
public class ValueCounter
{
    /// <summary>
    /// 标签名称
    /// </summary>
    public string Label { get; private set; }

    /// <summary>
    /// 当前计数值
    /// </summary>
    private long _count;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="label"></param>
    public ValueCounter(string label = "xx")
    {
        this.Label = label;
    }


    /// <summary>
    /// 递增 计数器
    /// </summary>
    /// <returns></returns>
    public long Increment()
    {
        return Interlocked.Increment(ref _count);
    }


    /// <summary>
    /// 递减 计数器
    /// </summary>
    /// <returns></returns>
    public long Decrement()
    {
        return Interlocked.Decrement(ref _count);
    }

    /// <summary>
    /// 增加计数器的值
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public long Add(long value)
    {
        return Interlocked.Add(ref _count, value);
    }


    /// <summary>
    /// 指定计数器的值，并返回原先的值
    /// </summary>
    /// <param name="value"></param>
    public long Set(long value)
    {
        return Interlocked.Exchange(ref _count, value);
    }


    /// <summary>
    /// 指定一个时间做为计数器的值，并返回原先的值
    /// </summary>
    /// <param name="value"></param>
    public long Set(DateTime value)
    {
        return Interlocked.Exchange(ref _count, value.Ticks);
    }

    /// <summary>
    /// 获取计数器的值
    /// </summary>
    /// <returns></returns>
    public long Get()
    {
        return Interlocked.Read(ref _count);
    }

    /// <summary>
    /// 获取计数器的值，并转成DateTime类型
    /// </summary>
    /// <returns></returns>
    public DateTime GetAsDateTime()
    {
        return new DateTime(Interlocked.Read(ref _count));
    }

    /// <summary>
    /// 获取计数器的值，并重置
    /// </summary>
    /// <returns></returns>
    public long Reset()
    {
        return Interlocked.Exchange(ref _count, 0L);
    }


    /// <summary>
    /// 将计数器隐式转成long类型
    /// </summary>
    /// <param name="counter"></param>
    public static implicit operator long(ValueCounter counter)
    {
        if( counter == null )
            throw new ArgumentNullException(nameof(counter));

        return counter.Get();
    }

    /// <summary>
    /// 将计数器的值转成字符串
    /// </summary>
    /// <returns></returns>
    public string AsString()
    {
        return this.Get().ToString();
    }


    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.Label + "=" + this.Get().ToString();
    }
}
