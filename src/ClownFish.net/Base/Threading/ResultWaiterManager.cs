namespace ClownFish.Base.Threading;

#if NET46_OR_GREATER || NETCOREAPP

internal static class ResultWaiterManager
{
    private static readonly TSafeDictionary<string, ResultWaiter> s_dict = new TSafeDictionary<string, ResultWaiter>(1024);

    public static void Add(ResultWaiter waiter)
    {
        s_dict.Set(waiter.ResultId, waiter);
    }

    public static ResultWaiter Get(string resultId)
    {
        ResultWaiter waiter = s_dict.TryGet(resultId);

        if( waiter != null )
            s_dict.TryRemove(resultId, out _);

        return waiter;
    }

    public static void Remove(string resultId)
    {
        s_dict.TryRemove(resultId, out _);
    }
}


#endif
