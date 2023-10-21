using ClownFish.Jwt;
using ClownFish.UnitTest.Base.Cryptography;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;

#if NET6_0_OR_GREATER
namespace ClownFish.UnitTest.Jwt;

[TestClass]
public class JwtUtilsTest2
{
    private static readonly string s_key = "4dd668b33e8d4a05bec7e0ec54b0bd28+494a9286af164a46a809e7e110bf3cec";
    private static readonly byte[] s_keyBytes = Encoding.UTF8.GetBytes(s_key);

    private static string CreateJwt(IJwtAlgorithm algorithm, object payload, string key)
    {
        IJsonSerializer serializer = new JsonNetSerializer();
        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

        return encoder.Encode(payload, key);
    }

    private static readonly object s_payloadData = new Dictionary<string, object>
    {
        { "claim1", 0 },
        { "claim2", "claim2-value" }
    };


    #region HMACSHA

    [TestMethod]
    public void Test_HS256_clownfish_生成_解析()
    {
        string payload = s_payloadData.ToJson();

        string token = JwtUtils.Encode(payload, s_keyBytes, "HS256");

        string body = JwtUtils.Decode(token, s_keyBytes, "HS256");

        Assert.AreEqual(body, payload);
    }

    [TestMethod]
    public void Test_HS512_clownfish_生成_解析()
    {
        string payload = s_payloadData.ToJson();

        string token = JwtUtils.Encode(payload, s_keyBytes, "HS512");

        string body = JwtUtils.Decode(token, s_keyBytes, "HS512");

        Assert.AreEqual(body, payload);
    }


    [TestMethod]
    public void Test_HS256_3rd生成_Clownfish解析()
    {
        IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
        string token = CreateJwt(algorithm, s_payloadData, s_key);
        Console.WriteLine(token);

        string body = JwtUtils.Decode(token, s_keyBytes, "HS256");
        string payload = s_payloadData.ToJson();

        Assert.AreEqual(body, payload);
    }


    [TestMethod]
    public void Test_HS256_Clownfish生成_3rd解析()
    {
        string payload = s_payloadData.ToJson();

        string token = JwtUtils.Encode(payload, s_keyBytes, "HS256");
        Console.WriteLine(token);

        string body = JwtBuilder.Create()
                     .WithAlgorithm(new HMACSHA256Algorithm())
                     .WithSecret(s_keyBytes)
                     .MustVerifySignature()
                     .Decode(token);

        Assert.AreEqual(body, payload);
    }


    [TestMethod]
    public void Test_HS512_3rd生成_Clownfish解析()
    {
        IJwtAlgorithm algorithm = new HMACSHA512Algorithm();
        string token = CreateJwt(algorithm, s_payloadData, s_key);
        Console.WriteLine(token);

        string body = JwtUtils.Decode(token, s_keyBytes, "HS512");
        string payload = s_payloadData.ToJson();

        Assert.AreEqual(body, payload);
    }


    [TestMethod]
    public void Test_HS512_Clownfish生成_3rd解析()
    {
        string payload = s_payloadData.ToJson();

        string token = JwtUtils.Encode(payload, s_keyBytes, "HS512");
        Console.WriteLine(token);

        string body = JwtBuilder.Create()
                     .WithAlgorithm(new HMACSHA512Algorithm())
                     .WithSecret(s_keyBytes)
                     .MustVerifySignature()
                     .Decode(token);

        Assert.AreEqual(body, payload);
    }

    #endregion

    #region RSA


    [TestMethod]
    public void Test_RS256_clownfish_生成_解析()
    {
        X509Certificate2 x509 = X509Utils.CreateRSASignCertificate(HashAlgorithmName.SHA256, "fishli", 2048);

        // 导出包含私钥的证书字节数组
        byte[] certBytes = x509.Export(X509ContentType.Pfx, "pwd9999999999");
        File.WriteAllBytes("files/TempX509_RS256.pfx", certBytes);

        string payload = s_payloadData.ToJson();
        string token = JwtUtils.Encode2(payload, x509, "RS256");

        string body = JwtUtils.Decode2(token, x509, "RS256");

        Assert.AreEqual(body, payload);
    }

    [TestMethod]
    public void Test_RS512_clownfish_生成_解析()
    {
        X509Certificate2 x509 = X509Utils.CreateRSASignCertificate(HashAlgorithmName.SHA512, "fishli", 4096);

        // 导出包含私钥的证书字节数组
        byte[] certBytes = x509.Export(X509ContentType.Pfx, "pwd9999999999");
        File.WriteAllBytes("files/TempX509_RS512.pfx", certBytes);

        string payload = s_payloadData.ToJson();
        string token = JwtUtils.Encode2(payload, x509, "RS512");
        Console.WriteLine(token);

        string body = JwtUtils.Decode2(token, x509, "RS512");

        Assert.AreEqual(body, payload);
    }



