using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Http.Pipleline;

namespace ClownFish.UnitTest.Http.Pipleline.Test
{
    public class TestModule4 : NHttpModule
    {
        public static readonly ValueCounter ErrorCounter = new ValueCounter("xx");
        public static readonly ValueCounter EndCounter = new ValueCounter("xx");

        public override void PreFindAction(NHttpContext httpContext)
        {
            base.PreFindAction(httpContext);

            // 这里代替URL路由，直接选择了一个 Handler
            httpContext.PipelineContext.SetHttpHandler(new TestHandler1());
        }

        public override void OnError(NHttpContext httpContext)
        {
            base.OnError(httpContext);

            ErrorCounter.Increment();

            throw new InvalidOperationException();
        }

        public override void EndRequest(NHttpContext httpContext)
        {
            base.EndRequest(httpContext);

            EndCounter.Increment();

            throw new InvalidOperationException();
        }
    }
}
