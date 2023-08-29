namespace PerformanceTest.Utils;

internal static class ThreadUtils
{
    internal static TimeSpan TestMultiThread(int threadCount, Action action)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        Task[] tasks = new Task[threadCount];

        for( int i = 0; i < threadCount; i++ ) {
            tasks[i] = Task.Run(action);
        }
        Task.WaitAll(tasks);

        stopwatch.Stop();
        return stopwatch.Elapsed;
    }
}
