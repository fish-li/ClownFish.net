using ClownFish.UnitTest.Data.Command;
using ClownFish.UnitTest.WebClient;

namespace ClownFish.UnitTest.Log.Logging;

public class TestHandler1 : IAsyncNHttpHandler
{
    public OprLogScope OprLogScope { get; private set; }

    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        CPQueryTest test = new CPQueryTest();
        test.Test_CPQuery的基本CRUD操作();

        await test.Test_CPQuery的基本CRUD操作_Async();

        await test.Test_CPQuery加载实体列表();

        HttpOptionExtensionsTest test2 = new HttpOptionExtensionsTest();
        test2.Test_return_void();
        test2.Test_RemoteWebException();

        this.OprLogScope = OprLogScope.Get();

        this.OprLogScope.OprLog.TenantId = "x_TenantId";
        this.OprLogScope.OprLog.UserId = "x_UserId";
        this.OprLogScope.OprLog.UserName = "x_UserName";
        this.OprLogScope.OprLog.UserRole = "x_UserRole";
        this.OprLogScope.OprLog.Module = "x_ActionModule";
        this.OprLogScope.OprLog.Controller = "x_ActionController";
        this.OprLogScope.OprLog.Action = "x_ActionMethod";
        this.OprLogScope.OprLog.BizId = "x_BizId";
        this.OprLogScope.OprLog.BizName = "x_BizName";
        this.OprLogScope.OprLog.Addition = "x_Addition";


        // 制造一个异常
        using( DbContext db = DbContext.Create("sqlserver") ) {
           db.CPQuery.Create("delete from tablexxx where id > 0").ExecuteNonQuery();
        }
    }

}
