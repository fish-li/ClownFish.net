#if NETCOREAPP

using Cronos;
namespace ClownFish.Tasks;


internal sealed class NbCronExpression
{
    private readonly CronExpression _cronExpression;

    public NbCronExpression(string expression)
    {
        _cronExpression = CronExpression.Parse(expression, Cronos.CronFormat.IncludeSeconds);
    }

    public DateTime? GetNextUtcTime(DateTime dateTime)
    {
        return _cronExpression.GetNextOccurrence(dateTime.ToUniversalTime());
    }


    public DateTime? GetNextLocalTime(DateTime dateTime)
    {
        DateTime? value = GetNextUtcTime(dateTime);
        if( value.HasValue == false )
            return null;
        else
            return value.Value.ToLocalTime();
    }
}
#endif
