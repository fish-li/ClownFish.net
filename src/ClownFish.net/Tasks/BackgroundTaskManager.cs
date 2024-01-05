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
    public static int StartAll(params Type[] types)
    {
        if( types.IsNullOrEmpty() )
            return 0;

        if( s_taskList.Count > 0 )
            throw new InvalidOperationException("此方法不允许多次调用！");

        foreach(Type t in types ) {

            if( t.ModuleIsEnable() == false ) {
                Console2.Info($"BackgroundTask {t.FullName} 已配置为 不启用");
                continue;
            }

            if( t.IsSubclassOf(typeof(BackgroundTask)) ) {
                StartSyncTask(t);
                continue;
            }

            if( t.IsSubclassOf(typeof(AsyncBackgroundTask)) ) {
                StartAsyncTask(t);
                continue;
            }
        }
        return s_taskList.Count;
    }


    private static void StartSyncTask(Type t)
    {
        BackgroundTask task = Activator.CreateInstance(t) as BackgroundTask;
        s_taskList.Add(task);

        Console2.Info("Start BackgroundTask: " + t.FullName);

        ThreadUtils.Run2("BackgroundTask_Start", task.GetType().Name, task.Run);
    }


    private static void StartAsyncTask(Type t)
    {
        AsyncBackgroundTask task = Activator.CreateInstance(t) as AsyncBackgroundTask;
        s_taskList.Add(task);

        Console2.Info("Start BackgroundTask: " + t.FullName);

        ThreadUtils.RunAsync("AsyncBackgroundTask_Start", task.RunAsync);        
    }

    internal static DebugReportBlock GetReportBlock()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "BackgroundTask Information" };

        foreach( var task in s_taskList ) {
            string name = task.GetType().FullName;
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
    public static int ActivateTask(string taskName)
    {
        BaseBackgroundTask task = s_taskList.FirstOrDefault(x => x.GetType().FullName == taskName);
        task?.StopWait();
        return task == null ? 0 : 1;
    }


    internal static BaseBackgroundTask GetTaskInstance(string taskName)
    {
        return s_taskList.FirstOrDefault(x => x.GetType().FullName == taskName);
    }

    internal static void StopAll()   // 单元测试用
    {
        foreach(var task in s_taskList ) {
            task.OnAppExit();
        }

        s_taskList.Clear();
    }
}

#endif
