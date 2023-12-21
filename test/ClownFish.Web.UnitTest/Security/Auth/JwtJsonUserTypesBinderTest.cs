using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web.Security.Auth;

namespace ClownFish.Web.UnitTest.Security.Auth;
[TestClass]
public class JwtJsonUserTypesBinderTest
{
    static JwtJsonUserTypesBinderTest()
    {
        TypeHelper.RegisterAlias("Nebula.XXXXXXX.Security.WebUserInfo, Nebula.net", typeof(WebUserInfo));
    }

    [TestMethod]
    public void Test_shortname()
    {
        string json = @"{
    ""User"": {
        ""$type"": ""WebUserInfo"",
        ""TenantId"": ""tid_2222"",
        ""TenantCode"": ""tcode_333"",
        ""UserId"": ""id_111"",
        ""UserCode"": ""code111"",
        ""UserName"": ""name123"",
        ""UserRole"": ""admin"",
        ""UserType"": ""type111"",
        ""GrayFlag"": 0
    },
    ""Issuer"": ""Issuer_xxxxxxxxxx"",
    ""IssueTime"": 638387682287116057,
    ""Expiration"": 638413602287116148
}".Trim();

        JwtJsonSerializer jss1 = new JwtJsonSerializer(true);
        LoginTicket t1 = jss1.Deserialize<LoginTicket>(json);
        Assert.IsNotNull(t1.User);
        Assert.IsInstanceOfType(t1.User, typeof(WebUserInfo));


        MyAssert.IsError<JsonSerializationException>(() => {
            JwtJsonSerializer jss2 = new JwtJsonSerializer(false);

            // Newtonsoft.Json.JsonSerializationException:
            //      Error resolving type specified in JSON 'WebUserInfo'. Path 'User.$type', line 3, position 30.
            //      ---> System.ArgumentException: Value does not fall within the expected range. (Parameter 'typeName@11')

            LoginTicket t2 = jss2.Deserialize<LoginTicket>(json);
        });        
        
    }

    [TestMethod]
    public void Test_errorname()
    {
        string json = @"{
    ""User"": {
        ""$type"": ""Nebula.XXXXXXX.Security.WebUserInfo, Nebula.net"",
        ""TenantId"": ""tid_2222"",
        ""TenantCode"": ""tcode_333"",
        ""UserId"": ""id_111"",
        ""UserCode"": ""code111"",
        ""UserName"": ""name123"",
        ""UserRole"": ""admin"",
        ""UserType"": ""type111"",
        ""GrayFlag"": 0
    },
    ""Issuer"": ""Issuer_xxxxxxxxxx"",
    ""IssueTime"": 638387682287116057,
    ""Expiration"": 638413602287116148
}".Trim();

        JwtJsonSerializer jss1 = new JwtJsonSerializer(true);
        LoginTicket t1 = jss1.Deserialize<LoginTicket>(json);
        Assert.IsNotNull(t1.User);
        Assert.IsInstanceOfType(t1.User, typeof(WebUserInfo));


        JwtJsonSerializer jss2 = new JwtJsonSerializer(false);
        LoginTicket t2 = jss2.Deserialize<LoginTicket>(json);
        Assert.IsNotNull(t2.User);
        Assert.IsInstanceOfType(t2.User, typeof(WebUserInfo));
    }


    [TestMethod]
    public void Test_JwtJsonUserTypesBinder2()
    {
        LoginTicket ticket = new LoginTicket {
            User = new WebUserInfo {
                UserCode = "code111",
                UserName = "name123",
                UserId = "id_111",
                UserRole = "admin",
                TenantId = "tid_2222",
                TenantCode = "tcode_333",
                UserType = "type111"
            },
            IssueTime = DateTime.Now.Ticks,
            Expiration = DateTime.Now.AddDays(30).Ticks,
            Issuer = "Issuer_xxxxxxxxxx"
        };

        JwtJsonSerializer jss1 = new JwtJsonSerializer(true);
        string json1 = jss1.Serialize(ticket);

        //Console.WriteLine(json1);
        // {"User":{"$type":"WebUserInfo","TenantId":"tid_2222","TenantCode":"tcode_333","UserId":"id_111","UserCode":"code111","UserName":"name123","UserRole":"admin","UserType":"type111","GrayFlag":0},"Issuer":"Issuer_xxxxxxxxxx","IssueTime":638387700812199866,"Expiration":638413620812199964}
        Assert.IsTrue(json1.Contains("\"$type\":\"WebUserInfo\""));


        JwtJsonSerializer jss2 = new JwtJsonSerializer(false);
        string json2 = jss2.Serialize(ticket);

        //Console.WriteLine(json2);
        // {"User":{"$type":"ClownFish.Web.Security.Auth.WebUserInfo, ClownFish.Web","TenantId":"tid_2222","TenantCode":"tcode_333","UserId":"id_111","UserCode":"code111","UserName":"name123","UserRole":"admin","UserType":"type111","GrayFlag":0},"Issuer":"Issuer_xxxxxxxxxx","IssueTime":638387700812199866,"Expiration":638413620812199964}
        Assert.IsTrue(json2.Contains("\"$type\":\"ClownFish.Web.Security.Auth.WebUserInfo, ClownFish.Web\""));

    }
}
