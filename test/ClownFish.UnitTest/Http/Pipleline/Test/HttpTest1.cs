using ClownFish.UnitTest.Http.Mock;

namespace ClownFish.UnitTest.Http.Pipleline.Test;

[TestClass]
public class HttpTest1
{
    public static MockRequestData GetRequestData()
    {
        return new MockRequestData {
            HttpMethod = "POST",
            Url = new Uri("http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3"),
            Headers = HttpHeaderCollection.Create(new {
                Content_Type = "application/json",
                x_client_app = "HttpTest1",
                Cookie = "c1=1111111; c2=22222222"
            }),
            Body = new {
                Query = "12345",
                Parameters = new {
                    sid = "248654324",
                    start = new DateTime(2021, 7, 1),
                    end = new DateTime(2021, 7, 2)
                }
            }.ToJson().GetBytes()
        };
    }

    private void AssertTestHandler1(TestHandler1 handler1)
    {
        Assert.AreEqual("POST", handler1.RequestValues["HttpMethod"]);
        Assert.AreEqual("http://www.abc.com:14752", handler1.RequestValues["RootUrl"]);
        Assert.AreEqual("/aaa/bb/ccc.aspx", handler1.RequestValues["Path"]);
        Assert.AreEqual("?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3", handler1.RequestValues["Query"]);
        Assert.AreEqual("/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3", handler1.RequestValues["RawUrl"]);
        Assert.AreEqual("http://www.abc.com:14752/aaa/bb/ccc.aspx", handler1.RequestValues["FullPath"]);
        Assert.AreEqual("http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3", handler1.RequestValues["FullUrl"]);
        Assert.AreEqual("application/json", handler1.RequestValues["ContentType"]);
        Assert.IsNull(handler1.RequestValues["UserAgent"]);
        Assert.AreEqual(false.ToString(), handler1.RequestValues["IsHttps"]);
        Assert.AreEqual(false.ToString(), handler1.RequestValues["IsAuthenticated"]);
        Assert.AreEqual(true.ToString(), handler1.RequestValues["HasBody"]);
        Assert.AreEqual(true.ToString(), handler1.RequestValues["LogRequestBody"]);

        Assert.AreEqual(3, handler1.Headers.Count);
        Assert.IsNotNull(handler1.Headers.FindByName("Content-Type"));
        Assert.IsNotNull(handler1.Headers.FindByName("x-client-app"));
        Assert.IsNotNull(handler1.Headers.FindByName("Cookie"));
        Assert.AreEqual("application/json", handler1.Headers.FindByName("Content-Type").Value);
        Assert.AreEqual("HttpTest1", handler1.Headers.FindByName("x-client-app").Value);

        Assert.AreEqual(2, handler1.Cookies.Count);
        Assert.IsNotNull(handler1.Cookies.FindByName("c1"));
        Assert.IsNotNull(handler1.Cookies.FindByName("c2"));
        Assert.AreEqual("1111111", handler1.Cookies.FindByName("c1").Value);
        Assert.AreEqual("22222222", handler1.Cookies.FindByName("c2").Value);

        Assert.AreEqual(2, handler1.QuerySting.Count);
        Assert.IsNotNull(handler1.QuerySting.FindByName("tenantId"));
        Assert.IsNotNull(handler1.QuerySting.FindByName("checkType"));
        Assert.AreEqual("my57972739adc90", handler1.QuerySting.FindByName("tenantId").Value);
        Assert.AreEqual("系统应用水平", handler1.QuerySting.FindByName("checkType").Value);

        Console.WriteLine(handler1.RequestBodyText);
    }


    [TestMethod]
    public void Test_RequestData()
    {
        MockRequestData data1 = GetRequestData();

        string text = data1.ToText();
        Console.WriteLine(text);

        MockRequestData data2 = MockRequestData.FromText(text);
        string text2 = data2.ToText();

        Assert.AreEqual(text, text2);
    }

