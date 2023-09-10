using ClownFish.Base.Jwt;
using ClownFish.Web.Security.Auth;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;

#pragma warning disable CS0618
namespace ClownFish.Web.UnitTest.Security.Jwt;

[TestClass]
public class PerformanceTest
{
    //[TestMethod]
    public void Run()
    {
        for(int i =0; i< 10; i++) {
            Run0();
        }

    }

    private void Run0()
    {
        int runCount = 10_0000;

        // 先预热
        string token1 = Encode1();
        string token2 = Encode2();
        LoginTicket2 ticket1 = Decode1(token1);
        LoginTicket2 ticket2 = Decode2(token2);



        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Restart();
        for( int i = 0; i < runCount; i++ ) {
            token1 = Encode1();
        }
        stopwatch.Stop();
        Console.WriteLine($"ClownFish-JWT 生成 Token {runCount} 次耗时：" + stopwatch.Elapsed.ToString());


        stopwatch.Restart();
        for( int i = 0; i < runCount; i++ ) {
            token2 = Encode2();
        }
        stopwatch.Stop();
        Console.WriteLine($"第三方-JWT类库 生成 Token {runCount} 次耗时：" + stopwatch.Elapsed.ToString());


        if( token1 != token2 )
            throw new ApplicationException("token1 != token2");

        Console.WriteLine("------------------------------------------------------------------");


        stopwatch.Restart();
        for( int i = 0; i < runCount; i++ ) {
            ticket1 = Decode1(token1);
        }
        stopwatch.Stop();
        Console.WriteLine($"ClownFish-JWT 解析 Token {runCount} 次耗时：" + stopwatch.Elapsed.ToString());



        stopwatch.Restart();
        for( int i = 0; i < runCount; i++ ) {
            ticket2 = Decode2(token1);
        }
        stopwatch.Stop();
        Console.WriteLine($"第三方-JWT类库 解析 Token {runCount} 次耗时：" + stopwatch.Elapsed.ToString());


        if( ticket1.ToJson() != ticket2.ToJson() )
            throw new ApplicationException("ticket1 != ticket2");

        Console.WriteLine("------------------------------------------------------------------");
    }


    private static readonly LoginTicket2 s_ticket = new LoginTicket2 {
        User = JwtTest.WebUser,
        Issuer = "ClownFish.Web.UnitTest",
        IssueTime = (new DateTime(2000, 1, 1)).Ticks,
        Expiration = DateTime.MaxValue.Ticks,
    };

    private string Encode1()
    {
        string json = s_ticket.ToJson();
        return JwtUtils.Encode(json, JwtTest.JwtKey, "HS256");
    }

    private LoginTicket2 Decode1(string token)
    {
        string json = JwtUtils.Decode(token, JwtTest.JwtKey, "HS256");
        return json.FromJson<LoginTicket2>();
    }


    private string Encode2()
    {
        IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
        IJsonSerializer serializer = new JsonNetSerializer();
        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

        return encoder.Encode(s_ticket, JwtTest.JwtKey);
    }

    private LoginTicket2 Decode2(string token)
    {
        return JwtBuilder.Create()
                        .WithAlgorithm(new HMACSHA256Algorithm())
                        .WithSecret(JwtTest.JwtKey)
                        .MustVerifySignature()
                        .Decode<LoginTicket2>(token);
    }
}
#pragma warning restore CS0618



public sealed class LoginTicket2
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public WebUserInfo User { get; set; }

    /// <summary>
    /// 凭证的发布者
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// 凭证创建时间
    /// </summary>
    public long IssueTime { get; set; }

    /// <summary>
    /// 凭证有效截止时间
    /// </summary>
    public long Expiration { get; set; }
}




