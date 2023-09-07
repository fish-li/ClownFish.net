namespace ClownFish.UnitTest.Data.Events;

//[TestClass]
public class DbContextEventTest
{
    //[ClassInitialize]
    public static void Init(TestContext context)
    {
#if NETCOREAPP
        DbContextEventSubscriber.Start();
#endif

        DbContextEvent.OnConnectionOpened += DbContextEvent_OnConnectionOpened;
        //DbContextEvent.OnBeforeExecute += DbContextEvent_OnBeforeExecute;
        DbContextEvent.OnAfterExecute += DbContextEvent_OnAfterExecute;
        DbContextEvent.OnCommited += DbContextEvent_OnCommited;

#if NET6_0_OR_GREATER
        DbContextEvent.OnAfterExecuteBatch += DbContextEvent_OnAfterExecuteBatch;
#endif
    }

    

    private static void DbContextEvent_OnCommited(object sender, CommitTransEventArgs e)
    {

    }

    private static void DbContextEvent_OnAfterExecute(object sender, ExecuteCommandEventArgs e)
    {

    }

    //private static void DbContextEvent_OnBeforeExecute(object sender, ExecuteCommandEventArgs e)
    //{

    //}

#if NET6_0_OR_GREATER
    private static void DbContextEvent_OnAfterExecuteBatch(object sender, ExecuteBatchEventArgs e)
    {
    }
#endif

    private static void DbContextEvent_OnConnectionOpened(object sender, OpenConnEventArgs e)
    {

    }

    //[TestMethod]
    //public void Test_ConnectionOpened()
    //{
    //    using( DbContext db = DbContext.Create() ) {

    //        db.BeginTransaction();

    //        var query = db.CPQuery.Create("select getdate()");
    //        var xx = query.ExecuteScalar<string>();

    //        db.Commit();
    //    }
    //}

}



#if NETCOREAPP

internal class DbContextEventSubscriber : IObserver<DiagnosticListener>
{
    public static void Start()
    {
        DbContextEventSubscriber observer = new DbContextEventSubscriber();
        DiagnosticListener.AllListeners.Subscribe(observer);
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(DiagnosticListener listener)
    {
        switch( listener.Name ) {
            case "ClownFish.DALEvent":
                listener.Subscribe(new ClownFishDALEventObserver());
                break;
        }
    }
}

public class ClownFishDALEventObserver : IObserver<KeyValuePair<string, object>>
{
    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(KeyValuePair<string, object> kvp)
    {
        switch( kvp.Key ) {

            case "ConnectionOpened": {
                    ConnectionOpened(kvp.Value);
                    return;
                }

            case "BeforeExecute": {
                    BeforeExecute(kvp.Value);
                    return;
                }

            case "AfterExecute": {
                    AfterExecute(kvp.Value);
                    return;
                }

            case "AfterExecuteBatch": {
                    AfterExecuteBatch(kvp.Value);
                    return;
                }

            case "OnCommit": {
                    OnCommit(kvp.Value);
                    return;
                }
        }
    }

    private void ConnectionOpened(object eventData)
    {

    }

    private void BeforeExecute(object eventData)
    {

    }

    private void AfterExecute(object eventData)
    {

    }

    private void AfterExecuteBatch(object eventData)
    {

    }

    private void OnCommit(object eventData)
    {

    }
}
#endif
