namespace ClownFish.MQ.Pipeline;
#if NET6_0_OR_GREATER
/// <summary>
/// 消息处理管道
/// </summary>
/// <typeparam name="T">消息的数据类型参数</typeparam>
public sealed class MessagePipeline<T> where T : class
{
    private readonly BaseMessageHandler<T> _handler;
    private readonly int _retryCount;
    private readonly int _waitMilliseconds;

    /// <summary>
    /// 当前管道中所使用的消息处理器实例
    /// </summary>
    public BaseMessageHandler<T> HandlerInstance => _handler;


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="handler">消息处理器实例</param>
    /// <param name="retryCount">消息处理失败时的重试次数</param>
    /// <param name="waitMilliseconds">消息处理失败时的重试操作的等待时间</param>
    public MessagePipeline(BaseMessageHandler<T> handler, int retryCount, int waitMilliseconds)
    {
        if( handler == null )
            throw new ArgumentOutOfRangeException(nameof(handler));

        this._handler = handler;
        this._retryCount = (retryCount < 0 ? 0 : retryCount) + 1;
        this._waitMilliseconds = waitMilliseconds < 0 ? 0 : waitMilliseconds;

        handler.OnInit();
    }



    /// <summary>
    /// 将一条消息推送到管道中处理
    /// </summary>
    /// <param name="request"></param>
    /// <returns>如果消息处理成功，返回 true, 失败，返回 false</returns>
    public bool PushMessage(MqRequest request)
    {
        if( request == null )
            return true;

        for( int i = 0; i < _retryCount; i++ ) {

            PipelineContext<T> context = new PipelineContext<T>(request, _handler, false, i);
            using( context ) {
                HandleMessage(context);    // ##### 将消息交给“消息处理器”来处理 ######
            }

            // 没有出现异常就认为处理成功
            if( context.LastException == null ) {
                return true;        // ##### 消息已成功处理 ######
            }
            else {
                // 如果开启了日志，异常会被记录到日志。如果日志没有开启，就输出到控制台
                if( _handler.EnableLog == false ) {
                    ConsoleException(context.LastException);
                }

                // 对于不需要重试的场景，就跳出循环
                if( _handler.IsNeedRetry(context) == false ) {
                    break;
                }

                // 消息多次尝试都失败，只能按死消息来处理
                if( i == _retryCount - 1 ) {
                    ProcessDeadMessage(context);
                }
            }

            if( _waitMilliseconds > 0 ) {
                Thread.Sleep(_waitMilliseconds);
            }
        }

        return false;
    }

    private void HandleMessage(PipelineContext<T> context)
    {
        try {
            _handler.ValidateMessage(context);
            _handler.PrepareMessage(context);
            _handler.SaveMessage(context);
            _handler.ProcessMessage(context);
            _handler.AfterProcess(context);
            _handler.SaveState(context);
        }
        catch( AbortRequestException ) {
            // 提前结束请求
        }
        catch( Exception ex ) {
            context.SetException(ex);
            PipelineUtils.SafeCall(_handler.OnError, context);
        }
        finally {
            PipelineUtils.SafeCall(_handler.OnEnd, context);
        }
    }

    private void ConsoleException(Exception ex)
    {
        Console2.Error(ex);
    }

    private void ProcessDeadMessage(PipelineContext<T> context)
    {
        PipelineUtils.SafeCall(_handler.ProcessDeadMessage, context);
    }


}


#endif
