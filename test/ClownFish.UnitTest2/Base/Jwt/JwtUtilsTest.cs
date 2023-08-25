using System;
using System.Text;
using ClownFish.Base;
using ClownFish.Base.Jwt;
using ClownFish.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Jwt;
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


}
