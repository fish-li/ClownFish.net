namespace ClownFish.Base.Threading;

internal static class ResultWaiterManager
{
    private static readonly CacheDictionary<ResultWaiter> s_dict = new CacheDictionary<ResultWaiter>();

    public static void Add(ResultWaiter waiter, TimeSpan timeout)
    {
        s_dict.Set(waiter.ResultId, waiter, DateTime.Now.Add(timeout).AddMinutes(5));
    }

    public static ResultWaiter Get(string resultId)
    {
        ResultWaiter waiter = s_dict.Get(resultId);

        if( waiter != null )
            s_dict.Remove(resultId);

        return waiter;
    }
}


