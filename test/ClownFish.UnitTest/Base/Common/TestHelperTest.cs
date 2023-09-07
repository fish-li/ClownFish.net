namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class TestHelperTest
{
    [TestMethod]
    public void Test_TryThrowException()
    {
        Exception e1 = new NotImplementedException("12345");
        TestHelper.SetException(e1);

        Exception exception = null;
        try {
            TestHelper.TryThrowException();
        }
        catch( Exception ex ) {
            exception = ex;
        }

        Assert.IsNotNull(exception);
        Assert.IsTrue(exception is NotImplementedException);
        Assert.AreEqual("12345", exception.Message);

        //Assert.IsNull(TestHelper.ExceptionForTest);
    }
}
