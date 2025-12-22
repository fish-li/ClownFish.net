using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.MockTest;
using ClownFish.Log;
using ClownFish.Web.Security.Auth;
using AuthenticationManager = ClownFish.Web.Security.Auth.AuthenticationManager;

namespace ClownFish.Web.UnitTest.Security.Auth;

[TestClass]
public class TokenHelperTest
{
    [TestMethod]
    public void Test1()
    {
        WebUserInfo userinfo = new WebUserInfo {
            TenantId = "11111111",
            UserId = "222222222222",
            UserRole = "amdin",
            UserName = "Test",
        };
        string token = AuthenticationManager.GetLoginToken(userinfo, 3600 * 24 * 100);

        string requestText = $@"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
x-token: {token}
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            AuthenticationManager.AuthenticationUser(mock.HttpContext);   // 登录 成功
            Assert.IsNotNull(mock.HttpContext.User);
            //----------------------------------
            List<NameTime> logs = mock.PipelineContext.OprLogScope.GetLogs();
            Assert.IsNull(logs);            
        }
    }


    [TestMethod]
    public void Test2()
    {
        WebUserInfo userinfo = new WebUserInfo {
            TenantId = "11111111",
            UserId = "222222222222",
            UserRole = "amdin",
            UserName = "Test",
        };
        string token = AuthenticationManager.GetLoginToken(userinfo, 3600 * 24 * 100) + "xx";

        string requestText = $@"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
x-token: {token}
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            AuthenticationManager.AuthenticationUser(mock.HttpContext);  // 登录 失败，token无效
            Assert.IsNull(mock.HttpContext.User);
            //----------------------------------
            Console.WriteLine(mock.PipelineContext.OprLogScope.GetLogsText(mock.PipelineContext));
            List<NameTime> logs = mock.PipelineContext.OprLogScope.GetLogs();

            Assert.IsTrue(logs.Any(x => x.Name.Contains("[身份认证失败] DecodePayload: catch-Exception")));
            Assert.IsTrue(logs.Any(x => x.Name.Contains("[身份认证失败] DecodePayload return null")));
            Assert.IsTrue(logs.Any(x => x.Name.Contains("[身份凭证来源] Header=x-token")));
        }
    }

    [TestMethod]
    public void Test3()
    {
        WebUserInfo userinfo = new WebUserInfo {
            TenantId = "11111111",
            UserId = "222222222222",
            UserRole = "amdin",
            UserName = "Test",
        };
        string token = AuthenticationManager.GetLoginToken(userinfo, -100);  // 登录 失败，token已过期

        string requestText = $@"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
x-token: {token}
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            AuthenticationManager.AuthenticationUser(mock.HttpContext);
            Assert.IsNull(mock.HttpContext.User);
            //----------------------------------
            Console.WriteLine(mock.PipelineContext.OprLogScope.GetLogsText(mock.PipelineContext));
            List<NameTime> logs = mock.PipelineContext.OprLogScope.GetLogs();

            Assert.IsTrue(logs.Any(x => x.Name.Contains("[身份认证失败] DecodeJson: VerifyExpiration=false")));
            Assert.IsTrue(logs.Any(x => x.Name.Contains("[身份凭证来源] Header=x-token")));
        }
    }
}