/* 
 * 
ClownFish-JWT 生成 Token 100000 次耗时：00:00:00.4934064
第三方-JWT类库 生成 Token 100000 次耗时：00:00:00.3680080
------------------------------------------------------------------
ClownFish-JWT 解析 Token 100000 次耗时：00:00:00.4495074
第三方-JWT类库 解析 Token 100000 次耗时：00:00:00.9655231
------------------------------------------------------------------
ClownFish-JWT 生成 Token 100000 次耗时：00:00:00.3157930
第三方-JWT类库 生成 Token 100000 次耗时：00:00:00.3308798
------------------------------------------------------------------
ClownFish-JWT 解析 Token 100000 次耗时：00:00:00.4375244
第三方-JWT类库 解析 Token 100000 次耗时：00:00:00.9511799
------------------------------------------------------------------
ClownFish-JWT 生成 Token 100000 次耗时：00:00:00.3117610
第三方-JWT类库 生成 Token 100000 次耗时：00:00:00.3354114
------------------------------------------------------------------
ClownFish-JWT 解析 Token 100000 次耗时：00:00:00.4290904
第三方-JWT类库 解析 Token 100000 次耗时：00:00:00.9404687
------------------------------------------------------------------
ClownFish-JWT 生成 Token 100000 次耗时：00:00:00.3146105
第三方-JWT类库 生成 Token 100000 次耗时：00:00:00.3291869
------------------------------------------------------------------
ClownFish-JWT 解析 Token 100000 次耗时：00:00:00.4292509
第三方-JWT类库 解析 Token 100000 次耗时：00:00:00.9520207
------------------------------------------------------------------
ClownFish-JWT 生成 Token 100000 次耗时：00:00:00.3147319
第三方-JWT类库 生成 Token 100000 次耗时：00:00:00.3256219
------------------------------------------------------------------
ClownFish-JWT 解析 Token 100000 次耗时：00:00:00.4321082
第三方-JWT类库 解析 Token 100000 次耗时：00:00:00.9568863
------------------------------------------------------------------
ClownFish-JWT 生成 Token 100000 次耗时：00:00:00.3167135
第三方-JWT类库 生成 Token 100000 次耗时：00:00:00.3323634
------------------------------------------------------------------
ClownFish-JWT 解析 Token 100000 次耗时：00:00:00.4347459
第三方-JWT类库 解析 Token 100000 次耗时：00:00:00.9502265
------------------------------------------------------------------
ClownFish-JWT 生成 Token 100000 次耗时：00:00:00.3144964
第三方-JWT类库 生成 Token 100000 次耗时：00:00:00.3284639
------------------------------------------------------------------
ClownFish-JWT 解析 Token 100000 次耗时：00:00:00.4353345
第三方-JWT类库 解析 Token 100000 次耗时：00:00:00.9473577
------------------------------------------------------------------
ClownFish-JWT 生成 Token 100000 次耗时：00:00:00.3200246
第三方-JWT类库 生成 Token 100000 次耗时：00:00:00.3313957
------------------------------------------------------------------
ClownFish-JWT 解析 Token 100000 次耗时：00:00:00.4333547
第三方-JWT类库 解析 Token 100000 次耗时：00:00:00.9453798
------------------------------------------------------------------
ClownFish-JWT 生成 Token 100000 次耗时：00:00:00.3150689
第三方-JWT类库 生成 Token 100000 次耗时：00:00:00.3283957
------------------------------------------------------------------
ClownFish-JWT 解析 Token 100000 次耗时：00:00:00.4293454
第三方-JWT类库 解析 Token 100000 次耗时：00:00:00.9568694
------------------------------------------------------------------
ClownFish-JWT 生成 Token 100000 次耗时：00:00:00.3196599
第三方-JWT类库 生成 Token 100000 次耗时：00:00:00.3263463
------------------------------------------------------------------
ClownFish-JWT 解析 Token 100000 次耗时：00:00:00.4275421
第三方-JWT类库 解析 Token 100000 次耗时：00:00:00.9444699
------------------------------------------------------------------

*/