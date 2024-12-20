using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Web.UnitTest.Test;

// 有些复杂的Cookie值用SetCookieHeaderValue无法解析，所以只能自己写个简单的
// 目前在 Nebula.FoxFairy 中使用

internal static class SetCookieValueParser
{
    private static readonly char[] s_separator = new char[] { ';' };

    public static string RemoveSome(string cookieValue)
    {
        if( cookieValue.IsNullOrEmpty() )
            return cookieValue;

        string[] items = cookieValue.Split(s_separator, StringSplitOptions.RemoveEmptyEntries);

        StringBuilder sb = StringBuilderPool.Get();
        try {
            for( int i = 0; i < items.Length; i++ ) {
                string item = items[i];
                item = item.Trim(' ');

                if( i == 0 ) {
                    sb.Append(item);
                    continue;
                }

                NameValue nv = ParseItemValue(item);

                // 只保留这几个设置
                if( "Path".Is(nv.Name) || "HttpOnly".Is(nv.Name) || "Expires".Is(nv.Name) || "Max-Age".Is(nv.Name) )
                    sb.Append("; ").Append(item);
            }
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    internal static NameValue ParseItemValue(string item)
    {
        int p = item.IndexOf('=');
        if( p < 0 )
            return new NameValue(item, null);
        else
            return new NameValue(item.Substring(0, p), item.Substring(p + 1));
    }
}




[TestClass]
public class SetCookieValueParserTest
{
    [TestMethod]
    public void Test_ParseItemValue()
    {
        NameValue nv1 = SetCookieValueParser.ParseItemValue("a=b");
        Assert.AreEqual("a", nv1.Name);
        Assert.AreEqual("b", nv1.Value);

        NameValue nv2 = SetCookieValueParser.ParseItemValue("aa");
        Assert.AreEqual("aa", nv2.Name);
        Assert.IsNull(nv2.Value);

        NameValue nv3 = SetCookieValueParser.ParseItemValue("aa=");
        Assert.AreEqual("aa", nv3.Name);
        Assert.AreEqual("", nv3.Value);

        NameValue nv4 = SetCookieValueParser.ParseItemValue("a=b c");
        Assert.AreEqual("a", nv4.Name);
        Assert.AreEqual("b c", nv4.Value);
    }

    [TestMethod]
    public void Test1()
    {
        string value1 = "Login_User=UserCode=liqf01&SignTime=2024/12/19 11:16:43&UserSign=CFae/a7R0krmj1VFzKWVJrz5sQWtl6NEit4fQ2DOUXaX5uvmvZ2f+XXU8QbrbVet6uadsaurxv2hQ/QHlyhwEbZ2gINt7t5mADoBN+Lkr6jeIxv+84FKoQTtkPouQSDTln+d+wsX2bppJXndbBWKiW2exPvEwhxwFTbf/uBwxzE=; domain=mingyuanyun.com; expires=Fri, 20-Dec-2024 03:16:43 GMT; path=/";
        string value2 = SetCookieValueParser.RemoveSome(value1);
        Assert.AreEqual(value2, "Login_User=UserCode=liqf01&SignTime=2024/12/19 11:16:43&UserSign=CFae/a7R0krmj1VFzKWVJrz5sQWtl6NEit4fQ2DOUXaX5uvmvZ2f+XXU8QbrbVet6uadsaurxv2hQ/QHlyhwEbZ2gINt7t5mADoBN+Lkr6jeIxv+84FKoQTtkPouQSDTln+d+wsX2bppJXndbBWKiW2exPvEwhxwFTbf/uBwxzE=; expires=Fri, 20-Dec-2024 03:16:43 GMT; path=/");
    }

    [TestMethod]
    public void Test2()
    {
        string value1 = "Email_User=UserCode=liqf01&SignTime=2024/12/19 11:16:43&UserSign=CFae/a7R0krmj1VFzKWVJrz5sQWtl6NEit4fQ2DOUXaX5uvmvZ2f+XXU8QbrbVet6uadsaurxv2hQ/QHlyhwEbZ2gINt7t5mADoBN+Lkr6jeIxv+84FKoQTtkPouQSDTln+d+wsX2bppJXndbBWKiW2exPvEwhxwFTbf/uBwxzE=; expires=Fri, 20-Dec-2024 03:16:43 GMT; path=/";
        string value2 = SetCookieValueParser.RemoveSome(value1);
        Assert.AreEqual(value2, "Email_User=UserCode=liqf01&SignTime=2024/12/19 11:16:43&UserSign=CFae/a7R0krmj1VFzKWVJrz5sQWtl6NEit4fQ2DOUXaX5uvmvZ2f+XXU8QbrbVet6uadsaurxv2hQ/QHlyhwEbZ2gINt7t5mADoBN+Lkr6jeIxv+84FKoQTtkPouQSDTln+d+wsX2bppJXndbBWKiW2exPvEwhxwFTbf/uBwxzE=; expires=Fri, 20-Dec-2024 03:16:43 GMT; path=/");
    }

    [TestMethod]
    public void Test3()
    {
        string value1 = "MyPPS_User=UserIP=27.17.30.203, 112.124.159.104:38360; expires=Fri, 20-Dec-2024 03:16:43 GMT; path=/";
        string value2 = SetCookieValueParser.RemoveSome(value1);
        Assert.AreEqual(value2, "MyPPS_User=UserIP=27.17.30.203, 112.124.159.104:38360; expires=Fri, 20-Dec-2024 03:16:43 GMT; path=/");
    }

    [TestMethod]
    public void Test4()
    {
        string value1 = "ASP.NET_SessionId=h41exur05lk3jrxrkr2dcbmy; path=/; HttpOnly; SameSite=Lax";
        string value2 = SetCookieValueParser.RemoveSome(value1);
        Assert.AreEqual(value2, "ASP.NET_SessionId=h41exur05lk3jrxrkr2dcbmy; path=/; HttpOnly");
    }

    [TestMethod]
    public void Test5()
    {
        string value1 = "asweb=fco123stq919016pjpqa3gbi36; path=/; HttpOnly";
        string value2 = SetCookieValueParser.RemoveSome(value1);
        Assert.AreEqual(value2, "asweb=fco123stq919016pjpqa3gbi36; path=/; HttpOnly");
    }


    [TestMethod]
    public void Test6()
    {
        string value1 = "SERVERID=c4e8dcc88c9dc307460619b9acb47b01|1734577385|1734577253;Path=/";
        string value2 = SetCookieValueParser.RemoveSome(value1);
        Assert.AreEqual(value2, "SERVERID=c4e8dcc88c9dc307460619b9acb47b01|1734577385|1734577253; Path=/");
    }

    [TestMethod]
    public void Test7()
    {
        string value1 = "acw_tc=1a0c595917345773853444499e0132bb0c8b86636e6a557c3700020be9a354;path=/;HttpOnly;Max-Age=1800";
        string value2 = SetCookieValueParser.RemoveSome(value1);
        Assert.AreEqual(value2, "acw_tc=1a0c595917345773853444499e0132bb0c8b86636e6a557c3700020be9a354; path=/; HttpOnly; Max-Age=1800");
    }

    [TestMethod]
    public void Test8()
    {
        string value1 = "ApplicationGatewayAffinity=2b63240f22f75f354019b36960ba9166ddd13f550a5864625cb10790f36b00a5;Path=/;Domain=login.mingyuanyun.com";
        string value2 = SetCookieValueParser.RemoveSome(value1);
        Assert.AreEqual(value2, "ApplicationGatewayAffinity=2b63240f22f75f354019b36960ba9166ddd13f550a5864625cb10790f36b00a5; Path=/");
    }

    [TestMethod]
    public void Test9()
    {
        string value1 = "_identity-pas=deleted; expires=Thu, 01-Jan-1970 00:00:01 GMT; Max-Age=0; path=/; httponly";
        string value2 = SetCookieValueParser.RemoveSome(value1);
        Assert.AreEqual(value2, "_identity-pas=deleted; expires=Thu, 01-Jan-1970 00:00:01 GMT; Max-Age=0; path=/; httponly");
    }

    [TestMethod]
    public void Test10()
    {
        string value1 = "admin=3pl4t6o0g1mtt4qpf3lq3opav1; expires=Sun, 27-Aug-2056 04:49:48 GMT; Max-Age=999999999; path=/; HttpOnly";
        string value2 = SetCookieValueParser.RemoveSome(value1);
        Assert.AreEqual(value2, "admin=3pl4t6o0g1mtt4qpf3lq3opav1; expires=Sun, 27-Aug-2056 04:49:48 GMT; Max-Age=999999999; path=/; HttpOnly");
    }

    [TestMethod]
    public void Test11()
    {
        string value1 = "apprev=v1; expires=Fri, 20-Dec-2024 03:03:09 GMT; Max-Age=86400; path=/";
        string value2 = SetCookieValueParser.RemoveSome(value1);
        Assert.AreEqual(value2, "apprev=v1; expires=Fri, 20-Dec-2024 03:03:09 GMT; Max-Age=86400; path=/");
    }
}

