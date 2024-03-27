#if NETCOREAPP

using System.Net.Http;
using ClownFish.WebClient.V2;

namespace ClownFish.UnitTest.Log.Logging;
[TestClass]
public class HttpClientLogger2Test
{
    // 模拟 HttpClient 发布事件
    private static readonly DiagnosticListener s_diagnosticSource = new DiagnosticListener("HttpHandlerDiagnosticListener");

    static HttpClientLogger2Test()
    {
        HttpClientLogger2.Init();
    }

    [TestMethod]
    public void Test1()
    {
        using OprLogScope scope = OprLogScope.Start();

        PublishEvent();
        PublishEvent2();

        List<StepItem> steps = scope.GetStepItems();
        Assert.AreEqual(2, steps.Count);

        string details = scope.GetOprDetails();
        Console.WriteLine(details);

        Assert.IsTrue(details.Contains("[ExceptionMessage]: 调用超时."));
    }


    [TestMethod]
    public void Test2()
    {
        PublishEvent();
        PublishEvent2();
    }

    private void PublishEvent()
    {
        object data1 = new {
            Request = new HttpRequestMessage(HttpMethod.Get, "http://www.abc.com/aa/bb.aspx"),
        };
        s_diagnosticSource.Write("System.Net.Http.Request", data1);


        Thread.Sleep(50);
        object data2 = new {
            Exception = ExceptionHelper.CreateException()
        };
        s_diagnosticSource.Write("System.Net.Http.Exception", data2);


        Thread.Sleep(50);
        object data3 = new {
            RequestTaskStatus = TaskStatus.RanToCompletion,
            Response = CreateResponse()
        };
        s_diagnosticSource.Write("System.Net.Http.Response", data3);
    }

    private HttpResponseMessage CreateResponse()
    {
        HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        response.Headers.Add("x-name1", "11111111");
        response.Content = HttpObjectUtils.CreateRequestMessageBody3(SerializeFormat.Form, new { a = 2, b = 3, c = "abc" });
        response.Content.Headers.TryAddWithoutValidation("Content-Length", "11");
        return response;
    }


    private void PublishEvent2()
    {
        object data1 = new {
            Request = new HttpRequestMessage(HttpMethod.Get, "http://www.abc.com/aa/cc.aspx")
        };
        s_diagnosticSource.Write("System.Net.Http.Request", data1);



        Thread.Sleep(50);
        object data4 = new {
            RequestTaskStatus = TaskStatus.Canceled,
            Response = (HttpResponseMessage)null
        };
        s_diagnosticSource.Write("System.Net.Http.Response", data4);
    }



    [TestMethod]
    public void Test_x1()
    {
        HttpClientEventSubscriber x = new HttpClientEventSubscriber();
        x.OnCompleted();
        x.OnError(null);
    }


    [TestMethod]
    public void Test_x2()
    {
        HttpClientEventObserver x = new HttpClientEventObserver();
        x.OnCompleted();
        x.OnError(null);
    }

    [TestMethod]
    public void Test_TryReplaceContent()
    {
        HttpOption httpOption = new HttpOption {
            Method = "POST",
            Url = "http://www.abc.com/aa/bb",
            Data = new {
                a = 2,
                b = 3,
            },
            Format = SerializeFormat.Json,
            Header = new {
                Content_Length = 20
            }
        };

        HttpRequestMessage requestMessage = HttpObjectUtils.CreateRequestMessage(httpOption);
        HttpContent content1 = requestMessage.Content;

        Assert.AreEqual(3, HttpClientEventObserver.TryReplaceContent(requestMessage));
        HttpContent content2 = requestMessage.Content;

        Assert.IsTrue(object.ReferenceEquals(content1, content2));
    }

    [TestMethod]
    public void Test_TryReplaceContent1()
    {
        HttpOption httpOption = new HttpOption {
            Method = "POST",
            Url = "http://www.abc.com/aa/bb",
            Data = new {
                a = 2,
                b = 3,
            }.ToJson().GetBytes(),
            Format = SerializeFormat.Json,
            Header = new {
                Content_Length = 20
            }
        };

        HttpRequestMessage requestMessage = HttpObjectUtils.CreateRequestMessage(httpOption);
        HttpContent content1 = requestMessage.Content;

        Assert.AreEqual(1, HttpClientEventObserver.TryReplaceContent(requestMessage));
        HttpContent content2 = requestMessage.Content;

        Assert.IsFalse(object.ReferenceEquals(content1, content2));
    }


    [TestMethod]
    public void Test_TryReplaceContent2()
    {
        Assert.AreEqual(0, HttpClientEventObserver.TryReplaceContent((HttpRequestMessage)null));

        HttpOption httpOption = new HttpOption {
            Method = "GET",
            Url = "http://www.abc.com/aa/bb"
        };

        // 没有 body
        HttpRequestMessage requestMessage = HttpObjectUtils.CreateRequestMessage(httpOption);
        Assert.AreEqual(2, HttpClientEventObserver.TryReplaceContent(requestMessage));
    }

    [TestMethod]
    public void Test_TryReplaceContent3()
    {
        HttpOption httpOption = new HttpOption {
            Method = "POST",
            Url = "http://www.abc.com/aa/bb",
            Data = "xxxxxxxxxxxxxxxx".GetBytes(),
            Format = SerializeFormat.Binary,
            Header = new {
                Content_Length = 20
            }
        };

        // body 不是文本
        HttpRequestMessage requestMessage = HttpObjectUtils.CreateRequestMessage(httpOption);
        Assert.AreEqual(2, HttpClientEventObserver.TryReplaceContent(requestMessage));
    }

}
#endif