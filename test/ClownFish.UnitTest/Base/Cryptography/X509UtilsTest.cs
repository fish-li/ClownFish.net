using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using ClownFish.UnitTest.Jwt;

namespace ClownFish.UnitTest.Base.Cryptography;


// 数字证书及结构   https://www.jianshu.com/p/03151a281deb

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

[TestClass]
public class X509UtilsTest
{

    [TestMethod]
    public void Test_Sign()
    {
        string key = Guid.NewGuid().ToString();
        byte[] data = Guid.NewGuid().ToByteArray();

        X509Certificate2 x1 = X509Utils.CreateRSASignCertificate(HashAlgorithmName.SHA512, key, 4096);
        X509Certificate2 x2 = X509Utils.CreateRSASignCertificate(HashAlgorithmName.SHA512, key, 4096);

        string sign = x1.Sign(data);
        Assert.IsTrue(x1.Verify(data, sign));

        string sign2 = x2.Sign(data);
        Assert.IsTrue(x2.Verify(data, sign2));

        //string sign3 = x1.Sign(data);
        //Assert.IsTrue(x2.Verify(data, sign3));

        //string sign4 = x2.Sign(data);
        //Assert.IsTrue(x1.Verify(data, sign4));

        string sign5 = x1.Sign(data);
        string sign6 = sign5.Substring(0, sign5.Length - 2) + "xx";
        Assert.IsFalse(x1.Verify(data, sign6));
    }

    [TestMethod]
    public void Test_Sign2()
    {
        byte[] data = "1fd50ea4-5b77-44b3-af4d-a074cd0126cc".GetBytes();

        X509Certificate2 x1 = X509FinderTest.LoadPfxFile();

        string sign = x1.Sign(data);        
        Assert.IsTrue(x1.Verify(data, sign));

        string sign2 = "xx" + sign.Substring(0, sign.Length - 2);
        Assert.IsFalse(x1.Verify(data, sign2));

        string sign3 = sign.Substring(0, sign.Length - 2) + "xx";
        Assert.IsFalse(x1.Verify(data, sign3));
    }
}