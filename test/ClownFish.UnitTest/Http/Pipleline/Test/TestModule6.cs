using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Pipleline;

namespace ClownFish.UnitTest.Http.Pipleline.Test
{
    public class TestModule6 : NHttpModule
    {
        public override void PreFindAction(NHttpContext httpContext)
        {
            base.PreFindAction(httpContext);

            // 这里代替URL路由，直接选择了一个 Handler
            httpContext.PipelineContext.SetHttpHandler(new TestHandler1());
        }

        public override void PreRequestExecute(NHttpContext httpContext)
        {
            base.PreRequestExecute(httpContext);

            httpContext.Response.End();
        }
    }


}