    [TestMethod]
    public void Test_RS256_3rd生成_Clownfish解析()
    {
        X509Certificate2 x509 = X509Utils.CreateRSASignCertificate(HashAlgorithmName.SHA256, "fishli", 2048);

        IJwtAlgorithm algorithm = new RS256Algorithm(x509);

        string token = CreateJwt(algorithm, s_payloadData, (string)null);
        Console.WriteLine(token);

        string body = JwtUtils.Decode2(token, x509, "RS256");
        string payload = s_payloadData.ToJson();

        Assert.AreEqual(body, payload);
    }


    [TestMethod]
    public void Test_RS256_Clownfish生成_3rd解析()
    {
        X509Certificate2 x509 = X509Utils.CreateRSASignCertificate(HashAlgorithmName.SHA256, "fishli", 2048);

        string payload = s_payloadData.ToJson();
        string token = JwtUtils.Encode2(payload, x509, "RS256");
        Console.WriteLine(token);

        string body = JwtBuilder.Create()
                     .WithAlgorithm(new RS256Algorithm(x509))
                     .MustVerifySignature()
                     .Decode(token);

        Assert.AreEqual(body, payload);
    }



    [TestMethod]
    public void Test_RS512_3rd生成_Clownfish解析()
    {
        X509Certificate2 x509 = X509Utils.CreateRSASignCertificate(HashAlgorithmName.SHA512, "fishli", 4096);
        IJwtAlgorithm algorithm = new RS512Algorithm(x509);

        string token = CreateJwt(algorithm, s_payloadData, (string)null);
        Console.WriteLine(token);


        string body = JwtUtils.Decode2(token, x509, "RS512");
        string payload = s_payloadData.ToJson();

        Assert.AreEqual(body, payload);
    }


    [TestMethod]
    public void Test_RS512_Clownfish生成_3rd解析()
    {
        X509Certificate2 x509 = X509Utils.CreateRSASignCertificate(HashAlgorithmName.SHA512, "fishli", 4096);

        string payload = s_payloadData.ToJson();
        string token = JwtUtils.Encode2(payload, x509, "RS512");
        Console.WriteLine(token);

        string body = JwtBuilder.Create()
                     .WithAlgorithm(new RS512Algorithm(x509))
                     .MustVerifySignature()
                     .Decode(token);

        Assert.AreEqual(body, payload);
    }

    #endregion

    #region ECDsa

    [TestMethod]
    public void Test_ES256_clownfish_生成_解析()
    {
        X509Certificate2 x509 = X509Utils.CreateECDsaSignCertificate(HashAlgorithmName.SHA256, "fishli");

        // 导出包含私钥的证书字节数组
        byte[] certBytes = x509.Export(X509ContentType.Pfx, "pwd9999999999");
        File.WriteAllBytes("files/TempX509_ES256.pfx", certBytes);

        string payload = s_payloadData.ToJson();
        string token = JwtUtils.Encode2(payload, x509, "ES256");

        string body = JwtUtils.Decode2(token, x509, "ES256");

        Assert.AreEqual(body, payload);
    }

    [TestMethod]
    public void Test_ES512_clownfish_生成_解析()
    {
        X509Certificate2 x509 = X509Utils.CreateECDsaSignCertificate(HashAlgorithmName.SHA512, "fishli");

        // 导出包含私钥的证书字节数组
        byte[] certBytes = x509.Export(X509ContentType.Pfx, "pwd9999999999");
        File.WriteAllBytes("files/TempX509_ES512.pfx", certBytes);

        string payload = s_payloadData.ToJson();
        string token = JwtUtils.Encode2(payload, x509, "ES512");

        string body = JwtUtils.Decode2(token, x509, "ES512");

        Assert.AreEqual(body, payload);
    }



    [TestMethod]
    public void Test_ECD256_3rd生成_Clownfish解析()
    {
        X509Certificate2 x509 = X509Utils.CreateECDsaSignCertificate(HashAlgorithmName.SHA256, "fishli");
        IJwtAlgorithm algorithm = new ES256Algorithm(x509);

        string token = CreateJwt(algorithm, s_payloadData, (string)null);
        Console.WriteLine(token);


        string body = JwtUtils.Decode2(token, x509, "ES256");
        string payload = s_payloadData.ToJson();

        Assert.AreEqual(body, payload);
    }


