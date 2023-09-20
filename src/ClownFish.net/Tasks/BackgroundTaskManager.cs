namespace ClownFish.Tasks;
#if NETCOREAPP

/// <summary>
/// 管理所有继承BackgroundTask的后台任务工具类
/// </summary>
public static class BackgroundTaskManager
{
    private static readonly List<BaseBackgroundTask> s_taskList = new List<BaseBackgroundTask>(64);


    /// <summary>
    /// 启用所有的BackgroundTask
    /// </summary>
    /// <param name="types"></param>
    public static void StartAll(params Type[] types)
    {
        if( types.IsNullOrEmpty() )
            return;

        if( s_taskList.Count > 0 )
            throw new InvalidOperationException("此方法不允许多次调用！");

        foreach(Type t in types ) {
            if( t.IsSubclassOf(typeof(BackgroundTask)) ) {
                StartSyncTask(t);
                continue;
            }

            if( t.IsSubclassOf(typeof(AsyncBackgroundTask)) ) {
                StartAsyncTask(t);
                continue;
            }
        }
    }


    private static void StartSyncTask(Type t)
    {
        BackgroundTask task = Activator.CreateInstance(t) as BackgroundTask;
        s_taskList.Add(task);

        using( ExecutionContext.SuppressFlow() ) {
            Thread thread = new Thread(task.Run);
            thread.Name = task.GetType().Name;
            thread.IsBackground = true;
            thread.Start();
        }

        Console2.Info("Start BackgroundTask: " + t.FullName);
    }


    private static void StartAsyncTask(Type t)
    {
        AsyncBackgroundTask task = Activator.CreateInstance(t) as AsyncBackgroundTask;
        s_taskList.Add(task);

        using( ExecutionContext.SuppressFlow() ) {
            Task.Run(async () => await task.RunAsync());
        }

        Console2.Info("Start AsyncBackgroundTask: " + t.FullName);
    }

    internal static DebugReportBlock GetReportBlock()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "BackgroundTask Information" };

        foreach( var task in s_taskList ) {
            string name = task.GetType().Name;
            long count = task.ExecuteCount.Get();
            long error = task.ErrorCount.Get();
            block.AppendLine($"{name}: {count.ToWString()},  {error.ToWString()}");
        }

        return block;
    }


    /// <summary>
    /// 获取所有后台任务的状态信息
    /// </summary>
    /// <returns></returns>
    public static List<BgTaskStatus> GetAllStatus()
    {
        List<BgTaskStatus> list = new List<BgTaskStatus>(s_taskList.Count);

        foreach( var task in s_taskList ) {
            BgTaskStatus x = new BgTaskStatus {
                TaskName = task.GetType().FullName,
                Kind = (task is AsyncBackgroundTask) ? 1 : 0,
                SleepSeconds = task.SleepSeconds.GetValueOrDefault(),
                CronValue = task.CronValue,
                Status = task.Status,
                ExecuteCount = task.ExecuteCount,
                ErrorCount = task.ErrorCount,
                LastRunTime = task.LastRunTime,
                LastStatus = task.LastStatus,
                NextRunTime = task.NextRunTime
            };
            list.Add(x);
        }

        return list.OrderBy(x => x.TaskName).ToList();
    }


    /// <summary>
    /// 激活某个休眠的任务
    /// </summary>
    /// <param name="taskName"></param>
    public static void ActivateTask(string taskName)
    {
        BaseBackgroundTask task = s_taskList.FirstOrDefault(x => x.GetType().FullName == taskName);
        task?.StopWait();
    }

}

#endif
