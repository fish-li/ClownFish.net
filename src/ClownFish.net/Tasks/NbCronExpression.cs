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

    public DateTime? GetNextTime(DateTime dateTime)
    {
        return _cronExpression.GetNextOccurrence(dateTime.ToUniversalTime());
    }
}
#endif
