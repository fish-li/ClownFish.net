namespace ClownFish.UnitTest.Base.Cache;

[TestClass]
public class CacheResultTest
{
    [TestMethod]
    public void Test()
    {
        CacheResult<string> result = new CacheResult<string>("abc", null);

        Assert.AreEqual("abc", result.Result);
        Assert.AreEqual("abc", result.TryGetResult());
    }

    [TestMethod]
    public void Test2()
    {
        Exception ex = ExceptionHelper.CreateException();
        CacheResult<string> result = new CacheResult<string>("abc", ex);

        Assert.AreEqual("abc", result.TryGetResult());
    }


    [ExpectedException(typeof(MessageException))]
    [TestMethod]
    public void Test3()
    {
        Exception ex = ExceptionHelper.CreateException();
        CacheResult<string> result = new CacheResult<string>("abc", ex);

        string xx = result.Result;
    }
}
