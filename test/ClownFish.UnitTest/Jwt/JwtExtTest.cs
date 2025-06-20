using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Jwt;
using ClownFish.Jwt.Impl;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace ClownFish.UnitTest.Jwt;
[TestClass]
public class JwtExtTest
{
    static JwtExtTest()
    {
        JwtSm3Impl.RegisterImpl();
    }

    [TestMethod]
    public void Test1()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            JwtExtMananger.RegisterAlgorithmImpl(null);
        });

        MyAssert.IsError<ValidationException2>(() => {
            JwtUtils.RegisterAlgorithmImpl<Xx1JwtImpl>();
        });

        MyAssert.IsError<ValidationException2>(() => {
            JwtUtils.RegisterAlgorithmImpl<Xx2JwtImpl>();
        });

        JwtBase jwtImpl = JwtUtils.GetImpl("SM3");
        Assert.AreEqual("SM3", jwtImpl.Name);
    }

    [TestMethod]
    public  void Test2()
    {
        byte[] key = "3a1660e5162e3eaef3hc3oyu98fhrlwd".UnBase64();
        string payload = new {sub = "oc",  ndb = 1750316350, exp = 1750323550, jti = Guid.NewGuid().ToString()}.ToJson();
        string token = JwtUtils.Encode(payload, key, "SM3");   // 不指定 header，使用默认值
        Console.WriteLine(token);

        string payload2 = JwtUtils.Decode(token, key, "SM3");
        Assert.AreEqual(payload, payload2);
    }

    [TestMethod]
    public void Test3()
    {
        byte[] key = "3a1660e5162e3eaef3hc3oyu98fhrlwd".UnBase64();
        string header = new { alg = "SM3", kid = "1732163717" }.ToJson();
        string payload = new { sub = "oc", ndb = 1750316350, exp = 1750323550, jti = Guid.NewGuid().ToString() }.ToJson();
        string token = JwtUtils.Encode(header, payload, key, "SM3");
        Console.WriteLine(token);

        string payload2 = JwtUtils.Decode(token, key, "SM3");
        Assert.AreEqual(payload, payload2);
    }
}




public class JwtSm3Impl : IJwtAlgorithm2
{
    public string Name => "SM3";

    private readonly string _defaultHeader = new { typ = "JWT", alg = "SM3"}.ToJson();

    static JwtSm3Impl()
    {
        // 放在这里可确保只调用一次，防止一些2货多次调用
        JwtUtils.RegisterAlgorithmImpl<JwtSm3Impl>();
    }

    public static void RegisterImpl()
    {
        // 触发 cctor
    }

    public string GetHeaderJson()
    {
        return _defaultHeader;
    }

    public string GetSignature(object secret, byte[] bytesToSign)
    {
        if( secret == null || secret is not byte[] key )
            throw new ArgumentException("secret is error!");

        KeyParameter parameters = new KeyParameter(key);
        SM3Digest digest = new SM3Digest();

        HMac hMac = new HMac(digest);
        hMac.Init(parameters);
        hMac.BlockUpdate(bytesToSign, 0, bytesToSign.Length);
        byte[] array = new byte[hMac.GetMacSize()];
        hMac.DoFinal(array, 0);
        return array.Base64UrlEncode();
    }

    public void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        string value = GetSignature(secret, bytesToSign);
        if( value != signature )
            throw new SignatureVerificationException("Jwt Token signature verify failed");
    }
}


internal class Xx1JwtImpl : IJwtAlgorithm2
{
    public string Name => "";

    public string GetHeaderJson()
    {
        throw new NotImplementedException();
    }

    public string GetSignature(object secret, byte[] bytesToSign)
    {
        throw new NotImplementedException();
    }

    public void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        throw new NotImplementedException();
    }
}

internal class Xx2JwtImpl : IJwtAlgorithm2
{
    public string Name => "abc";

    public string GetHeaderJson()
    {
        return "";
    }

    public string GetSignature(object secret, byte[] bytesToSign)
    {
        throw new NotImplementedException();
    }

    public void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        throw new NotImplementedException();
    }
}