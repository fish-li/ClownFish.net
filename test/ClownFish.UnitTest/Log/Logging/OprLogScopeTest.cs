namespace ClownFish.UnitTest.Log.Logging;

[TestClass]
public class OprLogScopeTest
{
    [TestMethod]
    public async Task Test_HttpLog()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "GET",
            Url = new Uri("http://www.abc.com:14752/aaa/bb/ccc.aspx?id=3")
        };

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {
            NHttpModuleFactory.RegisterModule<OprLogModule>();

            TestHandler1 handler1 = new TestHandler1();
            mock.PipelineContext.SetHttpHandler(handler1);
            mock.PipelineContext.PerformanceThresholdMs = 1;

            await mock.ProcessRequest();




            OprLogScope oprLogScope = handler1.OprLogScope;
            Assert.IsNotNull(oprLogScope);
            RetryFile.WriteAllText("./temp/OprLogScopeTest_test1.json", oprLogScope.OprLog.ToJson(JsonStyle.Indented));

            OprLog log = oprLogScope.OprLog;
            Assert.AreEqual("http", log.OprKind);
            Assert.AreEqual("http", log.OprName);
            Assert.AreEqual(1, log.HasError);
            Assert.AreEqual(1, log.IsSlow);
            Assert.AreEqual("ClownFish.Data.DbExceuteException", log.ExType);
            Assert.AreEqual("ClownFish.UnitTest", log.AppName);
            Assert.AreEqual("ClownFish_TEST", log.EnvName);

            Assert.AreEqual("x_TenantId", log.TenantId);
            Assert.AreEqual("x_UserId", log.UserId);
            Assert.AreEqual("x_UserName", log.UserName);
            Assert.AreEqual("x_UserRole", log.UserRole);
            Assert.AreEqual("x_BizId", log.BizId);
            Assert.AreEqual("x_BizName", log.BizName);
            Assert.AreEqual("x_ActionModule", log.Module);
            Assert.AreEqual("x_ActionController", log.Controller);
            Assert.AreEqual("x_ActionMethod", log.Action);
            Assert.AreEqual("x_Addition", log.Addition);

            Assert.AreEqual(log.OprId, (log as IMsgObject).GetId());
            Assert.AreEqual(log.StartTime, (log as IMsgObject).GetTime());

            List<StepItem> list = (List<StepItem>)oprLogScope.GetFieldValue("_steps");
            RetryFile.WriteAllText("./temp/OprLogScopeTest_test1_list.json", list.ToJson(JsonStyle.Indented));
            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list.Count(x => x.StepKind == "sqlconn") > 0);
            Assert.IsTrue(list.Count(x => x.StepKind == "sqlcmd") > 0);
            Assert.IsTrue(list.Count(x => x.StepKind == "sqltrans") > 0);
            Assert.IsTrue(list.Count(x => x.StepKind == "httprpc") > 0);

            Assert.IsTrue(list.Count(x => x.IsAsync == 1) > 0);
            Assert.IsTrue(list.Count(x => x.Status == 500) > 0);

        }
    }


    [TestMethod]
    public void Test_Start()
    {
        using OprLogScope s1 = OprLogScope.Start();

        // OprLogScope不允许嵌套使用
        MyAssert.IsError<InvalidOperationException>(() => {
            OprLogScope s2 = OprLogScope.Start();
        });
    }


    [TestMethod]
    public void Test_AddStep()
    {
        using OprLogScope s1 = OprLogScope.Start();

        Assert.AreEqual(-1, s1.AddStep(null));

        Assert.AreEqual(1, s1.AddStep(new StepItem()));
        Assert.AreEqual(1, s1.AddStep(new StepItem()));
        Assert.AreEqual(2, s1.GetStepItems().Count);

        s1.SetFieldValue("_isEnd", true);
        Assert.AreEqual(-2, s1.AddStep(new StepItem()));

        Assert.AreEqual(2, s1.GetStepItems().Count);
    }


    [TestMethod]
    public void Test_SetException()
    {
        using OprLogScope s1 = OprLogScope.Start();

        Assert.AreEqual(-1, s1.SetException(null));

        var ex1 = ExceptionHelper.CreateException();
        var ex2 = ExceptionHelper.CreateException();

        Assert.AreEqual(1, s1.SetException(ex1));
        Assert.AreEqual(1, s1.SetException(ex2));

        s1.SetFieldValue("_isEnd", true);
        Assert.AreEqual(-2, s1.SetException(ex2));
    }


    [TestMethod]
    public void Test_SaveOprLog()
    {
        using OprLogScope s1 = OprLogScope.Start();

        string detail = (string)s1.InvokeMethod("GetOprDetails");
        Assert.AreEqual(string.Empty, detail);

        MyAssert.IsError<ArgumentNullException>(() => {
            s1.SaveOprLog(null);
        });
    }


    [TestMethod]
    public void Test_AddStep2()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "GET",
            Url = new Uri($"http://www.abc.com:14752/aa/bb/cc.aspx?id=3")
        };
        using MockHttpPipeline mock = new MockHttpPipeline(requestData);

        OprLogScope scope = mock.PipelineContext.OprLogScope;

        for(int i= 0; i < LoggingLimit.OprLog.StepsMaxCount + 10; i++) {
            scope.AddStep(DateTime.Now, $"s{i}", new string('a', 10240), DateTime.Now, ExceptionHelper.CreateException());
        }


        Assert.AreEqual(-1, scope.AddStep((StepItem)null));
        Assert.AreEqual(0, scope.AddStep(DateTime.Now, "", "xx"));
        Assert.AreEqual(2, scope.AddStep(DateTime.Now, "x1", "xx"));

        Thread.Sleep(1);
        mock.PipelineContext.PerformanceThresholdMs = 1;
        mock.PipelineContext.End();
        scope.EndSet(mock.PipelineContext);

        int ret = scope.SaveOprLog(mock.PipelineContext);
        Assert.AreEqual(0, ret);

        Assert.AreEqual(1, scope.OprLog.IsSlow);
        Assert.AreEqual(0, scope.OprLog.HasError);
        Assert.AreEqual(200, scope.OprLog.Status);
        Assert.AreEqual(LoggingLimit.OprLog.StepsMaxCount, scope.GetStepItems().Count);

        Assert.AreEqual(-2, scope.AddStep(DateTime.Now, "x2", "xx"));
    }


    [TestMethod]
    public void Test_CheckError500()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "GET",
            Url = new Uri($"http://www.abc.com:14752/aa/bb/cc.aspx?id=3")
        };
        using MockHttpPipeline mock = new MockHttpPipeline(requestData);

        OprLogScope scope = mock.PipelineContext.OprLogScope;

        for( int i = 0; i <  10; i++ ) {
            scope.AddStep(DateTime.Now, $"s{i}", new string('a', 10240), DateTime.Now, ExceptionHelper.CreateException());
        }

        mock.PipelineContext.End();
        scope.EndSet(mock.PipelineContext);

        Assert.AreEqual(0, scope.OprLog.HasError);
        Assert.AreEqual(200, scope.OprLog.Status);


        scope.OprLog.Status = 500;
        scope.CheckError500();
        Assert.AreEqual(1, scope.OprLog.HasError);
        Assert.AreEqual(500, scope.OprLog.Status);
    }



    [TestMethod]
    public void Test_AddLog()
    {
        MockRequestData requestData = new MockRequestData {
            HttpMethod = "GET",
            Url = new Uri($"http://www.abc.com:14752/aa/bb/cc.aspx?id=3")
        };
        using MockHttpPipeline mock = new MockHttpPipeline(requestData);

        OprLogScope scope = mock.PipelineContext.OprLogScope;

        for( int i = 0; i < LoggingLimit.OprLog.LogsMaxCount + 10; i++ ) {
            scope.Log(Guid.NewGuid().ToString());
        }

        Assert.AreEqual(0, scope.Log(""));
        Assert.AreEqual(2, scope.Log("111111111111"));

        Thread.Sleep(1);
        mock.PipelineContext.PerformanceThresholdMs = 1;
        mock.PipelineContext.End();
        scope.EndSet(mock.PipelineContext);

        int ret = scope.SaveOprLog(mock.PipelineContext);
        Assert.AreEqual(0, ret);

        Assert.AreEqual(1, scope.OprLog.IsSlow);
        Assert.AreEqual(200, scope.OprLog.Status);
        Assert.AreEqual(LoggingLimit.OprLog.LogsMaxCount, scope.GetLogs().Count);

        Assert.AreEqual(-2, scope.Log("xxxxxxxxxxxxxx"));
    }



    [TestMethod]
    public void Test_Suspend1()
    {
        OprLog oprLog = null;

        using( CodeSnippetContext ctx = new CodeSnippetContext(typeof(OprLogScopeTest), "Test_Suspend1", 1) ) {
            oprLog = ctx.OprLog;
            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            Thread.Sleep(10);

            // 访问数据库，会记录2个操作步骤：OpenConnection, ExecuteScalar
            using( DbContext dbContext = DbContext.Create("mysql") ) {
                int x1 = dbContext.CPQuery.Create("select 1+1 as a").ExecuteScalar<int>();
            }

            // 下面的调用，不在乎是否能调用成功，只要发生调用即可
            SendHttpRpc();

            ShowSteps(ctx.OprLogScope);
            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            // 由于 HttpClientLogger，HttpClientLogger2 同时在工作，所以发起HTTP调用时，会产生2个 StepItem，所以下面会做去重的操作
            string[] oprNames = ctx.OprLogScope.GetStepItems().Select(x => x.StepName).Distinct().ToArray();
            Assert.AreEqual(3, oprNames.Length);
            Assert.IsTrue(oprNames.Contains("OpenConnection"));
            Assert.IsTrue(oprNames.Contains("ExecuteScalar"));
            Assert.IsTrue(oprNames.Contains("SendHttp"));
        }

        Assert.IsNotNull(oprLog.OprDetails);
        Assert.IsTrue(oprLog.OprDetails.Length > 0);
        //Console.WriteLine(oprLog.OprDetails);

#if NETCOREAPP
        string details = BrotliHelper.Decompress(oprLog.OprDetails);
        Assert.IsTrue(details.Contains("[StepName]: OpenConnection"));
        Assert.IsTrue(details.Contains("[StepName]: ExecuteScalar"));
        Assert.IsTrue(details.Contains("[StepName]: SendHttp"));
        Assert.IsTrue(details.Contains("http://www.xxxxxxxxxx.com/test1.aspx"));
#endif

    }

    private void SendHttpRpc()
    {
        HttpOption httpOption = new HttpOption {
            Url = "http://www.xxxxxxxxxx.com/test1.aspx",
            Timeout = 100
        };

        try {
            _ = httpOption.GetResult();
        }
        catch { }
    }

    private void ShowSteps(OprLogScope scope)
    {
#if NETCOREAPP
        var list = scope.GetStepItems();
        foreach( var item in list ) {
            Console.WriteLine("Step: " + item.ToJson());
            if( item.Cmdx != null ) {
                Console.WriteLine("   " + item.Cmdx.GetType().FullName);
                if( item.Cmdx is HttpClientEventData httpdata ) {
                    Console.WriteLine("   " + httpdata.Request.RequestUri.ToString());
                }
                if( item.Cmdx is RequestFinishedEventArgs requestArgs ) {
                    Console.WriteLine("   " + requestArgs.Request.RequestUri.ToString());
                }
            }
        }
#endif
    }

    [TestMethod]
    public void Test_Suspend2()
    {
        OprLog oprLog = null;

        using( CodeSnippetContext ctx = new CodeSnippetContext(typeof(OprLogScopeTest), "Test_Suspend1", 1) ) {
            oprLog = ctx.OprLog;
            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            Thread.Sleep(10);

            // 访问数据库，会记录2个操作步骤：OpenConnection, ExecuteScalar
            using( DbContext dbContext = DbContext.Create("mysql") ) {
                int x1 = dbContext.CPQuery.Create("select 1+1 as a").ExecuteScalar<int>();
            }

            ctx.OprLogScope.Suspend();         // 注意这个调用
            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            // 下面的调用，不在乎是否能调用成功，只要发生调用即可
            SendHttpRpc();
            ctx.OprLogScope.Restore();
            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            string[] oprNames = ctx.OprLogScope.GetStepItems().Select(x => x.StepName).ToArray();
            Assert.AreEqual(2, oprNames.Length);                       // 这里变成 2
            Assert.IsTrue(oprNames.Contains("OpenConnection"));
            Assert.IsTrue(oprNames.Contains("ExecuteScalar"));
        }

        Assert.IsNotNull(oprLog.OprDetails);
        Assert.IsTrue(oprLog.OprDetails.Length > 0);

#if NETCOREAPP
        string details = BrotliHelper.Decompress(oprLog.OprDetails);
        Assert.IsTrue(details.Contains("[StepName]: OpenConnection"));
        Assert.IsTrue(details.Contains("[StepName]: ExecuteScalar"));
        Assert.IsFalse(details.Contains("[StepName]: SendHttp"));
        Assert.IsFalse(details.Contains("http://www.xxxxxxxxxx.com/test1.aspx"));
#endif
    }

    [TestMethod]
    public void Test_Suspend3()
    {
        OprLog oprLog = null;

        using( CodeSnippetContext ctx = new CodeSnippetContext(typeof(OprLogScopeTest), "Test_Suspend1", 1) ) {
            oprLog = ctx.OprLog;
            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            Thread.Sleep(10);

            ctx.OprLogScope.Suspend();         // 注意这个调用
            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            // 访问数据库，会记录2个操作步骤：OpenConnection, ExecuteScalar
            using( DbContext dbContext = DbContext.Create("mysql") ) {
                int x1 = dbContext.CPQuery.Create("select 1+1 as a").ExecuteScalar<int>();
            }

            ctx.OprLogScope.Restore();
            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            // 下面的调用，不在乎是否能调用成功，只要发生调用即可
            SendHttpRpc();

            // 由于 HttpClientLogger，HttpClientLogger2 同时在工作，所以发起HTTP调用时，会产生2个 StepItem，所以下面会做去重的操作
            string[] oprNames = ctx.OprLogScope.GetStepItems().Select(x => x.StepName).Distinct().ToArray();
            Assert.AreEqual(1, oprNames.Length);
            Assert.IsTrue(oprNames.Contains("SendHttp"));
        }

        Assert.IsNotNull(oprLog.OprDetails);
        Assert.IsTrue(oprLog.OprDetails.Length > 0);

#if NETCOREAPP
        string details = BrotliHelper.Decompress(oprLog.OprDetails);
        Assert.IsFalse(details.Contains("[StepName]: OpenConnection"));
        Assert.IsFalse(details.Contains("[StepName]: ExecuteScalar"));
        Assert.IsTrue(details.Contains("[StepName]: SendHttp"));
        Assert.IsTrue(details.Contains("http://www.xxxxxxxxxx.com/test1.aspx"));
#endif
    }


    [TestMethod]
    public void Test_Suspend4()
    {
        OprLog oprLog = null;

        using( CodeSnippetContext ctx = new CodeSnippetContext(typeof(OprLogScopeTest), "Test_Suspend1", 1) ) {
            oprLog = ctx.OprLog;
            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            Thread.Sleep(10);

            ctx.OprLogScope.Suspend();         // 注意这个调用
            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            // 访问数据库，会记录2个操作步骤：OpenConnection, ExecuteScalar
            using( DbContext dbContext = DbContext.Create("mysql") ) {
                int x1 = dbContext.CPQuery.Create("select 1+1 as a").ExecuteScalar<int>();
            }            

            // 下面的调用，不在乎是否能调用成功，只要发生调用即可
            SendHttpRpc();

            ctx.OprLogScope.Restore();
            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            Assert.IsNull(ctx.OprLogScope.GetStepItems());   // #######
        }

        Assert.AreEqual("", oprLog.OprDetails);
    }

    [TestMethod]
    public void Test_Suspend5()
    {
        OprLog oprLog = null;

        using( CodeSnippetContext ctx = new CodeSnippetContext(typeof(OprLogScopeTest), "Test_Suspend1", 1) ) {
            oprLog = ctx.OprLog;

            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            Thread.Sleep(10);            

            // 访问数据库，会记录2个操作步骤：OpenConnection, ExecuteScalar
            using( DbContext dbContext = DbContext.Create("mysql") ) {
                int x1 = dbContext.CPQuery.Create("select 1+1 as a").ExecuteScalar<int>();
            }

            // 下面的调用，不在乎是否能调用成功，只要发生调用即可
            SendHttpRpc();

            ctx.OprLogScope.Suspend();         // 注意这个调用，但是后面没有调用 Restore

            Assert.AreEqual(ctx.OprLogScope.CanLog, !ctx.OprLogScope.IsNull);

            int x2 = ctx.OprLogScope.AddStep(DateTime.Now, "abc");
            Assert.AreEqual(-2, x2);

            int x3 = ctx.OprLogScope.AddFxEvent(new NameTime("x3", DateTime.Now));
            Assert.AreEqual(-2, x3);

            int x4 = ctx.OprLogScope.Log("xxxxxxxxxxxxxx");
            Assert.AreEqual(-2, x4);

            int x5 = ctx.OprLogScope.AddStep(new StepItem { StepName = "xxxxx" });
            Assert.AreEqual(-2, x5);

            // 由于 HttpClientLogger，HttpClientLogger2 同时在工作，所以发起HTTP调用时，会产生2个 StepItem，所以下面会做去重的操作
            string[] oprNames = ctx.OprLogScope.GetStepItems().Select(x => x.StepName).Distinct().ToArray();
            Assert.AreEqual(3, oprNames.Length);
            Assert.IsTrue(oprNames.Contains("OpenConnection"));
            Assert.IsTrue(oprNames.Contains("ExecuteScalar"));
            Assert.IsTrue(oprNames.Contains("SendHttp"));
        }

        Assert.IsNotNull(oprLog.OprDetails);
        Assert.IsTrue(oprLog.OprDetails.Length > 0);

#if NETCOREAPP
        string details = BrotliHelper.Decompress(oprLog.OprDetails);
        Assert.IsTrue(details.Contains("[StepName]: OpenConnection"));
        Assert.IsTrue(details.Contains("[StepName]: ExecuteScalar"));
        Assert.IsTrue(details.Contains("[StepName]: SendHttp"));
        Assert.IsTrue(details.Contains("http://www.xxxxxxxxxx.com/test1.aspx"));
#endif
    }
}
