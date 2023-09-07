using ClownFish.Web.Security.Auth;

namespace ClownFish.Web.UnitTest.Security.Jwt;

[TestClass]
public class UsertypeTest
{
    private static readonly JwtProvider s_longNameInstance = JwtProviderTest.CreateJwtProvider(null, false, true, true);
    private static readonly JwtProvider s_shortNameInstance = JwtProviderTest.CreateJwtProvider(null,  true, true, true);

    [TestMethod]
    public void Test_1()
    {
        Test_2way(JwtTest.WebUser);
    }


    private void Test_2way(IUserInfo userInfo)
    {
        TestDecode1(userInfo);

        TestDecode2(userInfo);
    }

    private void TestDecode1(IUserInfo userInfo)
    {
        string token = s_longNameInstance.CreateToken(userInfo, 3600 * 24 * 7);

        LoginTicket ticket2 = s_longNameInstance.DecodeToken(token);
        MyAssert.AreEqual(userInfo, ticket2.User);


        // 包含了完整的命令空间，所以能正常解析
        LoginTicket ticket3 = s_shortNameInstance.DecodeToken(token);
        MyAssert.AreEqual(userInfo, ticket3.User);
    }


    private void TestDecode2(IUserInfo userInfo)
    {
        string token = s_shortNameInstance.CreateToken(userInfo, 3600 * 24 * 7);

        string token2 = s_longNameInstance.CreateToken(userInfo, 3600 * 24 * 7);
        Assert.IsTrue(token.Length < token2.Length);


        LoginTicket ticket1 = s_shortNameInstance.DecodeToken(token);
        MyAssert.AreEqual(userInfo, ticket1.User);


        // 由于没有完整的命令空间，所以不能正常识别，只能按Unknown方式处理
        LoginTicket ticket3 = s_longNameInstance.DecodeToken(token);
        Assert.IsInstanceOfType(ticket3.User, typeof(UnknownUserInfo));
    }


}
