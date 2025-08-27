using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.WebClient;
[TestClass]
public class HttpOptionTest_Gzip
{

    [TestMethod]
    public void Test_Gzip_Text_2048()
    {
        string text = new string('中', 5000);

        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/show-body.aspx",
            Data = text,
            Format = SerializeFormat.Text
        };

        string response1 = http.GetResult();

        Assert.IsTrue(response1.Contains("Content-Type: text/plain"));
        Assert.IsFalse(response1.Contains("Content-Encoding: gzip"));

        Console.WriteLine("------------------------------------------------");
        Console.WriteLine(response1);

        Assert.IsTrue(response1.Contains(text));


        // ==============================================================================

        http.Finished = false;
        http.AutoGzipUpload = true;

        string response2 = http.GetResult();

        Assert.IsTrue(response2.Contains("Content-Type: text/plain"));
        Assert.IsTrue(response2.Contains("Content-Encoding: gzip"));

        Console.WriteLine("------------------------------------------------");
        //Console.WriteLine(response2);

        Assert.IsFalse(response2.Contains(text));

        string[] lines = response2.TrimEnd().ToLines();
        Assert.AreEqual(text, lines.Last().UnGzip());
    }



    [TestMethod]
    public void Test_Gzip_Json_2048()
    {
        NameValue data = new NameValue { Name = "abc", Value = new string('中', 2048) };

        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/show-body.aspx",
            Data = data,
            Format = SerializeFormat.Json
        };

        string response1 = http.GetResult();

        Assert.IsTrue(response1.Contains("Content-Type: application/json"));
        Assert.IsFalse(response1.Contains("Content-Encoding: gzip"));

        Console.WriteLine("------------------------------------------------");
        Console.WriteLine(response1);

        Assert.IsTrue(response1.Contains(data.Value));


        // ==============================================================================

        http.Finished = false;
        http.AutoGzipUpload = true;

        string response2 = http.GetResult();

        Assert.IsTrue(response2.Contains("Content-Type: application/json"));
        Assert.IsTrue(response2.Contains("Content-Encoding: gzip"));

        Console.WriteLine("------------------------------------------------");
        Console.WriteLine(response2);

        Assert.IsFalse(response2.Contains(data.Value));

        string[] lines = response2.TrimEnd().ToLines();
        Assert.AreEqual(data.ToJson(), lines.Last().UnGzip());
    }

}
