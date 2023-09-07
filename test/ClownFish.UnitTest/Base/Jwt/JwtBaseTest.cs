using ClownFish.Base.Jwt;
using ClownFish.Base.Jwt.Impl;

namespace ClownFish.UnitTest.Base.Jwt;

[TestClass]
public class JwtBaseTest
{
    private static readonly object s_user = new {
        UserId = 123,
        UserName = "test",
        UserRole = "admin"
    };

    [TestMethod]
    public void Test_1()
    {
        JwtBase jwt = JwtUtils.CreateImpl(null);

        string json1 = s_user.ToJson();
        string token = jwt.Encode(json1, JwtUtilsTest.JwtKey);
        string json2 = jwt.Decode(token, JwtUtilsTest.JwtKey);

        Assert.AreEqual(json2, json1);
    }


    [TestMethod]
    public void Test_Error()
    {
        JwtBase jwt = JwtUtils.CreateImpl(null);

        MyAssert.IsError<ArgumentException>(() => {
            _ = jwt.Encode(null, JwtUtilsTest.JwtKey);
        });



        string token = jwt.Encode(s_user.ToJson(), JwtUtilsTest.JwtKey) + "xx";
        MyAssert.IsError<SignatureVerificationException>(() => {
            _ = jwt.Decode(token, JwtUtilsTest.JwtKey);
        });

    }
}

