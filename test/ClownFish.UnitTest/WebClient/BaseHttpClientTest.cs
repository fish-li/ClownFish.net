using ClownFish.UnitTest.Base;

namespace ClownFish.UnitTest.WebClient;

[TestClass]
public class BaseHttpClientTest
{
    [TestMethod]
    public void Test()
    {
        HttpOption option = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };

        TestHttpClient clent1 = new TestHttpClient(option);
        TestHttpClient clent2 = new TestHttpClient(option);

        Assert.IsTrue(object.ReferenceEquals(option, clent1.HttpOption));
        Assert.IsTrue(object.ReferenceEquals(option, clent2.HttpOption));

        Assert.AreEqual(32, clent2.OperationId.Length);
        Assert.AreNotEqual(clent2.OperationId, clent1.OperationId);

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = clent1.GetResult2<Product2>(null);
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            TestHttpClient clent = new TestHttpClient(new HttpOption());
        });
    }
}


internal class TestHttpClient : BaseHttpClient
{
    public TestHttpClient(HttpOption option) : base(option)
    {
    }

    public override T Send<T>()
    {
        throw new NotImplementedException();
    }

    public override Task<T> SendAsync<T>()
    {
        throw new NotImplementedException();
    }

    public T GetResult2<T>(HttpWebResponse response)
    {
        return base.GetResult<T>(response);
    }
}
