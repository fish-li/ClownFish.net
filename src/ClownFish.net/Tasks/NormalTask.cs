namespace ClownFish.Tasks;
#if NETCOREAPP
/// <summary>
/// 一个普通的一次性任务，使用【当前调用线程】来执行。
/// 任务在的执行后会有日志记录和执行次数统计。
/// </summary>
public abstract class NormalTask :  BaseTaskObject
{
    /// <summary>
    /// 任务的启动传入参数
    /// </summary>
    protected object _args;

    /// <summary>
    /// BgTaskExecuteContext实例
    /// </summary>
    protected BgTaskExecuteContext Context { get; private set; }


    /// <summary>
    /// 开始执行任务
    /// </summary>
    public void Run(object args = null)
    {
        _args = args;
        Execute0();
    }    

    private void Execute0()
    {
        using( BgTaskExecuteContext context = new BgTaskExecuteContext(this) ) {
            this.Context = context;

            try {
                Execute();
            }
            catch( Exception ex ) {
                context.SetException(ex);
                throw;
            }
        }

        this.Context = null;
    }


    /// <summary>
    /// 执行任务的主体过程。
    /// </summary>
    public abstract void Execute();

}
#endif