    [TestMethod]
    public void Test_ECD256_Clownfish生成_3rd解析()
    {
        X509Certificate2 x509 = X509Utils.CreateECDsaSignCertificate(HashAlgorithmName.SHA256, "fishli");

        string payload = s_payloadData.ToJson();
        string token = JwtUtils.Encode2(payload, x509, "ES256");
        Console.WriteLine(token);

        string body = JwtBuilder.Create()
                     .WithAlgorithm(new ES256Algorithm(x509))
                     .MustVerifySignature()
                     .Decode(token);

        Assert.AreEqual(body, payload);
    }

    [TestMethod]
    public void Test_ECD512_3rd生成_Clownfish解析()
    {
        X509Certificate2 x509 = X509Utils.CreateECDsaSignCertificate(HashAlgorithmName.SHA512, "fishli");
        IJwtAlgorithm algorithm = new ES512Algorithm(x509);

        string token = CreateJwt(algorithm, s_payloadData, (string)null);
        Console.WriteLine(token);


        string body = JwtUtils.Decode2(token, x509, "ES512");
        string payload = s_payloadData.ToJson();

        Assert.AreEqual(body, payload);
    }


    [TestMethod]
    public void Test_ECD512_Clownfish生成_3rd解析()
    {
        X509Certificate2 x509 = X509Utils.CreateECDsaSignCertificate(HashAlgorithmName.SHA512, "fishli");

        string payload = s_payloadData.ToJson();
        string token = JwtUtils.Encode2(payload, x509, "ES512");
        Console.WriteLine(token);

        string body = JwtBuilder.Create()
                     .WithAlgorithm(new ES512Algorithm(x509))
                     .MustVerifySignature()
                     .Decode(token);

        Assert.AreEqual(body, payload);
    }

    #endregion


    [TestMethod]
    public void Test_11()
    {
        string configText = File.ReadAllText("files/ClownFish_AuthX509_RS256.conf");
        var x509 = X509Finder.LoadFromConfigFile(configText);

        string token1 = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJVc2VyIjp7IiR0eXBlIjoiTmVidWxhVXNlciIsIlVzZXJJZCI6ImxpcWY" +
            "wMSIsIlVzZXJOYW1lIjoi5p2O5aWH5bOwIiwiVXNlclJvbGUiOiJOZWJ1bGFVc2VyIn0sIklzc3VlciI6Ik5lYnVsYS5BbGxJbk9uZSIsIklzc" +
            "3VlVGltZSI6MjAyMzEwMjAxODIwMzMsIkV4cGlyYXRpb24iOjIwMjQxMDE5MTgyMDMzfQ.bqlePgvdZwYvPrnlB_HuSHkbYREPIFnwlaBe7gHMr" +
            "MzpmIgyUQ1TXY-z1eJiZreFhpXZPT6eo6WBAPfug_UVNCduOnCQFDfehvIqrLLdtOBjP1E0yfbSocudikpV35n4tUIOJBwScynnZ-LNCLouCi5z_" +
            "0hYArGvmnYdyb5MZDdn55wbkFZQsKFr8qpDg0Nuqatc77dyIeyYbm6wOGsh844vCDC15_OvVBb2AVknzuTJ071IxQGQ1oayD0C68XWP5oJMyIBfX" +
            "v1cSZkZDcyJLx2FWIuxySDcPMPTY7QATUSX4xsH9ftxZjqMpGMahoBUTckuBvdcgIy6poRMfcH_Rg";

        string body1 = JwtUtils.Decode2(token1, x509, "RS256");
        Console.WriteLine(body1);

        string token2 = token1.Substring(0, token1.Length - 1) + "h";   // 把最后一位从  g => (h - v) 之间任何一个字符
        string body2 = JwtUtils.Decode2(token2, x509, "RS256");         // 这里居然可以通过RSA的签名验证！！
        Assert.AreEqual(body1, body2);

        string token3 = token1.Substring(0, token1.Length - 1) + "v";
        string body3 = JwtUtils.Decode2(token3, x509, "RS256");
        Assert.AreEqual(body1, body3);

        MyAssert.IsError<SignatureVerificationException>(() => {
            string token3 = token1.Substring(0, token1.Length - 2) + "rw";
            string body3 = JwtUtils.Decode2(token3, x509, "RS256");
        });

        MyAssert.IsError<SignatureVerificationException>(() => {
            string token3 = token1.Substring(0, token1.Length - 2) + "rg";
            string body3 = JwtUtils.Decode2(token3, x509, "RS256");
        });
        
    }
}
#endif
