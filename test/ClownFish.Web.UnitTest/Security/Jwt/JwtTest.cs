using ClownFish.Web.Security.Auth;

namespace ClownFish.Web.UnitTest.Security.Jwt;
internal class JwtTest
{
    internal static readonly byte[] JwtKey = Encoding.UTF8.GetBytes("4dd668b33e8d4a05bec7e0ec54b0bd28+494a9286af164a46a809e7e110bf3cec");

    internal static readonly WebUserInfo WebUser = new WebUserInfo {
        TenantId = "t596c897825049",
        TenantCode = "test123",
        UserId = "fish",
        UserName = "Fish Li",
        UserRole = "Admin",
        UserCode = "fish",
        UserType = "???",
        UserData = "111",
        ExtData = "6666",
        GrayFlag = 2
    };


    internal static void UpdateUserData()
    {
        WebUser.ExtData = "中文汉字！@……%￥￥&￥（*……~！@#~#%……*……&*（（）——+（）——&@#~#@" + Guid.NewGuid().ToString("N");
    }
}
