using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Pipleline;
using ClownFish.Base;

namespace ClownFish.UnitTest.Http.Pipleline.Test
{
    public class TestModule2 : NHttpModule
    {
        public override void BeginRequest(NHttpContext httpContext)
        {
            base.BeginRequest(httpContext);

            // 这里代替URL路由，直接选择了一个 Handler
            //httpContext.PipelineContext.SetHttpHandler(new TestHandler1());

            MyAssert.IsError<ArgumentNullException>(() => {
                httpContext.PipelineContext.SetAction(null, true);
            });

            TestHandler1 handler = new TestHandler1();
            MethodInfo method = typeof(TestHandler1).GetInstanceMethod("ProcessRequestAsync");
            ActionDescription action = new ActionDescription(handler, method);

            httpContext.PipelineContext.SetAction(action, true);


            MyAssert.IsError<InvalidOperationException>(() => {
                httpContext.PipelineContext.SetAction(action, true);
            });
        }
    }
}
