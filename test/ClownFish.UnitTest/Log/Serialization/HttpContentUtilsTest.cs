using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log;

#if NETCOREAPP

namespace ClownFish.UnitTest.Log.Serialization;
[TestClass]
public class HttpContentUtilsTest
{
    [TestMethod]
    public void Test_ReadBodyAsText_1()
    {
        string text = Guid.NewGuid().ToString();
        HttpContent content = new ByteArrayContent(text.GetBytes());
        string result = content.ReadBodyAsText();

        Assert.AreEqual(text, result);
    }

    [TestMethod]
    public void Test_ReadBodyAsText_2()
    {
        string text = Guid.NewGuid().ToString();
        MemoryStream ms = new MemoryStream(text.GetBytes());

        HttpContent content = new StreamContent(ms);
        string result = content.ReadBodyAsText();

        Assert.AreEqual(text, result);
    }

    [TestMethod]
    public void Test_ReadBodyAsText_3()
    {
        string text = Guid.NewGuid().ToString();
        HttpContent content = new StringContent(text);
        string result = content.ReadBodyAsText();

        Assert.AreEqual(text, result);
    }

    [TestMethod]
    public void Test_ReadBodyAsText_4()
    {
        HttpContent content = null;
        Assert.IsNull(content.ReadBodyAsText());


    }
}


#endif
