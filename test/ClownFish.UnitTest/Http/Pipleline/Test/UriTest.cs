namespace ClownFish.UnitTest.Http.Pipleline.Test;

[TestClass]
public class UriTest
{
    private static void ShowUri(Uri uri)
    {
        Console.WriteLine("===================================================================");
        Console.WriteLine("OriginalString: " + uri.OriginalString);
        Console.WriteLine("AbsolutePath: " + uri.AbsolutePath);
        Console.WriteLine("AbsoluteUri: " + uri.AbsoluteUri);
        Console.WriteLine("PathAndQuery: " + uri.PathAndQuery);
        Console.WriteLine("Port: " + uri.Port);
        Console.WriteLine("Scheme: " + uri.Scheme);
        Console.WriteLine("Host: " + uri.Host);
        Console.WriteLine("LocalPath: " + uri.LocalPath);
        Console.WriteLine("Query: " + uri.Query);
        Console.WriteLine("Authority: " + uri.Authority);
        Console.WriteLine("DnsSafeHost: " + uri.DnsSafeHost);
        Console.WriteLine("Fragment: " + uri.Fragment);
        Console.WriteLine("IdnHost: " + uri.IdnHost);
        Console.WriteLine("Segments: " + uri.Segments.Merge("; "));
        Console.WriteLine("===================================================================");
    }

    [TestMethod]
    public void Test1()
    {
        // 下面这些代码主要是为了看一下 Uri 的各属性值是什么

        Uri u1 = new Uri("http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3");
        ShowUri(u1);
               

        Uri u2 = new Uri("https://www.abc.com/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3");
        ShowUri(u2);

        Uri u3 = new Uri("http://www.abc.com:333");
        ShowUri(u3);

        Uri u4 = new Uri("https://www.abc.com/");
        ShowUri(u4);

        Uri u5 = new Uri("http://www.abc.com");
        ShowUri(u5);

    }
}

// output: 
/*
===================================================================
OriginalString: http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3
AbsolutePath: /aaa/bb/ccc.aspx
AbsoluteUri: http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3
PathAndQuery: /aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3
Port: 14752
Scheme: http
Host: www.abc.com
LocalPath: /aaa/bb/ccc.aspx
Query: ?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3
Authority: www.abc.com:14752
DnsSafeHost: www.abc.com
Fragment: 
IdnHost: www.abc.com
Segments: /; aaa/; bb/; ccc.aspx
===================================================================
===================================================================
OriginalString: https://www.abc.com/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3
AbsolutePath: /aaa/bb/ccc.aspx
AbsoluteUri: https://www.abc.com/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3
PathAndQuery: /aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3
Port: 443
Scheme: https
Host: www.abc.com
LocalPath: /aaa/bb/ccc.aspx
Query: ?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3
Authority: www.abc.com
DnsSafeHost: www.abc.com
Fragment: 
IdnHost: www.abc.com
Segments: /; aaa/; bb/; ccc.aspx
===================================================================
===================================================================
OriginalString: http://www.abc.com:333
AbsolutePath: /
AbsoluteUri: http://www.abc.com:333/
PathAndQuery: /
Port: 333
Scheme: http
Host: www.abc.com
LocalPath: /
Query: 
Authority: www.abc.com:333
DnsSafeHost: www.abc.com
Fragment: 
IdnHost: www.abc.com
Segments: /
===================================================================
===================================================================
OriginalString: https://www.abc.com/
AbsolutePath: /
AbsoluteUri: https://www.abc.com/
PathAndQuery: /
Port: 443
Scheme: https
Host: www.abc.com
LocalPath: /
Query: 
Authority: www.abc.com
DnsSafeHost: www.abc.com
Fragment: 
IdnHost: www.abc.com
Segments: /
===================================================================
===================================================================
OriginalString: http://www.abc.com
AbsolutePath: /
AbsoluteUri: http://www.abc.com/
PathAndQuery: /
Port: 80
Scheme: http
Host: www.abc.com
LocalPath: /
Query: 
Authority: www.abc.com
DnsSafeHost: www.abc.com
Fragment: 
IdnHost: www.abc.com
Segments: /
===================================================================
*/