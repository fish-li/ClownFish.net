using ClownFish.UnitTest.Http.Mock;

namespace ClownFish.UnitTest.Log.Logging;

[TestClass]
public class OprLogTest
{
    [TestMethod]
    public void Test1()
    {
        OprLog log = new OprLog();

        Assert.AreEqual(0, log.Status);

        log.SetBaseInfo();
        Assert.AreEqual(200, log.Status);
        Assert.IsNotNull(log.AppName);
        Assert.IsNotNull(log.HostName);
        Assert.IsNotNull(log.EnvName);
        Assert.IsNotNull(log.OprId);


        Assert.AreEqual(0, log.SetException(null));

        Exception ex = ExceptionHelper.CreateException("xxxxxxxxxxxxxxx");
        Assert.AreEqual(1, log.SetException(ex));

        Assert.AreEqual(500, log.Status);
        Assert.AreEqual(1, log.HasError);
        Assert.IsNotNull(log.ExType);
        Assert.IsNotNull(log.ExMessage);
        Assert.IsNotNull(log.ExAll);

        log.SetHttpRequest(null);
        Assert.IsNull(log.Url);

        Assert.AreEqual(0, log.SetMCA(-1000));
        Assert.AreEqual(-2, log.SetMCA(1000));
        Assert.AreEqual(1, log.SetMCA());

        Assert.IsNotNull(log.Module);
        Assert.IsNotNull(log.Controller);
        Assert.IsNotNull(log.Action);

        log.Text1 = "text1111111111";
        log.Text2 = "text2222222222";
        log.Text3 = "text3333333333";
        log.Text4 = "text4444444444";
        log.Text5 = "text5555555555";
        Assert.AreEqual("text1111111111", log.Text1);
        Assert.AreEqual("text2222222222", log.Text2);
        Assert.AreEqual("text3333333333", log.Text3);
        Assert.AreEqual("text4444444444", log.Text4);
        Assert.AreEqual("text5555555555", log.Text5);

        log.Response = "response_111111";
        log.Logs = "logs_2222222";
        Assert.AreEqual("response_111111", log.Response);
        Assert.AreEqual("logs_2222222", log.Logs);

        log.AppKind = 999999;
        Assert.AreEqual(999999, log.AppKind);
        log.RetryCount = 888888;
        Assert.AreEqual(888888, log.RetryCount);


        log.CalcTime(0, DateTime.Now);
        Assert.AreEqual(0, log.IsSlow);
    }


