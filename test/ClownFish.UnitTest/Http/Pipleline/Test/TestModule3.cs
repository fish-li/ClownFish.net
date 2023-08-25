using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Http.Pipleline;

namespace ClownFish.UnitTest.Http.Pipleline.Test
{
    public class TestModule3 : NHttpModule
    {
        public override void BeginRequest(NHttpContext httpContext)
        {
            base.BeginRequest(httpContext);

            httpContext.RegisterForDispose(new XDisposableObject());
            httpContext.RegisterForDispose(new MemoryStream());
        }

        public override void ResolveRequestCache(NHttpContext httpContext)
        {
            base.ResolveRequestCache(httpContext);

            MyAssert.IsError<ArgumentNullException>(() => {
                httpContext.PipelineContext.SetHttpHandler(null);
            });

            // 这里代替URL路由，直接选择了一个 Handler
            httpContext.PipelineContext.SetHttpHandler(new TestHandler1());
        }
    }


    public class XDisposableObject : IDisposable
    {
        public static readonly ValueCounter InstanceCounter = new ValueCounter("xx");


        public XDisposableObject()
        {
            InstanceCounter.Increment();
        }

        public void Dispose()
        {
            InstanceCounter.Decrement();

            throw new InvalidOperationException();
        }
    }
}
