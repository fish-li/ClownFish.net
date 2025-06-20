using ClownFish.Jwt;
using ClownFish.Jwt.Impl;

namespace ClownFish.UnitTest.Jwt;
[TestClass]
public class JwtUtilsTest
{
    internal static readonly byte[] JwtKey = Encoding.UTF8.GetBytes("4dd668b33e8d4a05bec7e0ec54b0bd28+494a9286af164a46a809e7e110bf3cec");

    [TestMethod]
    public void Test1()
    {
        var data = new {
            iss = "JwtUtilsTest",
            sub = "all",
            iat = DateTime.Now.ToNumber(),
            exp = DateTime.Now.AddDays(1).ToNumber(),
            UseId = 123,
            UserName = "Fish Li",
            UserRole = "Admin",
            XFlag = 5
        };

        string json = data.ToJson();

        string token1 = JwtUtils.Encode(json, JwtKey, "HS256");
        string token2 = JwtUtils.Encode(json, JwtKey, "HS512");
        Console.WriteLine(token1);

        string text1 = JwtUtils.Decode(token1, JwtKey, "HS256");
        string text2 = JwtUtils.Decode(token2, JwtKey, "HS512");

        Assert.AreEqual(json, text1);
        Assert.AreEqual(json, text2);

        MyAssert.IsError<SignatureVerificationException>(() => {
            _= JwtUtils.Decode(token1, JwtKey, "HS512");
        });

        MyAssert.IsError<SignatureVerificationException>(() => {
            _ = JwtUtils.Decode(token2, JwtKey, "HS256");
        });

        MyAssert.IsError<NotSupportedException>(() => {
            _ = JwtUtils.Encode(json, JwtKey, "HS256111");
        });


        byte[] xxKey = "xxxxxxxxxxxxxxxxxx".ToUtf8Bytes();
        MyAssert.IsError<SignatureVerificationException>(() => {
            _ = JwtUtils.Decode(token1, xxKey, "HS256");
        });

        MyAssert.IsError<SignatureVerificationException>(() => {
            _ = JwtUtils.Decode(token2, xxKey, "HS512");
        });
    }


    [TestMethod]
    public void Test2()
    {
        string json = new { a = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxx" }.ToJson();

        string token1 = JwtUtils.Encode(json, JwtKey, "HS256");
        string token2 = JwtUtils.Encode(json, JwtKey, "HS512");
        Console.WriteLine(token1);

        string text1 = JwtUtils.Decode(token1, JwtKey, "HS256");
        string text2 = JwtUtils.Decode(token2, JwtKey, "HS512");

        Assert.AreEqual(json, text1);
        Assert.AreEqual(json, text2);
        
    }


    [TestMethod]
    public void Test3()
    {
        string header = new { typ = "JWT", alg = "HS256", a =2, b = "xx" }.ToJson();
        string payload = new { ab = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxx" }.ToJson();

        string token1 = JwtUtils.Encode(header, payload, JwtKey, "HS256");
        Console.WriteLine(token1);
    }

    [TestMethod]
    public void Test4()
    {
        string s1 = Guid.NewGuid().ToString() + "大明王朝-1566";
        string s2 = s1.Base64UrlEncode().Base64UrlDecode();
        Assert.AreEqual(s1, s2);

        Assert.AreEqual(string.Empty, JwtUtils.Base64UrlEncode(Empty.Array<byte>()));
        Assert.AreEqual(string.Empty, JwtUtils.Base64UrlEncode(""));

        Assert.AreEqual(string.Empty, JwtUtils.Base64UrlDecode(""));
        Assert.IsNull(JwtUtils.Base64UrlDecode(null));
    }

    [TestMethod]
    public void Test5()
    {
        Assert.AreEqual("HS512", JwtUtils.GetImpl(null).Name);
        Assert.AreEqual("HS256", JwtUtils.GetImpl("HS256").Name);
        Assert.AreEqual("HS512", JwtUtils.GetImpl("HS512").Name);

#if NETCOREAPP
        Assert.AreEqual("RS256", JwtUtils.GetImpl("RS256").Name);
        Assert.AreEqual("RS512", JwtUtils.GetImpl("RS512").Name);

        Assert.AreEqual("ES256", JwtUtils.GetImpl("ES256").Name);
        Assert.AreEqual("ES512", JwtUtils.GetImpl("ES512").Name);
#endif

        MyAssert.IsError<NotSupportedException>(() => {
            _ = JwtUtils.GetImpl("aaaaaaa");
        });
    }

}