    [TestMethod]
    public void Test2()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "DELETE",
            Url = new Uri("http://www.abc.com:14752/aaa/bb/ccc.aspx?id=3")
        };

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            Exception ex = ExceptionHelper.CreateException("xxxxxxxxxxxxxxx");
            OprLog log = OprLog.CreateErrLog(ex, mock.HttpContext);

            log.SetException(ex);
            Assert.AreEqual(500, log.Status);
            Assert.AreEqual(1, log.HasError);
            Assert.AreEqual(typeof(InvalidOperationException).FullName, log.ExType);
            Assert.AreEqual("xxxxxxxxxxxxxxx", log.ExMessage);
            Assert.IsNotNull(log.ExAll);

            Assert.AreEqual("error", log.OprKind);
            Assert.AreEqual("NULL", log.OprName);

            Assert.AreEqual("DELETE", log.HttpMethod);
            Assert.AreEqual("http://www.abc.com:14752/aaa/bb/ccc.aspx?id=3", log.Url);
        }
    }

    [TestMethod]
    public void Test3()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "DELETE",
            Url = new Uri("http://www.abc.com:14752/aaa/bb/ccc.aspx?id=3")
        };

        StepItem item = new StepItem();
        Assert.AreEqual(0, item.Status);
        Assert.IsNull(item.StepId);

        item = StepItem.CreateNew();
        Assert.AreEqual(200, item.Status);

        item = StepItem.CreateNew(DateTime.MaxValue, "91d8d8795c8f4d94a967b881bdaee803");
        Assert.AreEqual(DateTime.MaxValue, item.StartTime);
        Assert.AreEqual("91d8d8795c8f4d94a967b881bdaee803", item.StepId);

        Exception ex = ExceptionHelper.CreateException("xxxxxxxxxxxxxxx");
        item.SetException(ex);
        Assert.AreEqual(500, item.Status);
        Assert.AreEqual(1, item.HasError);
        Assert.IsNotNull(item.ExType);
        Assert.IsNotNull(item.ExMessage);

        //item.SetException(null);
        //Assert.AreEqual(200, item.Status);
        //Assert.AreEqual(0, item.HasError);
        //Assert.IsNull(item.ExType);
        //Assert.IsNull(item.ExMessage);

        item.Cmdx = requestData;
        string json = item.GetLogText2();

        Console.WriteLine(json);
        Assert.IsNotNull(item.Detail);

        StepItem item2 = json.FromJson<StepItem>();
        Assert.AreEqual(item.StepName, item2.StepName);

        MockRequestData requestData2 = item.Detail.FromJson<MockRequestData>();
        Assert.AreEqual(requestData.HttpMethod, requestData2.HttpMethod);
        Assert.AreEqual(requestData.Url, requestData2.Url);
    }

    [TestMethod]
    public async Task Test4()
    {
        string a20 = new string('a', 20);
        string b20 = new string('b', 20);
        string c20 = new string('c', 20);
        string c7 = new string('c', 7);

        MockRequestData requestData = new MockRequestData {
            HttpMethod = "DELETE",
            Url = new Uri($"http://www.abc.com:14752/{a20}/{b20}/{c20}.aspx?id=3")
        };

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {
            NHttpModuleFactory.RegisterModule<OprLogModule>();

            await mock.ProcessRequest();

            OprLog log = mock.PipelineContext.OprLogScope.OprLog;
            Assert.AreEqual("HttpRequest", log.OprName);
        }
    }

    [TestMethod]
    public void Test_CreateErrLog()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            OprLog.CreateErrLog(null, null);
        });
    }


    [TestMethod]
    public void Test_SetHttpRequest()
    {
        OprLog log = new OprLog();
        log.SetHttpRequest(EmptyHttpContext.Instance);

        Assert.IsNull(log.Url);

        log.Url = "http://www.abc.com/aa/bb.aspx";
        log.SetHttpRequest(EmptyHttpContext.Instance);

        Assert.AreEqual("http://www.abc.com/aa/bb.aspx", log.Url);
    }


    [TestMethod]
    public void Test_SetHttpData()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "GET",
            Url = new Uri($"http://www.abc.com:14752/aa/bb/cc.aspx?id=3")
        };

        using MockHttpPipeline mock = new MockHttpPipeline(requestData);
        mock.HttpContext.Response.StatusCode = 208;
        mock.HttpContext.Response.ContentLength = 128;

        Type thisType = this.GetType();
        MethodInfo method = thisType.GetMethod("Test_SetHttpData");

        mock.PipelineContext.SetAction(new ActionDescription(thisType, method));


        OprLog log = new OprLog();
        log.SetHttpData(mock.HttpContext);

        Assert.AreEqual(128, log.OutSize);
        Assert.AreEqual(208, log.Status);
        Assert.AreEqual("httpin", log.OprKind);

        Assert.AreEqual(thisType.Namespace, log.Module);
        Assert.AreEqual(thisType.Name, log.Controller);
        Assert.AreEqual(method.Name, log.Action);
        Assert.AreEqual($"{thisType.Name}/{method.Name}", log.OprName);
        //Assert.AreEqual("HttpRequest", log.OprName);
    }


    [TestMethod]
    public void Test_SetHttpData2()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "GET",
            Url = new Uri($"http://www.abc.com:14752/aa/bb/cc.aspx?id=3")
        };

        using MockHttpPipeline mock = new MockHttpPipeline(requestData);
        mock.HttpContext.Response.StatusCode = 208;
        mock.HttpContext.Response.ContentLength = 128;

        Type thisType = typeof(ClaaaXX1);
        MethodInfo method = thisType.GetMethod("Method1");

        mock.PipelineContext.SetAction(new ActionDescription(thisType, method));


        OprLog log = new OprLog();
        log.SetHttpData(mock.HttpContext);

        Assert.AreEqual(128, log.OutSize);
        Assert.AreEqual(208, log.Status);
        Assert.AreEqual("httpin", log.OprKind);

        Assert.AreEqual("测试模块A1", log.Module);
        Assert.AreEqual("测试类型B1", log.Controller);
        Assert.AreEqual("功能点C1", log.Action);
        Assert.AreEqual($"{thisType.Name}/{method.Name}", log.OprName);
        //Assert.AreEqual("HttpRequest", log.OprName);
    }


    [TestMethod]
    public void Test_SetHttpData3()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "GET",
            Url = new Uri($"http://www.abc.com:14752/aa/bb/cc.aspx?id=3")
        };

        using MockHttpPipeline mock = new MockHttpPipeline(requestData);
        
        OprLog log = new OprLog();
        log.SetHttpData(mock.HttpContext);

        Assert.AreEqual(0, log.OutSize);
        Assert.AreEqual(0, log.Status);
        Assert.AreEqual("httpin", log.OprKind);

        Assert.IsNull(log.Module);
        Assert.IsNull(log.Controller);
        Assert.IsNull(log.Action);
        Assert.AreEqual("HttpRequest", log.OprName);
    }


    [TestMethod]
    public void Test_TryGetBizInfo()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "GET",
            Url = new Uri($"http://www.abc.com:14752/aa/bb/cc.aspx?id=3")
        };
        using MockHttpPipeline mock = new MockHttpPipeline(requestData);

        mock.HttpContext.Items["TenantId"] = "TenantId_11111";
        mock.HttpContext.Items["UserId"] = "UserId_11111";
        mock.HttpContext.Items["UserCode"] = "UserCode_11111";
        mock.HttpContext.Items["UserName"] = "UserName_11111";
        mock.HttpContext.Items["UserRole"] = "UserRole_111111";
        mock.HttpContext.Items["Biz-Id"] = "BizId_111111";
        mock.HttpContext.Items["Biz-Name"] = "BizName_1111111";
        mock.HttpContext.Items["Biz-Module"] = "Module_11111";
        mock.HttpContext.Items["Biz-Controller"] = "Controller_11111";
        mock.HttpContext.Items["Biz-Action"] = "Action_1111111";
        mock.HttpContext.Items["Count1"] = "123";

        OprLog log = new OprLog();
        log.TryGetBizInfo(mock.HttpContext);

        Assert.AreEqual("TenantId_11111", log.TenantId);
        Assert.AreEqual("UserId_11111", log.UserId);
        Assert.AreEqual("UserCode_11111", log.UserCode);
        Assert.AreEqual("UserName_11111", log.UserName);
        Assert.AreEqual("UserRole_111111", log.UserRole);
        Assert.AreEqual("BizId_111111", log.BizId);
        Assert.AreEqual("BizName_1111111", log.BizName);
        Assert.AreEqual("Module_11111", log.Module);
        Assert.AreEqual("Controller_11111", log.Controller);
        Assert.AreEqual("Action_1111111", log.Action);
    }


    [TestMethod]
    public void Test_SetResponseData()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "GET",
            Url = new Uri($"http://www.abc.com:14752/aa/bb/cc.aspx?id=3")
        };
        using MockHttpPipeline mock = new MockHttpPipeline(requestData);
        mock.PipelineContext.ActionResult = "xxxxxxxxxxxxxxxxx";

        mock.HttpContext.Response.StatusCode = 200;
        mock.HttpContext.Response.ContentType = ResponseContentType.TextUtf8;



        OprLog log = new OprLog();
        log.SetResponseData(mock.HttpContext);

        Console.WriteLine(log.Response);

        Assert.IsTrue(log.Response.Contains("HTTP/1.1 200 OK"));
        Assert.IsTrue(log.Response.Contains("xxxxxxxxxxxxxxxxx"));
    }


    [TestMethod]
    public void Test_SetResponseData2()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "GET",
            Url = new Uri($"http://www.abc.com:14752/aa/bb/cc.aspx?id=3")
        };
        using MockHttpPipeline mock = new MockHttpPipeline(requestData);
        mock.PipelineContext.ActionResult = new {
            Id = 2,
            Count = 3
        };

        mock.HttpContext.Response.StatusCode = 200;
        mock.HttpContext.Response.ContentType = ResponseContentType.JsonUtf8;



        OprLog log = new OprLog();
        log.SetResponseData(mock.HttpContext);

        Console.WriteLine(log.Response);

        Assert.IsTrue(log.Response.Contains("HTTP/1.1 200 OK"));
        Assert.IsTrue(log.Response.Contains("{\"Id\":2,\"Count\":3}"));
    }

    [TestMethod]
    public void Test_SetResponseData3()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "GET",
            Url = new Uri($"http://www.abc.com:14752/aa/bb/cc.aspx?id=3")
        };
        using MockHttpPipeline mock = new MockHttpPipeline(requestData);
        mock.PipelineContext.ActionResult = new {
            Id = 2,
            Count = 3
        };

        mock.HttpContext.Response.StatusCode = 200;
        mock.HttpContext.Response.ContentType = ResponseContentType.XmlUtf8;



        OprLog log = new OprLog();
        log.SetResponseData(mock.HttpContext);

        Console.WriteLine(log.Response);

        Assert.IsTrue(log.Response.Contains("HTTP/1.1 200 OK"));
        Assert.IsTrue(log.Response.Contains("###ClownFish.net在记录Response时出现异常：System.InvalidOperationException: <>f__AnonymousType"));
    }

    [TestMethod]
    public void Test_SetResponseData4()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "GET",
            Url = new Uri($"http://www.abc.com:14752/aa/bb/cc.aspx?id=3")
        };
        using MockHttpPipeline mock = new MockHttpPipeline(requestData);
        mock.PipelineContext.ActionResult = new NameValue("abc", "123");

        mock.HttpContext.Response.StatusCode = 200;
        mock.HttpContext.Response.ContentType = ResponseContentType.XmlUtf8;



        OprLog log = new OprLog();
        log.SetResponseData(mock.HttpContext);

        Console.WriteLine(log.Response);

        Assert.IsTrue(log.Response.Contains("HTTP/1.1 200 OK"));
        Assert.IsTrue(log.Response.Contains("<NameValue Name=\"abc\">"));
        Assert.IsTrue(log.Response.Contains("<Value>123</Value>"));
    }


    [TestMethod]
    public void Test_TruncateTextnField()
    {
        string text = new string('a', 102400);

        OprLog log = new OprLog();
        log.CtxData = text;
        log.Addition = text;
        log.Request = text;
        log.Response = text;
        log.Text1 = text;
        log.Text2 = text;
        log.Text3 = text;
        log.Text4 = text;
        log.Text5 = text;

        log.TruncateTextFields();

        Assert.IsTrue(log.CtxData.Length < 102400);
        Assert.IsTrue(log.Addition.Length < 102400);
        Assert.IsTrue(log.Request.Length < 102400);
        Assert.IsTrue(log.Response.Length < 102400);
        Assert.IsTrue(log.Text1.Length < 102400);
        Assert.IsTrue(log.Text2.Length < 102400);
        Assert.IsTrue(log.Text3.Length < 102400);
        Assert.IsTrue(log.Text4.Length < 102400);
        Assert.IsTrue(log.Text5.Length < 102400);
    }

    [TestMethod]
    public void Test_GetActionType()
    {
        Assert.AreEqual(100, OprLog.GetActionType(OprKinds.HttpIn));
        Assert.AreEqual(100, OprLog.GetActionType(OprKinds.HttpProxy));

        Assert.AreEqual(200, OprLog.GetActionType(OprKinds.Msg));
        Assert.AreEqual(300, OprLog.GetActionType(OprKinds.BTask));

        Assert.AreEqual(400, OprLog.GetActionType("code"));
        Assert.AreEqual(400, OprLog.GetActionType("test"));
    }
}



public class EmptyHttpContext : NHttpContext
{
    public static readonly EmptyHttpContext Instance = new EmptyHttpContext();

    public override object OriginalHttpContext => throw new NotImplementedException();

    public override NHttpRequest Request => throw new NotImplementedException();

    public override NHttpResponse Response => throw new NotImplementedException();

    public override IPrincipal User { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override bool SkipAuthorization { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override XDictionary Items => throw new NotImplementedException();
}


[ControllerTitle(Module = "测试模块A1", Name = "测试类型B1")]
public class ClaaaXX1
{
    [ActionTitle(Name = "功能点C1")]
    public void Method1()
    {

    }
}
