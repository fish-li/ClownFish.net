namespace ClownFish.Base.Threading;

/// <summary>
/// 异步的结果等待器
/// </summary>
public sealed class ResultWaiter : IDisposable
{
    private TaskCompletionSource<object> _taskCompletionSource;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationTokenRegistration _tokenRegistration;

    private readonly string _resultId;

    private volatile object _result;
    private volatile bool _isEnd = false;

    /// <summary>
    /// ResultId
    /// </summary>
    public string ResultId => _resultId;

    /// <summary>
    /// ctor
    /// </summary>
    public ResultWaiter() : this(Guid.NewGuid().ToString("N")) { }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="resultId">用于从字典表中查找当前对象的ID，注意：此参数一定要唯一，建议使用GUID字符串</param>
    public ResultWaiter(string resultId)
    {
        if( resultId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(resultId));

        _resultId = resultId;

        // https://www.coder.work/article/246268
        _taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

        ResultWaiterManager.Add(this);
    }

    /// <summary>
    /// 结束等待，设置完成结果
    /// </summary>
    /// <param name="result"></param>
    public bool SetResult(object result)
    {
        if( _isEnd )
            return false;

        if( _result != null )
            return false;

        _result = result;
        return _taskCompletionSource.TrySetResult(result);
    }

    /// <summary>
    /// 结束等待，设置执行为异常
    /// </summary>
    /// <param name="ex"></param>
    public bool SetException(Exception ex)
    {
        if( _isEnd )
            return false;

        return _taskCompletionSource.TrySetException(ex);
    }


    /// <summary>
    /// 等待结果
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<object> WaitAsync(TimeSpan timeout)
    {
        if( _result != null )
            return _result;

        _cancellationTokenSource = new CancellationTokenSource(timeout);
        _tokenRegistration = _cancellationTokenSource.Token.Register(() => _taskCompletionSource.TrySetCanceled(), useSynchronizationContext: false);

        if( _result != null )
            return _result;

        try {
            return await _taskCompletionSource.Task;
        }
        catch( OperationCanceledException ) {
            //Console2.Info($"ResultWaiter.WaitAsync.OperationCanceledException, result is null: {_result == null}, hasCallback: {_hasCallback}");
        }

        // 在多线程并发时，即使另外一个线程拿到当前对象，当前对象也是一个无效状态，即调用 SetXXX 方法不起作用
        _isEnd = true;

        // 通常来说，应该在OperationCanceledException时直接返回 null，
        // 但是可能会有2种极限场景：1，在执行TrySetResult的过程中占用了少量时间最终导致了超时，2，有可能TrySetResult时刚好到达超时时间，
        // 所以，这里以变量为准做为返回结果
        return _result;
    }


    /// <summary>
    /// GetById
    /// </summary>
    /// <param name="resultId"></param>
    /// <returns></returns>
    public static ResultWaiter GetById(string resultId)
    {
        return ResultWaiterManager.Get(resultId);
    }

    void IDisposable.Dispose()
    {
        _isEnd = true;

        if( _cancellationTokenSource != null ) {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            _tokenRegistration.Dispose();
        }

        if( _taskCompletionSource != null ) {
            _taskCompletionSource = null;
            ResultWaiterManager.Remove(this.ResultId);
        }
    }
}



