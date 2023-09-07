using ClownFish.Base.Jwt;
using ClownFish.Web.Security.Auth;
using AuthenticationManager = ClownFish.Web.Security.Auth.AuthenticationManager;

namespace ClownFish.Web.UnitTest.Security.Jwt;

[TestClass]
public class JwtEncDecTest
{

    [TestMethod]
    public void Test_JwtHelperV3()
    {
        List<string> algNames = new List<string> { "HS256", "HS512" };

        foreach( string alg in algNames ) {
            JwtProvider jwt = JwtProviderTest.CreateJwtProvider(alg, false, true, true);

            for( int i = 0; i < 1000; i++ ) {
                JwtTest.UpdateUserData();

                string token1 = jwt.CreateToken(JwtTest.WebUser, 10000);
                LoginTicket ticket1 = jwt.DecodeToken(token1);
                MyAssert.AreEqual(JwtTest.WebUser, ticket1.User);
            }
        }
    }


    [TestMethod]
    public void Test_JwtUtils()
    {
        List<string> algNames = new List<string> { "HS256", "HS512" };
        foreach( string alg in algNames ) {
            for( int i = 0; i < 1000; i++ ) {
                JwtTest.UpdateUserData();

                string json1 = JwtTest.WebUser.ToJson();
                string token1 = JwtUtils.Encode(json1, JwtTest.JwtKey, alg);
                string json1b = JwtUtils.Decode(token1, JwtTest.JwtKey, alg);
                MyAssert.AreEqual(json1, json1b);
            }
        }
    }


    [TestMethod]
    public void Test_DecodeUserInfoWithoutVerify()
    {
        JwtProvider jwt = JwtProviderTest.CreateJwtProvider("HS512", true, true, true);

        // 生成一个过期（无效）的身份凭证
        string token = jwt.CreateToken(JwtTest.WebUser, DateTime.Now, DateTime.Now.AddHours(-5));
        //Console.WriteLine(token);

        var ticket = jwt.DecodeToken(token);

        // 由于时间过期，所以结果为NULL
        Assert.IsNull(ticket);

        // 直接从凭证中读取用户信息，不需要密钥，不检查过期时间
        IUserInfo user = AuthenticationManager.DecodeUserInfoWithoutVerify(token);

        // 确定可以读取到用户信息
        Assert.IsNotNull(user);
        MyAssert.AreEqual(JwtTest.WebUser, user);
    }
}
