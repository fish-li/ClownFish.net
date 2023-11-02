using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.UnitTest.Http.Mock;
using ClownFish.UnitTest.Http.Pipleline.Test;

namespace ClownFish.UnitTest.Http.Pipleline;
[TestClass]
public class NHttpRequest2Test
{
    [TestMethod]
    public async Task Test_GetBodyTextAsync()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);

        string body = await httpContext.Request.GetBodyTextAsync();
        Console.WriteLine(body);
    }


    [TestMethod]
    public async Task Test_ReadBodyAsBytesAsync()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);

        byte[] body = await httpContext.Request.ReadBodyAsBytesAsync();
        Console.WriteLine(body.ToUtf8String());
    }




    [TestMethod]
    public async Task Test_ReadBodyAsTextWithMonitorAsync()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        string body = await httpContext.Request.ReadBodyAsTextWithMonitorAsync();
        Console.WriteLine(body);
    }


    [TestMethod]
    public async Task Test_ReadBodyAsBytesWithMonitorAsync()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        byte[] body = await httpContext.Request.ReadBodyAsBytesWithMonitorAsync();
        Console.WriteLine(body.ToUtf8String());
    }

#if NETCOREAPP
    [TestMethod]
    public async Task Test_RequestToBytesWithMonitorAsync()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        byte[] body = await httpContext.Request.RequestToBytesWithMonitorAsync();
        Console.WriteLine(body.ToUtf8String());
    }
#endif




}
