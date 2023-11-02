using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.WebClient;
[TestClass]public class HttpClientMockResultsTest
{
    [TestMethod]
    public void Test1()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            HttpClientMockResults.SetMockResult("", new object());
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            HttpClientMockResults.SetMockResult("1111111111", null);
        });

        HttpClientMockResults.SetMockResult("306230c5b15843118e30e14a1fe0cf43", "abc", false);
        Assert.IsNull(HttpClientMockResults.GetMockResult(""));
        Assert.IsNull(HttpClientMockResults.GetMockResult("c5cd532c769f45d9bf5709bb60c9e972"));

        Assert.IsNotNull(HttpClientMockResults.GetMockResult("306230c5b15843118e30e14a1fe0cf43"));
        HttpClientMockResults.Clear();
        Assert.IsNull(HttpClientMockResults.GetMockResult("306230c5b15843118e30e14a1fe0cf43"));

    }
}
