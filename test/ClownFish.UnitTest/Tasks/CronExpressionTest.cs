namespace ClownFish.UnitTest.Tasks;
#if NET6_0_OR_GREATER

using ClownFish.Tasks;

[TestClass]
public class CronExpressionTest
{
    [TestMethod]
    public void Test_1()
    {
        string[] cronArray = new string[] { "0/5 * * * * ?" ,          // 每 5 秒执行一次
                                            "0 0 1 * * ?" ,            // 每天 1:00 执行一次
                                            "0 30 14 ? * FRI" ,        // 每天 14:30 执行一次
                                            "0/2 * * * * ?",           // 表示每2秒 执行任务
                                            "0 0/2 * * * ?",           // 表示每2分钟 执行任务
                                            "0 0 2 1 * ?",             // 表示在每月的1日的凌晨2点调整任务
                                            "0 15 10 ? * MON-FRI",     // 表示周一到周五每天上午10:15执行作业
                                            "0 0 10,14,16 * * ?",      // 每天上午10点，下午2点，4点 
                                            "0 0/30 9-17 * * ?",       // 朝九晚五工作时间内每半小时 
                                            "0 0 12 ? * WED",          // 表示每个星期三中午12点 
                                            "0 0 12 * * ?",            // 每天中午12点触发 
                                            "0 15 10 ? * *",           // 每天上午10:15触发
                                            "0 15 10 * * ?",           // 每天上午10:15触发
                                            //"0 15 10 * * ? 2005",      // 2005年的每天上午10:15触发 
                                            "0 * 14 * * ?",            // 在每天下午2点到下午2:59期间的每1分钟触发
                                            "0 0/5 14 * * ?",          // 在每天下午2点到下午2:55期间的每5分钟触发 
                                            "0 0/5 14,18 * * ?",       // 在每天下午2点到2:55期间和下午6点到6:55期间的每5分钟触发
                                            "0 0-5 14 * * ?",          // 在每天下午2点到下午2:05期间的每1分钟触发 
                                            "0 10,44 14 ? 3 WED",      // 每年三月的星期三的下午2:10和2:44触发
                                            "0 15 10 ? * MON-FRI",     // 周一至周五的上午10:15触发
                                            "0 15 10 15 * ?",          // 每月15日上午10:15触发 
                                            "0 15 10 L * ?",           // 每月最后一日的上午10:15触发
                                            //"0 15 10 ? * 6L",          // 每月的最后一个星期五上午10:15触发 
                                            //"0 15 10 ? * 6L 2002-2105",// 2002年至2005年的每月的最后一个星期五上午10:15触发
                                            //"0 15 10 ? * 6#3",         // 每月的第三个星期五上午10:15触发
                                            // copy from https://cron.qqe2.com/
                                            // 注意：对于 “day of week 1-7” ，Quartz和Cronos的解释不一样，前者是：SUN-SAT，后者是 MON-SUN
        };


        foreach( string cronValue in cronArray ) {
            Console.WriteLine("-------------------------------");
            Console.WriteLine(cronValue);

            DateTime start = DateTime.Now;
            int count = 10;

            string s1 = Test_Quartz(cronValue, start, count);
            string s2 = Test_Cronos(cronValue, start, count);
            string s3 = Test_NbCron(cronValue, start, count);
            
            Console.WriteLine(s1);

            Assert.AreEqual(s1, s2);
            Assert.AreEqual(s1, s3);
        }
    }

    private string Test_Quartz(string cronValue, DateTime start, int count)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(start.ToTimeString() + "---now");

        Quartz.CronExpression cron = new Quartz.CronExpression(cronValue);

        DateTimeOffset current = new DateTimeOffset(start);

        for( int i = 0; i < count; i++ ) {
            DateTimeOffset? next = cron.GetNextValidTimeAfter(current);
            sb.AppendLine(next.Value.DateTime.ToLocalTime().ToTimeString());   // 使用“本地时区”

            current = next.Value;
        }
        return sb.ToString();
    }

    private string Test_Cronos(string cronValue, DateTime start, int count)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(start.ToTimeString() + "---now");

        Cronos.CronExpression cron = Cronos.CronExpression.Parse(cronValue, Cronos.CronFormat.IncludeSeconds);

        DateTimeOffset current = new DateTimeOffset(start);

        for( int i = 0; i < count; i++ ) {
            DateTimeOffset? next = cron.GetNextOccurrence(current, TimeZoneInfo.Local);   // 使用“本地时区”
            sb.AppendLine(next.Value.DateTime.ToTimeString());

            current = next.Value;
        }

        return sb.ToString();
    }

    private string Test_NbCron(string cronValue, DateTime start, int count)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(start.ToTimeString() + "---now");

        NbCronExpression cron = new NbCronExpression(cronValue);

        DateTime current = start;

        for( int i = 0; i < count; i++ ) {
            DateTime? next = cron.GetNextLocalTime(current);   // 使用“本地时区”
            sb.AppendLine(next.Value.ToTimeString());

            current = next.Value;
        }

        return sb.ToString();
    }


    [TestMethod]
    public void Test_2()
    {
        Console.WriteLine("Now: " + DateTime.Now.ToTimeString());

        ShowNext10TimeValue("0/5 * * * * ?");
        Console.WriteLine("---------------------------------");
        ShowNext10TimeValue("0 0 1 * * ?");
        Console.WriteLine("---------------------------------");
        ShowNext10TimeValue("0 30 14 ? * FRI");
    }

    private void ShowNext10TimeValue(string cronValue)
    {
        NbCronExpression cron = new NbCronExpression(cronValue);

        DateTime current = DateTime.Now;

        for( int i = 0; i <= 10; i++ ) {
            DateTime? next = cron.GetNextLocalTime(current);
            Console.WriteLine(next.Value.ToTimeString());
            current = next.Value;
        }
    }

}
#endif
