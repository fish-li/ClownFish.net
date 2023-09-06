using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Http.Pipleline;
using ClownFish.Log;
using ClownFish.Log.Logging;
using ClownFish.Log.Models;
using ClownFish.UnitTest.Http.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Log.Logging
{
    [TestClass]
    public class OprLogScopeTest
    {
        [TestMethod]
        public async Task Test1()
        {
            MockRequestData requestData = new MockRequestData {
                HttpMethod = "GET",
                Url = new Uri("http://www.abc.com:14752/aaa/bb/ccc.aspx?id=3")
            };

            using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {
                mock.HttpContext.TimeEvents = new List<NameTime>(30);
                NHttpModuleFactory.RegisterModule<OprLogModule>();

                TestHandler1 handler1 = new TestHandler1();
                mock.PipelineContext.SetHttpHandler(handler1);
                mock.PipelineContext.PerformanceThresholdMs = 1;

                await mock.ProcessRequest();




                OprLogScope oprLogScope = handler1.OprLogScope;
                Assert.IsNotNull(oprLogScope);
                RetryFile.WriteAllText("./temp/OprLogScopeTest_test1.json", oprLogScope.OprLog.ToJson(JsonStyle.Indented));

                OprLog log = oprLogScope.OprLog;
                Assert.AreEqual("httpin", log.OprKind);
                Assert.AreEqual("HttpRequest", log.OprName);
                Assert.AreEqual(1, log.HasError);
                Assert.AreEqual(1, log.IsSlow);
                Assert.AreEqual("ClownFish.Data.DbExceuteException", log.ExType);
                Assert.AreEqual("ClownFish.UnitTest", log.AppName);
                Assert.AreEqual("ClownFish.TEST", log.EnvName);

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
                Assert.IsTrue(list.Count(x => x.StepKind == "sqlcommand") > 0);
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

            using OprLogScope scope = OprLogScope.Start();

            for(int i= 0; i < LoggingLimit.OprLog.StepsMaxCount + 10; i++) {
                scope.AddStep(DateTime.Now, $"s{i}", new string('a', 10240), DateTime.Now, ExceptionHelper.CreateException());
            }


            Assert.AreEqual(-1, scope.AddStep((StepItem)null));
            Assert.AreEqual(0, scope.AddStep(DateTime.Now, "", "xx"));
            Assert.AreEqual(2, scope.AddStep(DateTime.Now, "x1", "xx"));

            Thread.Sleep(1);
            mock.PipelineContext.PerformanceThresholdMs = 1;
            mock.PipelineContext.End();
            scope.EndSet0(mock.PipelineContext);

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

            using OprLogScope scope = OprLogScope.Start();

            for( int i = 0; i <  10; i++ ) {
                scope.AddStep(DateTime.Now, $"s{i}", new string('a', 10240), DateTime.Now, ExceptionHelper.CreateException());
            }

            mock.PipelineContext.End();
            scope.EndSet0(mock.PipelineContext);

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

            using OprLogScope scope = OprLogScope.Start();

            for( int i = 0; i < LoggingLimit.OprLog.LogsMaxCount + 10; i++ ) {
                scope.Log(Guid.NewGuid().ToString());
            }

            Assert.AreEqual(0, scope.Log(""));
            Assert.AreEqual(2, scope.Log("111111111111"));

            Thread.Sleep(1);
            mock.PipelineContext.PerformanceThresholdMs = 1;
            mock.PipelineContext.End();
            scope.EndSet0(mock.PipelineContext);

            int ret = scope.SaveOprLog(mock.PipelineContext);
            Assert.AreEqual(0, ret);

            Assert.AreEqual(1, scope.OprLog.IsSlow);
            Assert.AreEqual(200, scope.OprLog.Status);
            Assert.AreEqual(LoggingLimit.OprLog.LogsMaxCount, scope.GetLogs().Count);

            Assert.AreEqual(-2, scope.Log("xxxxxxxxxxxxxx"));
        }

    }
}
