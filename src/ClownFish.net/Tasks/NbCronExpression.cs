using Cronos;
//using Quartz;
namespace ClownFish.Tasks;

/// <summary>
/// 
/// </summary>
public sealed class NbCronExpression
{
    private readonly CronExpression _cronExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expression"></param>
    public NbCronExpression(string expression)
    {
        _cronExpression = Cronos.CronExpression.Parse(expression, Cronos.CronFormat.IncludeSeconds);

        //_cronExpression = new Quartz.CronExpression(expression);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public DateTime? GetNextUtcTime(DateTime dateTime)
    {
        return _cronExpression.GetNextOccurrence(dateTime.ToUniversalTime());   // Cronos

        //DateTimeOffset? next = _cronExpression.GetNextValidTimeAfter(dateTime);  // Quartz
        //return next?.DateTime;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public DateTime? GetNextLocalTime(DateTime dateTime)
    {
        DateTimeOffset? next = _cronExpression.GetNextOccurrence(new DateTimeOffset(dateTime), TimeZoneInfo.Local);   // Cronos
        return next?.DateTime;

        //DateTimeOffset? next = _cronExpression.GetNextValidTimeAfter(dateTime);  // Quartz
        //return next?.DateTime.ToLocalTime();
    }
}
