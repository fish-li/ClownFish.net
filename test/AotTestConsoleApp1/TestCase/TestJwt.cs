using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Jwt;
using ClownFish.Jwt.Impl;

namespace AotTestConsoleApp1.TestCase;
internal class TestJwt
{

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JwtRSA256))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JwtRSA512))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JwtECD256))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JwtECD512))]
    public static async Task Run()
    {
        await Task.CompletedTask;

        Test1();
        Test2();


        Test_RS256_clownfish_生成_解析();
        Test_RS512_clownfish_生成_解析();
        Test_ES256_clownfish_生成_解析();
        Test_ES512_clownfish_生成_解析();
    }

    internal static readonly byte[] JwtKey = Encoding.UTF8.GetBytes("4dd668b33e8d4a05bec7e0ec54b0bd28+494a9286af164a46a809e7e110bf3cec");

    private static void Test1()
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

        string text1 = JwtUtils.Decode(token1, JwtKey, "HS256");
        string text2 = JwtUtils.Decode(token2, JwtKey, "HS512");

        Assert.AreEqual(json, text1);
        Assert.AreEqual(json, text2);
    }

    private static void Test2()
    {
        string s1 = Guid.NewGuid().ToString() + "大明王朝-1566";
        string s2 = s1.Base64UrlEncode().Base64UrlDecode();
        Assert.AreEqual(s1, s2);

        Assert.AreEqual(string.Empty, JwtUtils.Base64UrlEncode(Empty.Array<byte>()));
        Assert.AreEqual(string.Empty, JwtUtils.Base64UrlEncode(""));

        Assert.AreEqual(string.Empty, JwtUtils.Base64UrlDecode(""));
        Assert.IsNull(JwtUtils.Base64UrlDecode(null));
    }


    private static readonly EndClientUserInfo s_payloadData = "{\"TenantId\":\"25841548187\",\"ClientId\":\"48742485458741658\",\"AppId\":\"7e3e3c15a3df46e4b82afe86e2ddd247\",\"AppName\":\"TxClientX\",\"Version\":\"5.25.10925.10/4.19.90.52/.NET 8.0.6\",\"ClientRole\":\"TxClient\",\"HostName\":\"k8s-node-2\",\"Ip\":\"10.1.60.136\",\"Cluster\":\"bigdata-yhyt-idc\",\"OsKind\":2,\"OsName\":\"Ubuntu 24.04.3 LTS\",\"CpuKind\":\"X64\",\"TimeZone\":\"Asia/Shanghai\",\"Culture\":\"\",\"DeployMode\":7,\"RunMode\":200,\"GrayFlag\":0}".FromJson<EndClientUserInfo>();

    private static void Test_RS256_clownfish_生成_解析()
    {
        X509Certificate2 x509 = X509Utils.CreateRSASignCertificate(HashAlgorithmName.SHA256, "fishli", 2048);


        string payload = s_payloadData.ToJson();
        string token = JwtUtils.Encode2(payload, x509, "RS256");

        string body = JwtUtils.Decode2(token, x509, "RS256");

        Assert.AreEqual(body, payload);


        string header = new { typ = "JWT", alg = "HS256", a = 2, b = "xx" }.ToJson();
        string token2 = JwtUtils.Encode2(header, payload, x509, "RS256");
        //Console.WriteLine(token2);

        string body2 = JwtUtils.Decode2(token2, x509, "RS256");
        Assert.AreEqual(body2, payload);
    }


    private static void Test_RS512_clownfish_生成_解析()
    {
        X509Certificate2 x509 = X509Utils.CreateRSASignCertificate(HashAlgorithmName.SHA512, "fishli", 4096);


        string payload = s_payloadData.ToJson();
        string token = JwtUtils.Encode2(payload, x509, "RS512");
        //Console.WriteLine(token);

        string body = JwtUtils.Decode2(token, x509, "RS512");

        Assert.AreEqual(body, payload);
    }

    private static void Test_ES256_clownfish_生成_解析()
    {
        X509Certificate2 x509 = X509Utils.CreateECDsaSignCertificate(HashAlgorithmName.SHA256, "fishli");


        string payload = s_payloadData.ToJson();
        string token = JwtUtils.Encode2(payload, x509, "ES256");

        string body = JwtUtils.Decode2(token, x509, "ES256");

        Assert.AreEqual(body, payload);
    }


    private static void Test_ES512_clownfish_生成_解析()
    {
        X509Certificate2 x509 = X509Utils.CreateECDsaSignCertificate(HashAlgorithmName.SHA512, "fishli");


        string payload = s_payloadData.ToJson();
        string token = JwtUtils.Encode2(payload, x509, "ES512");

        string body = JwtUtils.Decode2(token, x509, "ES512");

        Assert.AreEqual(body, payload);
    }


}



public static class X509Utils
{
    public static X509Certificate2 CreateRSASignCertificate(HashAlgorithmName hashName, string key, int keySize)
    {
        string subjectName = "CN=ClownFish_TEMP_X509_RSA_" + hashName.Name;

        using RSA rsa = RSA.Create(keySize);
        X500DistinguishedName distinguishedName = new X500DistinguishedName(subjectName);

        CertificateRequest request = new CertificateRequest(distinguishedName, rsa, hashName, RSASignaturePadding.Pkcs1);

        return CreateCertificate(request, key);
    }

    public static X509Certificate2 CreateECDsaSignCertificate(HashAlgorithmName hashName, string key)
    {
        string subjectName = "CN=ClownFish_TEMP_X509_ECDsa_" + hashName.Name;

        using ECDsa ecdsa = ECDsa.Create();
        X500DistinguishedName distinguishedName = new X500DistinguishedName(subjectName);

        CertificateRequest request = new CertificateRequest(distinguishedName, ecdsa, hashName);

        return CreateCertificate(request, key);
    }

    private static X509Certificate2 CreateCertificate(CertificateRequest request, string key)
    {
        // 设置证书请求的序列号
        string serialNumber = HashHelper.Sha1(key);
        request.CertificateExtensions.Add(new X509Extension("2.5.29.19", serialNumber.GetBytes(), false));

        // 添加所需的扩展属性（例如 Key Usage、Subject Alternative Name 等）
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));
        request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));

        DateTimeOffset notBefore = DateTimeOffset.UtcNow;
        DateTimeOffset notAfter = notBefore.AddYears(100);
        X509Certificate2 certificate = request.CreateSelfSigned(notBefore, notAfter);

        return certificate;
    }
}

