namespace ClownFish.UnitTest.Http.Utils;

[TestClass]
public class DownloadFileNameEncoderTest
{
    [TestMethod]
    public void Test()
    {
        string name = "中文_汉字.txt";
        string value = DownloadFileNameEncoder.Instance.GetFileNameHeader(name, string.Empty);

        Console.WriteLine(value);
        Assert.AreEqual("attachment; filename*=UTF-8''%e4%b8%ad%e6%96%87_%e6%b1%89%e5%ad%97.txt", value);            
    }


    [TestMethod]
    public void Test_ie8()
    {
        string name = "中文_汉字.txt";
        string userAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0)";
        string value = DownloadFileNameEncoder.Instance.GetFileNameHeader(name, userAgent);

        Console.WriteLine(value);
        Assert.AreEqual("attachment; filename=\"%e4%b8%ad%e6%96%87_%e6%b1%89%e5%ad%97.txt\"", value);
    }


    [TestMethod]
    public void Test_ie11()
    {
        string name = "中文_汉字.txt";
        string userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
        string value = DownloadFileNameEncoder.Instance.GetFileNameHeader(name, userAgent);

        Console.WriteLine(value);
        Assert.AreEqual("attachment; filename*=UTF-8''%e4%b8%ad%e6%96%87_%e6%b1%89%e5%ad%97.txt", value);
    }


    [TestMethod]
    public void Test_opera()
    {
        string name = "中文_汉字.txt";
        string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36 Edg/89.0.774.54";
        string value = DownloadFileNameEncoder.Instance.GetFileNameHeader(name, userAgent);

        Console.WriteLine(value);
        Assert.AreEqual("attachment; filename*=UTF-8''%e4%b8%ad%e6%96%87_%e6%b1%89%e5%ad%97.txt", value);
    }

    [TestMethod]
    public void Test_Empty()
    {
        string value1 = DownloadFileNameEncoder.Instance.GetFileNameHeader(null, null);
        Assert.AreEqual(string.Empty, value1);

        string value2 = DownloadFileNameEncoder.Instance.GetFileNameHeader(null, null);
        Assert.AreEqual(string.Empty, value2);
    }
}
