namespace ClownFish.UnitTest.WebClient;

[TestClass]
public class HttpClientEventTest
{
    [ClassInitialize]
    public static void Init(TestContext context)
    {
#if NETCOREAPP
        TestEventSubscriber.Start();
#endif

        HttpClientEvent.OnCreateRequest += HttpClientEvent_OnCreateRequest;
        HttpClientEvent.OnBeforeSendRequest += HttpClientEvent_OnBeforeSendRequest;
        HttpClientEvent.OnRequestFinished += HttpClientEvent_OnRequestFinished;
    }

    private static void HttpClientEvent_OnCreateRequest(object sender, BeforeCreateRequestEventArgs e)
    {
    }

    private static void HttpClientEvent_OnBeforeSendRequest(object sender, BeforeSendEventArgs e)
    {
    }
    private static void HttpClientEvent_OnRequestFinished(object sender, RequestFinishedEventArgs e)
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void Test_BeforeSend()
    {
        HttpOption option = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };
        TestHttpClient clent = new TestHttpClient(option);
        HttpClientEvent.BeforeSend(clent);
    }

    [TestMethod]
    public void Test_RequestFinished()
    {
        HttpOption option = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };

        TestHttpClient clent = new TestHttpClient(option);
        HttpClientEvent.RequestFinished(clent, null, null);
    }
}


#if NETCOREAPP

internal class TestEventSubscriber : IObserver<DiagnosticListener>
{
    public static void Start()
    {
        TestEventSubscriber observer = new TestEventSubscriber();
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
            case "ClownFish.HttpClientEvent":
                listener.Subscribe(new ClownFishHttpClientEventObserver());
                break;
        }
    }
}

public class ClownFishHttpClientEventObserver : IObserver<KeyValuePair<string, object>>
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

            case "OnBeforeSendRequest": {
                    OnBeforeSendRequest(kvp.Value);
                    return;
                }

            case "OnRequestFinished": {
                    OnRequestFinished(kvp.Value);
                    return;
                }
        }
    }

    private void OnBeforeSendRequest(object eventData)
    {

    }

    private void OnRequestFinished(object eventData)
    {
        throw new NotImplementedException();
    }
}
#endif