    [TestMethod]
    public async Task Test_Normal()
    {
        MockRequestData requestData = GetRequestData();

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<TestModule1>();

            await mock.ProcessRequest();

            Assert.IsNull(mock.LastException);

            // 直接获取一些属性，并强制转换，代替断言
            TestHandler1 handler1 = (TestHandler1)mock.PipelineContext.Action.Controller;
            AssertTestHandler1(handler1);

            //----------------------------------
            string text = requestData.ToText();
            Assert.AreEqual(text, handler1.LoggingText);

            string json = Encoding.UTF8.GetString(requestData.Body);
            Assert.AreEqual(json, handler1.RequestBodyText);
            Assert.AreEqual(json, Encoding.UTF8.GetString(handler1.RequestBodyBytes));

            Assert.AreEqual("OK/1533", mock.PipelineContext.ActionResult.ToString());
        }
    }



    [TestMethod]
    public async Task Test_BeginRequest_Set_Handler()
    {
        MockRequestData requestData = GetRequestData();

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<TestModule2>();

            await mock.ProcessRequest();

            Assert.IsNull(mock.LastException);

            // 直接获取一些属性，并强制转换，代替断言
            TestHandler1 handler1 = (TestHandler1)mock.PipelineContext.Action.Controller;
            AssertTestHandler1(handler1);
        }
    }


    [TestMethod]
    public async Task Test_HttpPipelineContext()
    {
        MockRequestData requestData = GetRequestData();

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<TestModule2>();

            await mock.ProcessRequest();

            Assert.IsNull(mock.LastException);

            //----------------------------------
            Assert.AreEqual(503, mock.PipelineContext.GetStatus());
            Assert.AreEqual("http://www.abc.com:14752/aaa/bb/ccc.aspx", mock.PipelineContext.GetTitle());
            Assert.AreEqual(mock.HttpContext.Request, mock.PipelineContext.GetRequest());

            Assert.AreEqual(mock.PipelineContext, HttpPipelineContext.Get());
            Assert.AreEqual(true, mock.PipelineContext.IsLoginAction);

            Assert.IsFalse(mock.HttpContext.IsAuthenticated);
            Assert.AreEqual("Request Url: /aaa/bb/ccc.aspx", mock.HttpContext.ToString());
            Assert.IsNull(mock.HttpContext.LastException);

            Assert.IsTrue((mock.HttpContext.EndExecuteTime - mock.HttpContext.BeginExecuteTime).TotalSeconds < 2);
        }
    }


    [TestMethod]
    public async Task Test_ResolveRequestCache_Set_Handler()
    {
        MockRequestData requestData = GetRequestData();

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<TestModule3>();

            await mock.ProcessRequest();

            Assert.IsNull(mock.LastException);

            // 直接获取一些属性，并强制转换，代替断言
            TestHandler1 handler1 = (TestHandler1)mock.PipelineContext.Action.Controller;
            AssertTestHandler1(handler1);

        }
    }


    [TestMethod]
    public async Task Test_HttpContextItemsDispose()
    {
        long count = XDisposableObject.InstanceCounter.Get();

        MockRequestData requestData = GetRequestData();

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<TestModule3>();

            await mock.ProcessRequest();

            Assert.IsNull(mock.LastException);

            //----------------------------------
            Assert.AreEqual(count + 1, XDisposableObject.InstanceCounter.Get());
        }

        Assert.AreEqual(count, XDisposableObject.InstanceCounter.Get());
    }


    [TestMethod]
    public void Test_RegisterForDispose()
    {
        long count = XDisposableObject.InstanceCounter.Get();
        MockRequestData requestData = GetRequestData();

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<TestModule1>();
            mock.Init();

            mock.HttpContext.RegisterForDispose(null);
            mock.HttpContext.RegisterForDispose(new XDisposableObject());
            mock.HttpContext.RegisterForDispose(new XDisposableObject());
            mock.HttpContext.RegisterForDispose(new MemoryStream());

            DebugReportBlock block = mock.Application.GetDebugReportBlock();
            string[] lines = block.ToString2().ToLines();

            //Console.WriteLine(text);
            Assert.IsTrue(lines.Count(x=>x.StartsWith0( "1: ClownFish.UnitTest.Http.Pipleline.Test.TestModule1")) == 1);

            //----------------------------------
            Assert.AreEqual(count + 2, XDisposableObject.InstanceCounter.Get());
        }

        Assert.AreEqual(count, XDisposableObject.InstanceCounter.Get());
    }

    [TestMethod]
    public async Task Test_Pipeline_Error()
    {
        MockRequestData requestData = GetRequestData();
        requestData.Headers.Add("x-ThrowException", "Test5");

        long count1 = TestModule4.ErrorCounter.Get();
        long count2 = TestModule4.EndCounter.Get();

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<TestModule4>();

            await mock.ProcessRequest();

            MyAssert.IsError<InvalidOperationException>(()=> {
                NHttpModuleFactory.RegisterModule<TestModule1>();
            });

            Assert.IsNotNull(mock.LastException);
            Assert.IsInstanceOfType(mock.LastException, typeof(ApplicationException));
            Assert.AreEqual("Test5", mock.LastException.Message);

            //----------------------------------

            Assert.AreEqual(count1 + 1, TestModule4.ErrorCounter.Get());
            Assert.AreEqual(count2 + 1, TestModule4.EndCounter.Get());
        }
    }



    [TestMethod]
    public async Task Test_Origin_Cors()
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
x-client-app: HttpTest1
Cookie: c1=1111111; c2=22222222
Origin: https://www.abc.com

xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<TestModule5>();

            await mock.ProcessRequest();

            MockHttpResponse response = (MockHttpResponse)mock.HttpContext.Response;
            //----------------------------------
            Assert.AreEqual("https://www.abc.com", response.OutHeaders["Access-Control-Allow-Origin"]);
            Assert.AreEqual("true", response.OutHeaders["Access-Control-Allow-Credentials"]);

            Assert.IsNull(response.OutHeaders["Access-Control-Allow-Methods"]);
            Assert.IsNull(response.OutHeaders["Access-Control-Allow-Headers"]);
        }
    }


    [TestMethod]
    public async Task Test_OPTIONS()
    {
        string requestText = @"
OPTIONS http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
x-client-app: HttpTest1
Cookie: c1=1111111; c2=22222222
Origin: https://www.abc.com
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<TestModule5>();

            await mock.ProcessRequest();

            MockHttpResponse response = (MockHttpResponse)mock.HttpContext.Response;
            //----------------------------------
            Assert.AreEqual("https://www.abc.com", response.OutHeaders["Access-Control-Allow-Origin"]);
            Assert.AreEqual("true", response.OutHeaders["Access-Control-Allow-Credentials"]);

            Assert.AreEqual("*", response.OutHeaders["Access-Control-Allow-Methods"]);
            Assert.AreEqual("*", response.OutHeaders["Access-Control-Allow-Headers"]);
        }
    }



    [TestMethod]
    public async Task Test_ResponseEnd()
    {
        MockRequestData requestData = GetRequestData();

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<TestModule6>();

            await mock.ProcessRequest();

            Assert.IsNotNull(mock.LastException);
            Assert.IsInstanceOfType(mock.LastException, typeof(AbortRequestException));

            // 直接获取一些属性，并强制转换，代替断言
            TestHandler1 handler1 = (TestHandler1)mock.PipelineContext.Action.Controller;

            // 确认 handler 没有执行过
            Assert.IsNull(handler1.RequestBodyText);

        }
    }



   


   


}
