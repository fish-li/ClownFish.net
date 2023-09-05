namespace ClownFish.Base.Threading;

/// <summary>
/// 异步的结果等待器
/// </summary>
public sealed class ResultWaiter : IDisposable
{
    private TaskCompletionSource<object> _taskCompletionSource;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationTokenRegistration _tokenRegistration;

    private volatile object _result;

    /// <summary>
    /// ResultId
    /// </summary>
    public readonly string ResultId = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 结束等待，设置完成结果
    /// </summary>
    /// <param name="result"></param>
    public void SetResult(object result)
    {
        _result = result;
        _taskCompletionSource?.TrySetResult(result);
    }

    /// <summary>
    /// 结束等待，设置执行为异常
    /// </summary>
    /// <param name="ex"></param>
    public void SetException(Exception ex)
    {
        _taskCompletionSource?.TrySetException(ex);
    }


    /// <summary>
    /// 等待结果
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<object> WaitAsync(TimeSpan timeout)
    {
        // https://www.coder.work/article/246268
        _taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

        _cancellationTokenSource = new CancellationTokenSource(timeout);
        _tokenRegistration = _cancellationTokenSource.Token.Register(() => _taskCompletionSource.TrySetCanceled(), useSynchronizationContext: false);

        ResultWaiterManager.Add(this, timeout);

        try {
            return await _taskCompletionSource.Task;
        }
        catch( OperationCanceledException ) {
            //Console2.Info($"ResultWaiter.WaitAsync.OperationCanceledException, result is null: {_result == null}, hasCallback: {_hasCallback}");
        }

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
        if( _cancellationTokenSource != null ) {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            _tokenRegistration.Dispose();
        }
    }
}



