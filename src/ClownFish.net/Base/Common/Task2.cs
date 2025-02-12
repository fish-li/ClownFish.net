namespace ClownFish.Base;

/// <summary>
/// Task2
/// </summary>
public static class Task2
{
    /// <summary>
    /// Delay without TaskCanceledException
    /// </summary>
    /// <param name="millisecondsDelay"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
    {
        try {
            await Task.Delay(millisecondsDelay, cancellationToken);
        }
        catch( TaskCanceledException ) { }
    }



    private static Task s_completedTask;

    /// <summary>
    /// 表示一个已完成的任务
    /// </summary>
    public static Task CompletedTask {     // 这个属性在 net46 中引入
        get {
            Task task = s_completedTask;
            if( task == null ) {
                task = (s_completedTask = Task.FromResult<int>(0));
            }
            return task;
        }
    }


}
