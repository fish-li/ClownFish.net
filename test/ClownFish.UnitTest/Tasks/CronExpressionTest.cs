namespace ClownFish.UnitTest.Tasks;
#if NET6_0_OR_GREATER

using ClownFish.Tasks;

[TestClass]
public class CronExpressionTest
{
    [TestMethod]
    public void Test()
    {
        string cronValue = "0/5 * * * * ? ";
        DateTime start = DateTime.Now;
        int count = 100;

        string s1 = Test1(cronValue, start, count);
        string s2 = Test2(cronValue, start, count);
        string s3 = Test3(cronValue, start, count);

        Console.WriteLine(s1);
        Assert.AreEqual(s1, s2);
        Assert.AreEqual(s1, s3);
    }

    private string Test1(string cronValue, DateTime start, int count)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(start.ToTimeString());

        Quartz.CronExpression cron = new Quartz.CronExpression(cronValue);

        DateTimeOffset current = new DateTimeOffset(start);

        for( int i = 0; i < count; i++ ) {
            DateTimeOffset? next = cron.GetNextValidTimeAfter(current);
            sb.AppendLine(next.Value.DateTime.ToTimeString());

            current = next.Value;
        }
        return sb.ToString();
    }

    private string Test2(string cronValue, DateTime start, int count)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(start.ToTimeString());

        Cronos.CronExpression cron = Cronos.CronExpression.Parse(cronValue, Cronos.CronFormat.IncludeSeconds);

        DateTime current = start;        

        for( int i = 0; i < count; i++ ) {
            DateTime? next = cron.GetNextOccurrence(current.ToUniversalTime());
            sb.AppendLine(next.Value.ToTimeString());

            current = next.Value;
        }

        return sb.ToString();
    }

    private string Test3(string cronValue, DateTime start, int count)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(start.ToTimeString());

        NbCronExpression cron = new NbCronExpression(cronValue);

        DateTime current = start;        

        for( int i = 0; i < count; i++ ) {
            DateTime? next = cron.GetNextUtcTime(current);
            sb.AppendLine(next.Value.ToTimeString());

            current = next.Value;
        }

        return sb.ToString();
    }


}
#endif
